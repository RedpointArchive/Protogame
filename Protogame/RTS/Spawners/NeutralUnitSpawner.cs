using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Protogame;
using System.Reflection;
using Protogame.MultiLevel;

namespace Protogame.RTS.Spawners
{
    [TileName("NeutralUnitSpawner", TilesetXmlLoader.XmlLoadMode.Entities)]
    public class NeutralUnitSpawner : MultiLevelEntityTile
    {
        private bool m_HasSpawned = false;
        private string m_UnitName = null;
        private int m_PlayerID = 0;
        private string m_SynchronisationName = null;
        private Dictionary<string, string> m_Attributes;
        private Unit m_Unit = null;

        public NeutralUnitSpawner(Dictionary<string, string> attributes)
        {
            this.Image = null;
            this.X = Convert.ToInt32(attributes["x"]);
            this.Y = Convert.ToInt32(attributes["y"]);
            this.m_UnitName = attributes["Unit"];
            this.m_Attributes = attributes;
            this.Width = 32;
            this.Height = 32;
        }

        public override void Update(World rawWorld)
        {
            RTSWorld world = rawWorld as RTSWorld;

            if (!this.m_HasSpawned)
            {
                // Set to neutral.
                Team teamInstance = world.Teams.DefaultIfEmpty(null).First(v => (v.Name == "Neutral"));
                if (teamInstance == null)
                    throw new ProtogameException("Unable to find neutral team instance.");

                // Find unit factory.
                foreach (Type t in AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()))
                {
                    if (typeof(IUnitFactory).IsAssignableFrom(t) && !t.IsInterface)
                    {
                        IUnitFactory factory = t.GetConstructor(Type.EmptyTypes).Invoke(null) as IUnitFactory;
                        if (factory.CanCreate(this.m_UnitName))
                        {
                            this.m_Unit = factory.Create(world, this.Level, teamInstance, this.m_UnitName, this.m_Attributes);
                            this.m_Unit.X = this.X;
                            this.m_Unit.Y = this.Y;
                            this.Level.Entities.Add(this.m_Unit);
                            this.m_HasSpawned = true;
                            if (this.m_SynchronisationName != null)
                            {
                                this.m_Unit.SetSynchronisationName(this.m_SynchronisationName);
                                this.m_Unit.ForceSynchronisation();
                            }
                            return;
                        }
                    }
                }

                throw new ProtogameException("Unable to find unit factory that handles unit name '" + this.m_UnitName + "'.");
            }

            base.Update(world);
        }

        public string SynchronisationName
        {
            set
            {
                // Make sure unit is spawned.
                if (this.m_Unit == null)
                    this.m_SynchronisationName = value;
                else
                {
                    this.m_Unit.SetSynchronisationName(value);
                    this.m_Unit.ForceSynchronisation();
                }
            }
        }

        internal void DestroyUnit()
        {
            // Make sure unit is spawned.
            if (this.m_Unit == null)
            {
                this.m_HasSpawned = true;
                return;
            }

            this.Level.Entities.Remove(this.m_Unit);
        }
    }
}
