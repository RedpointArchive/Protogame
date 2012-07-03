using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Protogame;
using Protogame.Tiles;
using Protogame.MultiLevel;
using Protogame.RTS;
using Protogame.RTS.Multiplayer;
using Protogame.RTS.Spawners;
#if MULTIPLAYER
using Process4;
using Process4.Collections;
#endif

namespace Protogame.RTS
{
    public abstract class RTSWorld : MultiLevelWorld
    {
        private List<Team> m_Teams = new List<Team>();
        private UiManager m_UiManager = new UiManager();
        private GlobalSession m_GlobalSession = null;
        private Player m_LocalPlayer = null;

        public RTSWorld()
            : base()
        {
            // Add default, neutral team.
            Team neutral = new Team(0, "Neutral", Color.LightGray);
            this.Teams.Add(neutral);
            
#if MULTIPLAYER
            // Get global session information.
            this.m_GlobalSession = new Distributed<GlobalSession>("rts-session");
            this.m_GlobalSession.LogEmitted += (sender, ev) => { this.UiManager.Log(ev.ToString()); };
            this.m_LocalPlayer = this.m_GlobalSession.Join();

            // Hook the game started event.
            if (LocalNode.Singleton.IsServer)
                this.m_GlobalSession.ServerGameStarted += this._HandleServerGameStart;
            else
                this.m_GlobalSession.ClientGameStarted += this._HandleClientGameStart;
#else
            // Get global session information.
            this.m_GlobalSession = new GlobalSession();
            this.m_GlobalSession.LogEmitted += (sender, ev) => { this.UiManager.Log(ev.ToString()); };
            this.m_LocalPlayer = this.m_GlobalSession.Join();

            // Hook the game started event.
            this.m_GlobalSession.ServerGameStarted += (sender, ev) => { this.HandleServerGameStart(); };
#endif
        }

#if MULTIPLAYER
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void _HandleServerGameStart(object sender, EventArgs e)
        {
            this.HandleServerGameStart();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected void _HandleClientGameStart(object sender, EventArgs e)
        {
            this.HandleClientGameStart();
        }

        ~RTSWorld()
        {
            // Attempt to leave the session.
            try
            {
                this.m_GlobalSession.Leave(this.m_LocalPlayer);
            }
            catch { }
        }
#endif

        #region Game Start Handlers

        private bool m_NeedsToInitialize = false;

        private void HandleServerGameStart()
        {
            this.m_NeedsToInitialize = true;
        }

        private void HandleClientGameStart()
        {
            this.m_NeedsToInitialize = true;
        }

        private void HandleTeamAllocations()
        {
            Stack<Color> colors = new Stack<Color>(new Color[] {
                Color.Blue,
                Color.Red,
                Color.Yellow,
                Color.Green,
                Color.Gray,
                Color.Purple,
                Color.Violet,
                Color.Cyan
            });

            // Assign teams.
            foreach (Player p in this.m_GlobalSession.Players)
            {
                Team t = new Team(p.PlayerID, "Player " + p.PlayerID, colors.Pop());
                this.m_Teams.Add(t);
            }
        }

        private void HandleUnitAllocations()
        {
            // Assign units to have player IDs using the grouping information.
            int pidx = 0;
            long uidx = 0;
            bool ranOut = false;
            for (int i = 0; i <= 100; i++)
            {
                bool usedActivePlayer = false;

                // Get all unit spawners with the specified unit ID.
                foreach (Tile t in this.Tileset.AsLinear())
                {
                    // If this tile is a TeamUnitSpawner...
                    if (t is TeamUnitSpawner)
                    {
                        TeamUnitSpawner s = t as TeamUnitSpawner;

                        // If the grouping ID is the same..
                        if (s.GroupingID == i)
                        {
                            // If we have run out of players to assign..
                            if (ranOut)
                            {
                                // Destroy active unit.
                                s.DestroyUnit();
                            }
                            else
                            {
                                // Assign player ID.
                                s.PlayerID = this.m_GlobalSession.Players[pidx].PlayerID;

                                // Assign unit synchronisation ID.
                                s.SynchronisationName = "starting-unit-" + (uidx++);

                                // Mark the player ID as used.
                                usedActivePlayer = true;
                            }
                        }
                    }
                }

                // Now increment the player index if the player was used.
                if (usedActivePlayer)
                    pidx++;

                // Check to see if we have any more players.
                if (pidx >= this.m_GlobalSession.Players.Count)
                    ranOut = true;
            }
        }

        private void HandleUnitNeutrals()
        {
            // Assign neutral units to be spawned correctly.
            long uidx = 0;

            // Get all unit spawners with the specified unit ID.
            foreach (Tile t in this.Tileset.AsLinear())
            {
                // If this tile is a TeamUnitSpawner...
                if (t is NeutralUnitSpawner)
                {
                    NeutralUnitSpawner s = t as NeutralUnitSpawner;

                    // Assign unit synchronisation ID.
                    s.SynchronisationName = "neutral-unit-" + (uidx++);
                }
            }
        }

        #endregion

        public Player LocalPlayer
        {
            get
            {
                return this.m_LocalPlayer;
            }
        }

        public override List<IEntity> Entities
        {
            get
            {
                List<IEntity> all = new List<IEntity>();
                foreach (Level l in this.m_Levels)
                    foreach (IEntity e in l.Entities)
                        all.Add(e);
                return all;
            }
        }

        public override Tileset Tileset
        {
            get
            {
                if (this.m_ActiveLevel == null)
                    throw new InvalidOperationException();
                return this.m_Tilesets[Array.IndexOf(this.m_Levels, this.m_ActiveLevel)];
            }
            protected set
            {
                if (value == null)
                    return;
                if (this.m_ActiveLevel == null)
                    throw new InvalidOperationException();
                this.m_Tilesets[Array.IndexOf(this.m_Levels, this.m_ActiveLevel)] = value;
            }
        }

        public List<Team> Teams
        {
            get
            {
                return this.m_Teams;
            }
        }

        public UiManager UiManager
        {
            get
            {
                return this.m_UiManager;
            }
        }

        public GlobalSession GlobalSession
        {
            get
            {
                return this.m_GlobalSession;
            }
        }

        public override void DrawBelow(GameContext context)
        {
            // Are we waiting for players?
#if MULTIPLAYER
            if (this.m_GlobalSession.Waiting || (this.m_GlobalSession.LoadingInitialData && !LocalNode.Singleton.IsServer))
#else
            if (this.m_GlobalSession.Waiting)
#endif
                return;

            // Handle game normally.
            if (this.m_ActiveLevel != null)
                this.m_ActiveLevel.DrawBelow(context);
            else
            {
                context.Graphics.GraphicsDevice.Clear(Color.Black);
                new XnaGraphics(context).DrawStringCentered(context.Graphics.GraphicsDevice.Viewport.Width / 2, 128, "No active level.");
            }
        }

        public override void DrawAbove(GameContext context)
        {
            // Are we waiting for players?
#if MULTIPLAYER
            if (this.m_GlobalSession.Waiting || (this.m_GlobalSession.LoadingInitialData && !LocalNode.Singleton.IsServer))
#else
            if (this.m_GlobalSession.Waiting)
#endif
            {
                XnaGraphics graphics = new XnaGraphics(context);
                graphics.FillRectangle(new Rectangle(0, 0, context.Graphics.GraphicsDevice.Viewport.Width, context.Graphics.GraphicsDevice.Viewport.Height), Color.Black);
                int dialogX = context.Graphics.GraphicsDevice.Viewport.X + context.Graphics.GraphicsDevice.Viewport.Width / 2 - 300;
                int dialogY = context.Graphics.GraphicsDevice.Viewport.Y + context.Graphics.GraphicsDevice.Viewport.Height / 2 - 200;
                graphics.FillRectangle(new Rectangle(dialogX, dialogY, 600, 400), Color.DarkSlateGray);
                graphics.DrawStringCentered(dialogX + 300, dialogY + 8, this.m_GlobalSession.LobbyMessage, "BigArial");
                graphics.DrawStringCentered(dialogX + 300, dialogY + 32, "Press enter to toggle ready status.");
                int a = 0;
                for (int i = 0; i < this.m_GlobalSession.Players.Count; i++)
                {
                    Player p = this.m_GlobalSession.Players[i];
                    if (p.Ready)
                        graphics.DrawStringLeft(dialogX + 8, dialogY + 48 + a * 16, "Player " + a + " (" + p.PlayerID + ") ready.");
                    else
                        graphics.DrawStringLeft(dialogX + 8, dialogY + 48 + a * 16, "Player " + a + " (" + p.PlayerID + ") not ready.");
                    a++;
                }
                return;
            }

            // Handle game normally.
            if (this.m_ActiveLevel != null)
                this.m_ActiveLevel.DrawAbove(context);
            this.m_UiManager.Draw(this, context, new XnaGraphics(context));
        }

        private bool m_DidJustPressEnter = false;
        private bool m_ServerHasRunOneTick = false;

        public override bool Update(GameContext context)
        {
            KeyboardState keystate = Keyboard.GetState();

            // Are we waiting for players?
            if (this.m_NeedsToInitialize)
            {
                this.HandleTeamAllocations();
                this.HandleUnitAllocations();
                this.HandleUnitNeutrals();
                this.m_NeedsToInitialize = false;
                return false;
            }
            else if (this.m_GlobalSession.Waiting)
            {
                // Check for ready toggle.
                if (keystate.IsKeyDown(Keys.Enter))
                    this.m_DidJustPressEnter = true;
                else if (keystate.IsKeyUp(Keys.Enter) && this.m_DidJustPressEnter)
                {
                    this.m_LocalPlayer.MarkReady(!this.m_LocalPlayer.Ready);
                    this.m_DidJustPressEnter = false;
                }

                // Update global state if permitted.
                this.m_GlobalSession.Update(context.GameTime);

                return false;
            }
#if MULTIPLAYER
            else if (this.m_GlobalSession.LoadingInitialData && !LocalNode.Singleton.IsServer)
            {
                // Can't do anything here.
                return false;
            }
#endif

            // Handle game normally.
            if (this.m_ActiveLevel != null)
                this.m_ActiveLevel.Update(context);

            if (keystate.IsKeyDown(Keys.Q))
                this.m_ActiveLevel = this.m_Levels[0];
            if (keystate.IsKeyDown(Keys.W))
                this.m_ActiveLevel = this.m_Levels[1];
            if (keystate.IsKeyDown(Keys.E))
                this.m_ActiveLevel = this.m_Levels[2];
            if (keystate.IsKeyDown(Keys.R))
                this.m_ActiveLevel = this.m_Levels[3];
            if (keystate.IsKeyDown(Keys.T))
                this.m_ActiveLevel = this.m_Levels[4];
            if (keystate.IsKeyDown(Keys.Q) ||
                keystate.IsKeyDown(Keys.W) ||
                keystate.IsKeyDown(Keys.E) ||
                keystate.IsKeyDown(Keys.R) ||
                keystate.IsKeyDown(Keys.T))
            {
                // reset selection.
                this.m_UiManager.Selected.Clear();
            }

            // The server is now ready, tell all clients to start loading.
#if MULTIPLAYER
            if (this.m_GlobalSession.LoadingInitialData && LocalNode.Singleton.IsServer)
#else
            if (this.m_GlobalSession.LoadingInitialData)
#endif
            {
                if (!this.m_ServerHasRunOneTick)
                    this.m_ServerHasRunOneTick = true;
                else
                    this.m_GlobalSession.ServerReady();
            }

            return true;
        }
    }
}
