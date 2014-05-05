// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
namespace Protogame
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Xna.Framework;

    /// <summary>
    /// An animation controller which uses a <see cref="ScriptAsset"/> to drive transitions
    /// between animations.
    /// </summary>
    public class AnimationController
    {
        /// <summary>
        /// The model whose animations are being controlled.
        /// </summary>
        private readonly ModelAsset m_Model;

        /// <summary>
        /// The script instance driving the animations.
        /// </summary>
        private readonly ScriptAssetInstance m_ScriptInstance;

        /// <summary>
        /// The current animation being played.
        /// </summary>
        private string m_CurrentState;

        /// <summary>
        /// The requested animation via <see cref="Request"/>.
        /// </summary>
        private string m_TargetState;

        /// <summary>
        /// The current frame number for the animation.
        /// </summary>
        private double m_TickNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationController"/> class.
        /// </summary>
        /// <param name="model">
        /// The model whose animations will be played.
        /// </param>
        /// <param name="script">
        /// The script instance.
        /// </param>
        public AnimationController(ModelAsset model, ScriptAsset script)
        {
            this.m_Model = model;
            this.m_ScriptInstance = script.CreateInstance();
        }

        /// <summary>
        /// Gets the current animation being played.
        /// </summary>
        /// <value>
        /// The current animation being played.
        /// </value>
        public string CurrentAnimation
        {
            get
            {
                return this.m_CurrentState;
            }
        }

        /// <summary>
        /// Gets the current frame to render.
        /// </summary>
        /// <value>
        /// The current frame to render.
        /// </value>
        public int Frame
        {
            get
            {
                return (int)this.m_TickNumber;
            }
        }

        /// <summary>
        /// Requests that the animation controller change to the specified animation.
        /// </summary>
        /// <param name="animationName">
        /// The new animation to play.
        /// </param>
        public void Request(string animationName)
        {
            this.m_TargetState = animationName;
        }

        /// <summary>
        /// Updates the animation controller, using the associated script to drive animation changes.
        /// </summary>
        /// <param name="gameTime">
        /// The delta game time.
        /// </param>
        /// <param name="multiply">
        /// The animation multiplier to apply.
        /// </param>
        public void Update(GameTime gameTime, float multiply = 1)
        {
            if (this.m_CurrentState == null)
            {
                if (this.m_TargetState != null)
                {
                    this.m_CurrentState = this.m_TargetState;
                }
                else
                {
                    // No current or requested animation, so do nothing.
                    return;
                }
            }

            this.m_TickNumber += gameTime.ElapsedGameTime.TotalSeconds
                                 * this.m_Model.AvailableAnimations[this.m_CurrentState].TicksPerSecond * multiply;

            var results = this.m_ScriptInstance.Execute(
                this.m_Model.AvailableAnimations[this.m_CurrentState].Name, 
                new Dictionary<string, object>
                {
                    { "IN_CurrentAnim", this.m_CurrentState }, 
                    { "IN_RequestedAnim", this.m_TargetState }, 
                    { "IN_Tick", this.m_TickNumber }, 
                    { "IN_LastTick", this.m_Model.AvailableAnimations[this.m_CurrentState].DurationInTicks - 1 }
                });

            if (results.ContainsKey("OUT_Anim"))
            {
                this.m_CurrentState = (string)results["OUT_Anim"];
            }

            if (results.ContainsKey("OUT_Tick"))
            {
                this.m_TickNumber = (float)results["OUT_Tick"];
            }
        }
    }
}