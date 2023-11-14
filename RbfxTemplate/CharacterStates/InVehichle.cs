using Urho3DNet;
using static RbfxTemplate.Character;

namespace RbfxTemplate.CharacterStates
{
    public class InVehicle : BaseState
    {
        private Vehicle _vehicle;
        private RaycastVehicle _raycastVehicle;

        public InVehicle(Character character) : base(character)
        {
        }

        /// <inheritdoc/>
        public override void Enter(object argument)
        {
            base.Enter(argument);

            _vehicle = argument as Vehicle;
            if (_vehicle == null)
            {
                Character.TransitionToState(CharacterState.OnGround);
                return;
            }

            _raycastVehicle = _vehicle.GetComponent<RaycastVehicle>();

            var animationParameters = new AnimationParameters(Character.Drive).Looped();
            Character.AnimationController.PlayNewExclusive(animationParameters, 0.2f);
            Character.SetPhysicsEnabled(false);
            _vehicle.Node.AddChild(Character.Node);
            Character.Node.Position = Vector3.Zero;
            Character.Node.Rotation = Quaternion.IDENTITY;
        }

        /// <inheritdoc/>
        public override void Exit()
        {
            var pos = Character.Node.WorldPosition;
            _vehicle.Scene.AddChild(Character.Node);
            Character.Node.Rotation = Quaternion.IDENTITY;
            Character.Node.Position = pos;
            Character.SetPhysicsEnabled(true);
            if (_raycastVehicle != null)
            {
                _raycastVehicle.UpdateInput(0.0f, 0.0f, 1.0f);
            }
            base.Exit();
        }

        /// <inheritdoc/>
        public override void Update(Character.Inputs inputs)
        {
            //_raycastVehicle.GetComponent<RigidBody>().Activate();
            _raycastVehicle.UpdateInput(inputs.InputVelocity.X, inputs.InputVelocity.Z, 0.0f);
            inputs.CurrentVelocity = Vector3.Zero;
        }
    }
}