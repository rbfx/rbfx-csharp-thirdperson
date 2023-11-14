using System;
using Urho3DNet;

namespace RbfxTemplate.CharacterStates
{
    public class Fall : BaseState
    {
        private bool firstFrame_;
        private float speedScale_;

        public Fall(Character character) : base(character)
        {
        }

        public override void Enter(object argument)
        {
            firstFrame_ = true;
            var animationParameters = new AnimationParameters(Character.Falling).Looped();
            Character.AnimationController.PlayNewExclusive(animationParameters, 0.2f);
        }

        public override void Update(Character.Inputs inputs)
        {
            if (Character.CharacterController.OnGround())
            {
                Character.TransitionToState(CharacterState.OnGround, ref inputs);
                return;
            }

            if (firstFrame_)
            {
                if (inputs.InputSpeed < 1e-2f)
                {
                    speedScale_ = Character.Walk.GetMetadata("LinearVelocity").Vector3.Length;
                }
                else
                {
                    speedScale_ = inputs.CurrentVelocity.Length / Math.Min(1.0f, inputs.InputSpeed);
                }
                firstFrame_ = false;
            }

            var velocity = new Quaternion(0, Character.GetYaw(), 0) *
                           (inputs.InputDirection * speedScale_ * Math.Min(1.0f, inputs.InputSpeed));
            inputs.CurrentVelocity = inputs.CurrentVelocity.Lerp(velocity, 4.0f * inputs.TimeStep);
        }
    }
}