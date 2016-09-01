namespace Protogame
{
    using System.Collections.Generic;
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
        private readonly IModel _model;

        /// <summary>
        /// The script instance driving the animations.
        /// </summary>
        private readonly ScriptAssetInstance _scriptInstance;

        /// <summary>
        /// The current animation being played.
        /// </summary>
        private string _currentState;

        /// <summary>
        /// The requested animation via <see cref="Request"/>.
        /// </summary>
        private string _targetState;

        /// <summary>
        /// The current frame number for the animation.
        /// </summary>
        private double _tickNumber;

        /// <summary>
        /// Whether the animation is changing on the next frame.
        /// </summary>
        private bool _changeOnNextFrame;

        /// <summary>
        /// Initializes a new instance of the <see cref="AnimationController"/> class.
        /// </summary>
        /// <param name="model">
        /// The model whose animations will be played.
        /// </param>
        /// <param name="script">
        /// The script instance.
        /// </param>
        public AnimationController(IModel model, ScriptAsset script)
        {
            _model = model;
            _scriptInstance = script.CreateInstance();
        }

        /// <summary>
        /// Gets the current animation being played.
        /// </summary>
        /// <value>
        /// The current animation being played.
        /// </value>
        public string CurrentAnimation => _currentState;

        /// <summary>
        /// Gets the current frame to render.
        /// </summary>
        /// <value>
        /// The current frame to render.
        /// </value>
        public double Frame => _tickNumber;

        /// <summary>
        /// Gets or sets a string representing a custom effect that should be applied to rendering.
        /// </summary>
        /// <value>
        /// The custom effect that should be applied to rendering.
        /// </value>
        public string Effect
        {
            get; 
            set;
        }

        /// <summary>
        /// Requests that the animation controller change to the specified animation.
        /// </summary>
        /// <param name="animationName">
        /// The new animation to play.
        /// </param>
        public void Request(string animationName)
        {
            _targetState = animationName;
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
            if (_currentState == null)
            {
                if (_targetState != null)
                {
                    _currentState = _targetState;
                }
                else
                {
                    // No current or requested animation, so do nothing.
                    return;
                }
            }

            _tickNumber += gameTime.ElapsedGameTime.TotalSeconds
                                 * _model.AvailableAnimations[_currentState].TicksPerSecond * multiply;

            var results = _scriptInstance.Execute(
                _model.AvailableAnimations[_currentState].Name, 
                new Dictionary<string, object>
                {
                    { "IN_CurrentAnim", _currentState }, 
                    { "IN_RequestedAnim", _targetState }, 
                    { "IN_Tick", _tickNumber }, 
                    { "IN_LastTick", _model.AvailableAnimations[_currentState].DurationInTicks - 1 },
                    { "IN_JustChanged", _changeOnNextFrame ? 1 : 0 }
                });

            _changeOnNextFrame = false;

            if (results.ContainsKey("OUT_Anim"))
            {
                if (_currentState != (string)results["OUT_Anim"])
                {
                    _changeOnNextFrame = true;
                }

                _currentState = (string)results["OUT_Anim"];
            }

            if (results.ContainsKey("OUT_Tick"))
            {
                _tickNumber = (float)results["OUT_Tick"];
            }

            if (results.ContainsKey("OUT_Effect"))
            {
                Effect = (string)results["OUT_Effect"];
            }
        }
    }
}