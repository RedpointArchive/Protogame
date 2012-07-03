using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#if MULTIPLAYER
using Process4.Attributes;
using Process4.Collections;
#endif

namespace Protogame.RTS.Multiplayer
{
#if MULTIPLAYER
    [Distributed]
#endif
    public class StringEventArgs : EventArgs
    {
        public string Message { get; private set; }

        public StringEventArgs(string msg)
        {
            this.Message = msg;
        }
    }

#if MULTIPLAYER
    [Distributed]
#endif
    public class GlobalSession
    {
        public event EventHandler LogEmitted;
        public event EventHandler ServerGameStarted;
        public event EventHandler ClientGameStarted;

        public GlobalSession()
        {
            this.Waiting = true;
            this.PlayersNotAllReady = true;
            this.StartCounter = 5;
            this.Name = "Random RTS Game!";
#if MULTIPLAYER
            this.Players = new DList<Player>();
#else
            this.Players = new List<Player>();
#endif
        }

        public bool Waiting
        {
            get;
            set;
        }

        public bool PlayersNotAllReady
        {
            get;
            set;
        }

        public int StartCounter
        {
            get;
            private set;
        }

        public bool LoadingInitialData
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }
        
#if MULTIPLAYER
        public DList<Player> Players
#else
        public List<Player> Players
#endif
        {
            get;
            set;
        }

        public string LobbyMessage
        {
            get
            {
                if (this.LoadingInitialData)
                    return "Server is initializing";
                else if (this.PlayersNotAllReady)
                    return "Waiting for players";
                else
                    return "Game starts in " + this.StartCounter + ".";
            }
        }

        private double m_LastTimeCounter
        {
            get;
            set;
        }

#if MULTIPLAYER
        [ClientIgnorable]
#endif
        public void Update(GameTime time)
        {
            // Set start counter if not all players are ready.
            // FIXME: Speed this up!
           /* if (this.PlayersNotAllReady)
            {
                if (this.Players.Count == 1)
                    this.StartCounter = 0;
                else
                    this.StartCounter = 5;
            }*/

            // Checks to see if (a) we have more than zero players,
            // (b) all players are ready.
            if (this.StartCounter >= 0)
            {
                if (this.Players.Count > 0)
                {
                    bool allReady = true;
                    foreach (Player p in this.Players)
                    {
                        if (!p.Ready)
                        {
                            allReady = false;
                            break;
                        }
                    }

                    // Depending on result, toggle waiting status.
                    if (this.PlayersNotAllReady && allReady)
                    {
                        this.PlayersNotAllReady = false;
                    }
                    else if (!this.PlayersNotAllReady && !allReady)
                    {
                        this.PlayersNotAllReady = true;
                        this.m_LastTimeCounter = 0;
                    }
                    else if (!this.PlayersNotAllReady)
                    {
                        if (this.m_LastTimeCounter == 0)
                            this.m_LastTimeCounter = time.TotalGameTime.TotalMilliseconds;
                        if (time.TotalGameTime.TotalMilliseconds - this.m_LastTimeCounter > 1000)
                        {
                            this.StartCounter -= (int)((time.TotalGameTime.TotalMilliseconds - this.m_LastTimeCounter) / 1000f);
                            this.m_LastTimeCounter = time.TotalGameTime.TotalMilliseconds;
                        }
                        if (this.StartCounter <= 0)
                        {
                            this.Waiting = false;
                            this.LoadingInitialData = true;
                            if (this.ServerGameStarted != null)
                                this.ServerGameStarted(this, new EventArgs());
                        }
                    }
                }
            }
        }

        public void ServerReady()
        {
            if (this.ClientGameStarted != null)
                this.ClientGameStarted(this, new EventArgs());
            this.LoadingInitialData = false;
        }

#if MULTIPLAYER
        [ClientCallable]
#endif
        public Player Join()
        {
            Player player = new Player();
            this.Players.Add(player);
            return player;
        }

#if MULTIPLAYER
        [ClientCallable]
#endif
        public void Leave(Player player)
        {
            this.Players.Remove(player);
        }

        public void Log(string msg)
        {
            if (this.LogEmitted != null)
                this.LogEmitted(this, new StringEventArgs(msg));
        }
    }
}
