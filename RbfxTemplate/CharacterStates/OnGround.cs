using Urho3DNet;

namespace RbfxTemplate.CharacterStates
{
    public class OnGround : BaseState
    {
        /// How much time character could be in air before it considers to be falling down.
        private const float InairThresholdTime = 0.1f;

        /// In air timer. Due to possible physics inaccuracy, character can be off ground for max. 1/10 second and still be allowed to move.
        private float _inAirTimer;

        /// Movement speed from animation.
        private float _speed;

        /// Current animation.
        private Animation _currentAnimation;

        /// Timeout in seconds before jump allowed.
        private float _jumpTimeout;

        public OnGround(Character character) : base(character)
        {
        }

        /// <inheritdoc />
        public override void Enter()
        {
            _inAirTimer = 0.0f;
            _currentAnimation = null;
            _jumpTimeout = 0.2f;
        }

        /// <inheritdoc />
        public override void Update(ref Character.Inputs inputs)
        {
            if (!Character.CharacterController.OnGround())
                _inAirTimer += inputs.TimeStep;
            else
                _inAirTimer = 0;

            var softGrounded = _inAirTimer < InairThresholdTime;
            if (!softGrounded)
            {
                Character.TransitionToState(CharacterState.Fall, ref inputs);
                return;
            }

            Animation nextAnimation = null;
            if (inputs.InputSpeed < 0.3f)
            {
                nextAnimation = Character.Idle;
                _speed = 0.0f;
            }
            else if (inputs.InputSpeed > 0.7f)
            {
                nextAnimation = Character.Run;
            }
            else
            {
                nextAnimation = Character.Walk;
            }

            if (nextAnimation != _currentAnimation)
            {
                var animationParameters = new AnimationParameters(nextAnimation).Looped();
                Character.AnimationController.PlayNewExclusive(animationParameters, 0.2f);
                _currentAnimation = nextAnimation;
                _speed = _currentAnimation.GetMetadata("LinearVelocity").Vector3.Length;
            }

            inputs.CurrentVelocity = new Quaternion(0, Character.GetYaw(), 0) * (inputs.InputDirection * _speed);

            _jumpTimeout -= inputs.TimeStep;
            if (inputs.Jump && _jumpTimeout <= 0) Character.TransitionToState(CharacterState.Jump);
        }
    }
}