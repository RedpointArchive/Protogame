namespace Protogame
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The input prediction.
    /// </summary>
    public class InputPrediction
    {
        /// <summary>
        /// The replay stack of actions.
        /// </summary>
        private readonly List<Action> m_Replays;

        /// <summary>
        /// The current counter of actions pushed into the input prediction class.
        /// </summary>
        private int m_CurrentCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="InputPrediction"/> class.
        /// </summary>
        public InputPrediction()
        {
            this.m_Replays = new List<Action>();
            this.m_CurrentCounter = 0;
        }

        /// <summary>
        /// Mark the specified input as acknowledged, removing all actions in the prediction queue
        /// prior to this point in time.
        /// </summary>
        /// <param name="count">
        /// The counter originally given by Predict.
        /// </param>
        public void Acknowledge(int count)
        {
            var remaining = this.m_CurrentCounter - count;
            if (remaining < 0)
            {
                this.m_CurrentCounter = count;
                this.m_Replays.Clear();
                return;
            }

            var amountToRemove = this.m_Replays.Count - remaining;
            if (amountToRemove < 0)
            {
                throw new InvalidOperationException(
                    "An invalid count was passed; ensure that you are checking"
                    + " that the acknowledgement count from the server is higher"
                    + " than the current count when receiving it.");
            }

            this.m_Replays.RemoveRange(0, amountToRemove);
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
            this.m_Replays.Add(action);
            return this.m_CurrentCounter++;
        }

        /// <summary>
        /// Replays the list of actions that are currently not acknowledged by the server.
        /// </summary>
        public void Replay()
        {
            foreach (var action in this.m_Replays)
            {
                action();
            }
        }
    }
}