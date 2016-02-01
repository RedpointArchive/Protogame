#if PLATFORM_WINDOWS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using DirectShowLib;
using Microsoft.Xna.Framework.Graphics;

namespace Protogame
{
    public class VideoCapture : ISampleGrabberCB, IDisposable
    {
        private static readonly object lockObj = new object();

        protected ICaptureGraphBuilder2 CaptureGraphBuilder;
        protected Texture2D frame;
        protected byte[] FrameBGR;
        protected bool FrameReady;
        protected byte[] FrameRGBA;
        protected IGraphBuilder GraphBuilder;
        protected GraphicsDevice GraphicsDevice;
        protected int Height = 480;
        private bool isRunning;
        protected IMediaControl MediaControl;
        protected ISampleGrabber SampleGrabber;
        protected Thread UpdateThread;
        protected int Width = 640;
        private int DeviceIndex;
        private DirectShowCamera Camera;

        public VideoCapture(GraphicsDevice GraphicsDevice, ICamera camera)
        {
            this.GraphicsDevice = GraphicsDevice;
            Camera = (DirectShowCamera)camera;
            Initialize();
        }

        public Texture2D Frame
        {
            get
            {
                if (frame.GraphicsDevice.Textures[0] == frame)
                    frame.GraphicsDevice.Textures[0] = null;
                lock (lockObj)
                {
                    frame.SetData(FrameRGBA);
                }
                return frame;
            }
        }

        public byte[] UnlockedFrameRGBA
        {
            get { return FrameRGBA; }
        }

        private class DirectShowCamera : ICamera
        {
            public string Name { get; set; }

            public int Width { get; set; }

            public int Height { get; set; }

            public int Index { get; set; }
        }

        private IBaseFilter GetVideoInputObjectForCamera(DirectShowCamera camera)
        {
            object VideoInputObject = null;
            IBaseFilter VideoInput = null;
            IEnumMoniker classEnum;
            var devEnum = (ICreateDevEnum)new CreateDevEnum();
            devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, 0);
            Marshal.ReleaseComObject(devEnum);
            if (classEnum != null)
            {
                var grabbingDevices = true;
                var allMonikers = new List<IMoniker>();
                while (true)
                {
                    var temp = new IMoniker[1];
                    classEnum.Next(1, temp, IntPtr.Zero);
                    if (temp[0] == null)
                    {
                        break;
                    }
                    allMonikers.Add(temp[0]);
                }
                var iid = typeof(IBaseFilter).GUID;
                var idx = 0;
                foreach (var validMoniker in allMonikers)
                {
                    if (idx == camera.Index)
                    {
                        validMoniker.BindToObject(null, null, ref iid, out VideoInputObject);
                        var result = ((IBaseFilter)VideoInputObject);
                        Marshal.ReleaseComObject(validMoniker);
                        Marshal.ReleaseComObject(classEnum);
                        return result;
                    }

                    Marshal.ReleaseComObject(validMoniker);

                    idx++;
                }

                Marshal.ReleaseComObject(classEnum);
            }

            return null;
        }

        public static List<ICamera> GetDirectShowCameras()
        {
            var cameras = new List<ICamera>();
            object VideoInputObject = null;
            IBaseFilter VideoInput = null;
            IEnumMoniker classEnum;
            var devEnum = (ICreateDevEnum)new CreateDevEnum();
            devEnum.CreateClassEnumerator(FilterCategory.VideoInputDevice, out classEnum, 0);
            Marshal.ReleaseComObject(devEnum);
            if (classEnum != null)
            {
                var grabbingDevices = true;
                var allMonikers = new List<IMoniker>();
                while (true)
                {
                    var temp = new IMoniker[1];
                    classEnum.Next(1, temp, IntPtr.Zero);
                    if (temp[0] == null)
                    {
                        break;
                    }
                    allMonikers.Add(temp[0]);
                }
                var iid = typeof(IBaseFilter).GUID;
                var piid = typeof(IPropertyBag).GUID;
                var idx = 0;
                foreach (var validMoniker in allMonikers)
                {
                    object propertyBagObject = null;
                    validMoniker.BindToObject(null, null, ref iid, out VideoInputObject);
                    validMoniker.BindToStorage(null, null, ref piid, out propertyBagObject);
                    var propertyBag = (IPropertyBag)propertyBagObject;
                    object descResult = null;
                    if (propertyBag.Read("Description", out descResult, null) != 0)
                    {
                        if (propertyBag.Read("FriendlyName", out descResult, null) != 0)
                        {
                            descResult = "(unknown device)";
                        }
                    }

                    var camera = new DirectShowCamera
                    {
                        Name = descResult as string,
                        Index = idx,
                        Width = 640, // TODO Fix
                        Height = 480,
                    };
                    cameras.Add(camera);

                    Marshal.ReleaseComObject(VideoInputObject);
                    Marshal.ReleaseComObject(validMoniker);
                    Marshal.ReleaseComObject(classEnum);

                    idx++;
                }
            }
            return cameras;
        }

        public void Dispose()
        {
            isRunning = false;
            Thread.Sleep(100);
            UpdateThread.Abort();
            if (MediaControl != null)
                MediaControl.StopWhenReady();
            Marshal.ReleaseComObject(MediaControl);
            Marshal.ReleaseComObject(GraphBuilder);
            Marshal.ReleaseComObject(CaptureGraphBuilder);
            CaptureGraphBuilder = null;
            GraphBuilder = null;
            MediaControl = null;
            frame.Dispose();
            frame = null;
            Marshal.ReleaseComObject(SampleGrabber);
            SampleGrabber = null;
        }

        public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            lock (lockObj)
            {
                Marshal.Copy(pBuffer, FrameBGR, 0, BufferLen);
            }
            FrameReady = true;
            return 0;
        }

        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            return 0;
        }

        protected void Initialize()
        {
            FrameReady = false;
            frame = new Texture2D(GraphicsDevice, Width, Height, false, SurfaceFormat.Color);
            FrameBGR = new byte[Width*Height*3];
            FrameRGBA = new byte[Width*Height*4];
            GraphBuilder = (IGraphBuilder) new FilterGraph();
            CaptureGraphBuilder = (ICaptureGraphBuilder2) new CaptureGraphBuilder2();
            MediaControl = (IMediaControl) GraphBuilder;
            CaptureGraphBuilder.SetFiltergraph(GraphBuilder);
            var videoInput = GetVideoInputObjectForCamera(Camera);
            if (videoInput != null)
            {
                isRunning = true;
                SampleGrabber = new SampleGrabber() as ISampleGrabber;
                GraphBuilder.AddFilter((IBaseFilter) SampleGrabber, "Render");
                var Type = new AMMediaType
                {
                    majorType = MediaType.Video,
                    subType = MediaSubType.RGB24,
                    formatType = FormatType.VideoInfo
                };
                SampleGrabber.SetMediaType(Type);
                GraphBuilder.AddFilter(videoInput, "Camera");
                SampleGrabber.SetBufferSamples(false);
                SampleGrabber.SetOneShot(false);
                SampleGrabber.GetConnectedMediaType(new AMMediaType());
                SampleGrabber.SetCallback(this, 1);
                CaptureGraphBuilder.RenderStream(PinCategory.Preview, MediaType.Video, videoInput, null,
                    SampleGrabber as IBaseFilter);
                UpdateThread = new Thread(UpdateBuffer);
                UpdateThread.IsBackground = true;
                UpdateThread.Start();
                MediaControl.Run();
                Marshal.ReleaseComObject(videoInput);
            }
        }

        protected void UpdateBuffer()
        {
            var samplePosRGBA = 0;
            var samplePosRGB24 = 0;
            while (isRunning)
            {
                lock (lockObj)
                {
                    for (int y = 0, y2 = Height - 1; y < Height; y++, y2--)
                    {
                        for (var x = 0; x < Width; x++)
                        {
                            samplePosRGBA = (y2*Width + x)*4;
                            samplePosRGB24 = (y*Width + (Width - x - 1))*3;
                            FrameRGBA[samplePosRGBA + 0] = FrameBGR[samplePosRGB24 + 2];
                            FrameRGBA[samplePosRGBA + 1] = FrameBGR[samplePosRGB24 + 1];
                            FrameRGBA[samplePosRGBA + 2] = FrameBGR[samplePosRGB24 + 0];
                            FrameRGBA[samplePosRGBA + 3] = 255;
                        }
                    }
                }
                FrameReady = false;
                while (!FrameReady) Thread.Sleep(20);
            }
        }
    }
}

#endif