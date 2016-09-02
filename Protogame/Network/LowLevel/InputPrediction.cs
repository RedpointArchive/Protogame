namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The input prediction.
    /// </summary>
    /// <module>Network</module>
    public class InputPrediction
    {
        /// <summary>
        /// The replay stack of actions.
        /// </summary>
        private readonly List<Action> _replays;

        /// <summary>
        /// The current counter of actions pushed into the input prediction class.
        /// </summary>
        private int _currentCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputPrediction"/> class.
        /// </summary>
        public InputPrediction()
        {
            _replays = new List<Action>();
            _currentCounter = 0;
        }

        public int ReplayCount => _replays.Count;

        /// <summary>
        /// Mark the specified input as acknowledged, removing all actions in the prediction queue
        /// prior to this point in time.
        /// </summary>
        /// <param name="count">
        /// The counter originally given by Predict.
        /// </param>
        public void Acknowledge(int count)
        {
            var remaining = _currentCounter - count;
            if (remaining < 0)
            {
                _currentCounter = count;
                _replays.Clear();
                return;
            }

            var amountToRemove = _replays.Count - remaining;
            if (amountToRemove < 0)
            {
                throw new InvalidOperationException(
                    "An invalid count was passed; ensure that you are checking"
                    + " that the acknowledgement count from the server is higher"
                    + " than the current count when receiving it.");
            }

            _replays.RemoveRange(0, amountToRemove);
        }

        /// <summary>
        /// Place an action in the input prediction queue, and return the counter that the server
        /// needs to acknowledge.
        /// </summary>
        /// <param name="action">
        /// The prediction action.
        /// </param>
        /// <returns>
        /// The counter that should be sent to the server for acknowledgement.
        /// </returns>
        public int Predict(Action action)
        {
            _replays.Add(action);
            return ++_currentCounter;
        }

        /// <summary>
        /// Replays the list of actions that are currently not acknowledged by the server.
        /// </summary>
        public void Replay()
        {
            foreach (var action in _replays)
            {
                action();
            }
        }
    }
}