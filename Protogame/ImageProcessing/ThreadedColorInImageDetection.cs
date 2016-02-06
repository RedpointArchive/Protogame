using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Protogame
{
    /// <summary>
    /// An implementation of <see cref="IColorInImageDetection"/> that uses
    /// a background thread for performing analysis.
    /// </summary>
    /// <module>Image Processing</module>
    /// <internal>True</internal>
    /// <interface_ref>Protogame.IColorInImageDetection</interface_ref>
    public class ThreadedColorInImageDetection : IColorInImageDetection
    {
        private readonly Dictionary<ISelectedColorHandle, DetectedColor> _colorsToDetect;
        private readonly IImageSource _source;
        private readonly Thread _thread;

        private int _chunkSize = 5;

        private bool _isStarted;

        public ThreadedColorInImageDetection(IImageSource source)
        {
            _source = source;
            _colorsToDetect = new Dictionary<ISelectedColorHandle, DetectedColor>();

            _thread = new Thread(ProcessorThread);
            _thread.IsBackground = true;
        }

        public string GetNameForColor(ISelectedColorHandle handle)
        {
            return _colorsToDetect[handle].Name;
        }

        public Color GetValueForColor(ISelectedColorHandle handle)
        {
            return _colorsToDetect[handle].Color;
        }

        public float GlobalSensitivity { get; set; } = 1/10000000f*25f;

        public ISelectedColorHandle RegisterColorForAnalysis(Color color, string name)
        {
            var handle = new PrivateSelectedColorHandle();
            var registeredColor = new DetectedColor
            {
                Color = color,
                Name = name
            };
            _colorsToDetect.Add(handle, registeredColor);
            return handle;
        }

        public int[,] GetUnlockedResultsForColor(ISelectedColorHandle handle)
        {
            return _colorsToDetect[handle].RecognisedArray;
        }

        public float GetSensitivityForColor(ISelectedColorHandle handle)
        {
            return _colorsToDetect[handle].Sensitivity;
        }

        public int GetTotalDetectedForColor(ISelectedColorHandle handle)
        {
            return _colorsToDetect[handle].TotalDetected;
        }

        public int ChunkSize
        {
            get { return _chunkSize; }
            set
            {
                if (_isStarted)
                {
                    throw new InvalidOperationException(
                        "You can not change the chunk size after image analysis has started.");
                }

                _chunkSize = value;
            }
        }

        public void Start()
        {
            if (_isStarted)
            {
                throw new InvalidOperationException("Can't start continuous analysis of a texture twice.");
            }

            _thread.Start();
            _isStarted = true;
        }

        public void Dispose()
        {
            if (_isStarted)
            {
                _thread.Abort();
                _isStarted = false;
            }
        }

        private void ProcessorThread()
        {
            while (true)
            {
                try
                {
                    var total = 0;

                    foreach (var color in _colorsToDetect.Values)
                    {
                        color.UnlockedTotalDetected = 0;
                    }

                    int width, height;
                    var copy = _source.GetSourceAsBytes(out width, out height);
                    if (copy != null)
                    {
                        for (var x = 0; x < width/ChunkSize; x++)
                        {
                            for (var y = 0; y < height/ChunkSize; y++)
                            {
                                foreach (var color in _colorsToDetect.Values)
                                {
                                    var rT = color.Color.R;
                                    var gT = color.Color.G;
                                    var bT = color.Color.B;

                                    var rI = 255 - color.Color.R;
                                    var gI = 255 - color.Color.G;
                                    var bI = 255 - color.Color.B;

                                    if (color.RecognisedArray == null ||
                                        color.RecognisedArray.GetLength(0) != width/_chunkSize ||
                                        color.RecognisedArray.GetLength(1) != height/_chunkSize)
                                    {
                                        color.RecognisedArray =
                                            new int[width/_chunkSize,
                                                height/_chunkSize];
                                    }

                                    var chunkScore = 0;
                                    for (var xx = 0; xx < _chunkSize; xx++)
                                    {
                                        for (var yy = 0; yy < _chunkSize; yy++)
                                        {
                                            var idx = (x*_chunkSize + xx + (y*_chunkSize + yy)*width)*4;
                                            var rA = copy[idx];
                                            var gA = copy[idx + 1];
                                            var bA = copy[idx + 2];

                                            var illumination = (rA + gA + bA)/3f/255f;

                                            var redOnTarget = 255 - Math.Abs(rT - rA);
                                            var greenOnTarget = 255 - Math.Abs(gT - gA);
                                            var blueOnTarget = 255 - Math.Abs(bT - bA);
                                            var onTarget = (int) ((redOnTarget + greenOnTarget + blueOnTarget)/1f);

                                            var redOffTarget = 255 - Math.Abs(rI - rA);
                                            var greenOffTarget = 255 - Math.Abs(gI - gA);
                                            var blueOffTarget = 255 - Math.Abs(bI - bA);
                                            var offTarget = (int) ((redOffTarget + greenOffTarget + blueOffTarget)/0.5f);

                                            chunkScore += (int) ((onTarget - offTarget)*illumination);
                                            color.UnlockedTotalDetected += (int) (illumination*255f);
                                        }
                                    }

                                    color.RecognisedArray[x, y] = chunkScore;
                                }
                            }
                        }
                    }

                    foreach (var color in _colorsToDetect.Values)
                    {
                        color.Sensitivity = Math.Max(0, color.TotalDetected)*GlobalSensitivity;
                        color.TotalDetected = color.UnlockedTotalDetected;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                Thread.Sleep(20);
            }
        }

        private class PrivateSelectedColorHandle : ISelectedColorHandle
        {
        }

        private class DetectedColor
        {
            public Color Color;

            public string Name;

            public int[,] RecognisedArray;

            public float Sensitivity;

            public int TotalDetected;

            public int UnlockedTotalDetected;
        }
    }
}