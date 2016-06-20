using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;

namespace Protogame
{
    public class ThreadedPointInAnalysedImageDetection : IPointInAnalysedImageDetection
    {
        private readonly IColorInImageDetection _colorInImageDetection;
        private readonly ISelectedColorHandle _selectedColorHandle;
        private readonly Thread _thread;

        private bool _isStarted = false;

        private bool[,] _checkArray;

        public int MinNumberOfPoints { get; set; } = 1;

        public int MaxNumberOfPoints { get; set; } = 100;

        public float PointThreshold { get; set; } = 10f;
        public int Width {get { return _checkArray?.GetLength(0) ?? 1; } }
        public int Height { get { return _checkArray?.GetLength(1) ?? 1; } }

        public ThreadedPointInAnalysedImageDetection(
            IColorInImageDetection colorInImageDetection,
            ISelectedColorHandle selectedColorHandle)
        {
            _colorInImageDetection = colorInImageDetection;
            _selectedColorHandle = selectedColorHandle;
            
            _thread = new Thread(ProcessorThread);
            _thread.IsBackground = true;
        }

        private void ProcessorThread()
        {
            while (true)
            {
                try
                {
                    var recogArray = _colorInImageDetection.GetUnlockedResultsForColor(_selectedColorHandle);
                    if (recogArray == null)
                    {
                        Thread.Sleep(1000);
                        continue;
                    }

                    var sensitivity = _colorInImageDetection.GetSensitivityForColor(_selectedColorHandle);

                    var detectedPoints = new List<Vector2>();

                    if (_checkArray == null ||
                        _checkArray.GetLength(0) != recogArray.GetLength(0) ||
                        _checkArray.GetLength(1) != recogArray.GetLength(1))
                    {
                        _checkArray =
                            new bool[recogArray.GetLength(0), recogArray.GetLength(1)];
                    }
                    else
                    {
                        for (var x = 0; x < _checkArray.GetLength(0); x++)
                        {
                            for (var y = 0; y < _checkArray.GetLength(1); y++)
                            {
                                _checkArray[x, y] = false;
                            }
                        }
                    }

                    for (var x = 0; x < recogArray.GetLength(0); x++)
                    {
                        for (var y = 0; y < recogArray.GetLength(1); y++)
                        {
                            var score = recogArray[x, y] / sensitivity;

                            if (score > PointThreshold && !_checkArray[x, y])
                            {
                                if (FloodFillCheckAt(recogArray, _checkArray, x, y))
                                {
                                    detectedPoints.Add(new Vector2(x, y));
                                }
                            }
                        }
                    }

                    DetectedPoints = detectedPoints;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                }

                Thread.Sleep(20);
            }
        }

        public List<Vector2> DetectedPoints { get; private set; }

        private bool FloodFillCheckAt(int[,] detectionArray, bool[,] checkArray, int x, int y)
        {
            var totalCount = 0;
            var toCheck = new Stack<KeyValuePair<int, int>>();
            toCheck.Push(new KeyValuePair<int, int>(x, y));
            while (toCheck.Count > 0)
            {
                var toScan = toCheck.ToList();
                toCheck.Clear();

                foreach (var s in toScan)
                {
                    var xx = s.Key;
                    var yy = s.Value;

                    if (checkArray[xx, yy])
                    {
                        continue;
                    }

                    if (detectionArray[xx, yy] > PointThreshold)
                    {
                        totalCount++;

                        if (totalCount > MaxNumberOfPoints)
                        {
                            // We can't be true beyond this point.
                            return false;
                        }

                        checkArray[xx, yy] = true;

                        if (xx >= 1 && xx < checkArray.GetLength(0) - 1)
                        {
                            if (yy >= 1 && yy < checkArray.GetLength(1) - 1)
                            {
                                toCheck.Push(new KeyValuePair<int, int>(xx - 1, yy - 1));
                                toCheck.Push(new KeyValuePair<int, int>(xx - 1, yy));
                                toCheck.Push(new KeyValuePair<int, int>(xx - 1, yy + 1));
                                toCheck.Push(new KeyValuePair<int, int>(xx, yy - 1));
                                toCheck.Push(new KeyValuePair<int, int>(xx, yy + 1));
                                toCheck.Push(new KeyValuePair<int, int>(xx + 1, yy - 1));
                                toCheck.Push(new KeyValuePair<int, int>(xx + 1, yy));
                                toCheck.Push(new KeyValuePair<int, int>(xx + 1, yy + 1));
                            }
                        }
                    }
                }
            }

            if (totalCount >= MinNumberOfPoints && totalCount <= MaxNumberOfPoints)
            {
                return true;
            }

            return false;
        }

        public void Start()
        {
            if (!_isStarted)
            {
                _thread.Start();
            }
        }
        
        public void Dispose()
        {
            if (_isStarted)
            {
                _thread.Abort();
                _isStarted = false;
            }
        }
    }
}
