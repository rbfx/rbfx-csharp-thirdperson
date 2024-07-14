using System;
using System.Diagnostics;
using Urho3DNet;
using RbfxTemplate.CharacterStates;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public partial class Character : MoveAndOrbitComponent
    {
        /// Character states (on ground, falling, etc).
        private readonly BaseState[] _states;

        /// Cached structure to capture raycast results.
        private readonly PhysicsRaycastResult _physicsRaycastResult = new PhysicsRaycastResult();

        /// Character state input.
        private Inputs _inputs = new Inputs();

        /// Kinematic character controller.
        private KinematicCharacterController _characterController;

        /// <summary>
        /// Current active character state.
        /// </summary>
        private BaseState _currentState;

        /// <summary>
        /// Index of the current state.
        /// </summary>
        private CharacterState _state = CharacterState.NumStates;

        /// <summary>
        /// Construct Character component.
        /// </summary>
        /// <param name="context">Game engine context.</param>
        public Character(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate;
            _states = new BaseState[(int)CharacterState.NumStates]
            {
                new StartState(this),
                new OnGround(this),
                new JumpState(this),
                new Fall(this),
                new InVehicle(this),
            };
            TransitionToState(CharacterState.Start, null);
        }

        public KinematicCharacterController CharacterController
        {
            get => _characterController;
            set
            {
                if (_characterController != value)
                {
                    _characterController = value;
                    UpdateHeight();
                }
            }
        }

        public AnimationController AnimationController { get; set; }
        public Node ModelPivot { get; set; }
        public Node CameraYaw { get; set; }
        public Node CameraPitch { get; set; }
        public Animation Idle { get; set; }
        public Animation Walk { get; set; }
        public Animation Drive { get; set; }
        public Node CameraNode { get; set; }

        public float CameraDistance { get; set; } = 1.4f;

        public bool Crouch
        {
            get => _inputs.Crouch;
            set
            {
                if (_inputs.Crouch != value)
                {
                    _inputs.Crouch = value;
                    UpdateHeight();
                }
            }
        }

        public uint CameraCollisionMask { get; set; } = uint.MaxValue;
        public Animation Run { get; set; }
        public Animation Falling { get; set; }

        public bool Jump
        {
            get => _inputs.Jump;
            set => _inputs.Jump = value;
        }

        /// <summary>
        /// Current character state index.
        /// </summary>
        public CharacterState State => _state;

        /// <summary>
        /// Update character setup.
        /// </summary>
        /// <param name="timeStep">Time step.</param>
        public override void Update(float timeStep)
        {
            // Update camera position and rotation.
            ApplyRotation();

            // Update character state.
            _inputs.TimeStep = timeStep;
            _inputs.InputVelocity = Velocity;
            _inputs.InputSpeed = _inputs.InputVelocity.Length;
            if (_inputs.InputSpeed < 1e-3f)
            {
                _inputs.InputSpeed = 0;
                _inputs.InputDirection = Vector3.Zero;
            }
            else
            {
                _inputs.InputDirection = _inputs.InputVelocity / _inputs.InputSpeed;
            }

            _currentState.Update(_inputs);

            // Move character based on evaluated linear velocity.
            CharacterController.SetWalkIncrement(_inputs.CurrentVelocity * timeStep);

            // Rotate character model with smoothing.
            RotateModel(timeStep);

            base.Update(timeStep);
        }

        /// <summary>
        /// Transition to the character state with user or AI input.
        /// </summary>
        /// <param name="state">Target character state.</param>
        /// <param name="inputs">User or AI inputs.</param>
        public void TransitionToState(CharacterState state, ref Inputs inputs)
        {
            var targetState = _states[(int)state];
            if (_currentState != targetState)
            {
                // Notify previous state about transition.
                if (_currentState != null)
                    _currentState.Exit();

                // Update current state.
                _currentState = targetState;
                _state = state;

                // Notify new state about transition.
                if (_currentState != null)
                {
                    _currentState.Enter(null);
                    // Allow state to update.
                    _currentState.Update(inputs);
                }
            }
        }

        /// <summary>
        /// Transition to the character state.
        /// </summary>
        /// <param name="state">Target character state.</param>
        public void TransitionToState(CharacterState state, object argument = null)
        {
            var targetState = _states[(int)state];
            if (_currentState != targetState)
            {
                // Notify previous state about transition.
                if (_currentState != null)
                    _currentState.Exit();

                // Update current state.
                _currentState = targetState;
                _state = state;

                // Notify new state about transition.
                if (_currentState != null)
                    _currentState.Enter(argument);
            }
        }

        /// <summary>
        /// Update character state (stand or crouch).
        /// </summary>
        private void UpdateHeight()
        {
            if (CharacterController != null)
            {
                if (Crouch)
                {
                    CharacterController.Height = 0.9f;
                    CharacterController.Offset = new Vector3(0.0f, 0.45f);
                }
                else
                {
                    CharacterController.Height = 1.8f;
                    CharacterController.Offset = new Vector3(0.0f, 0.9f);
                }
            }
        }

        /// <summary>
        /// Evaluate smooth model rotation.
        /// Character model rotated independently from collision capsule.
        /// </summary>
        /// <param name="timeStep">Time step to limit angular speed of model rotation.</param>
        private void RotateModel(float timeStep)
        {
            if (_inputs.CurrentVelocity.LengthSquared > 1e-2f)
            {
                //Limit rotation to avoid instant change of model orientation.
                var targetRot = new Quaternion(Vector3.Forward, _inputs.CurrentVelocity);
                var currentRot = ModelPivot.Rotation;
                var diff = Math.Abs(MathDefs.RadiansToDegrees((currentRot.Inversed * targetRot).Angle));
                diff = Math.Min(diff, 360 - diff);
                var maxAngle = 2.0f * 360.0f * timeStep;
                if (diff > maxAngle)
                {
                    var k = maxAngle / Math.Abs(diff);
                    targetRot = currentRot.Slerp(targetRot, k);
                }

                ModelPivot.Rotation = targetRot;
            }
        }

        /// <summary>
        /// Rotate camera node.
        /// </summary>
        private void ApplyRotation()
        {
            if (CameraPitch != null)
                CameraPitch.Rotation = new Quaternion(new Vector3(GetPitch()));
            if (CameraYaw != null)
                CameraYaw.Rotation = new Quaternion(new Vector3(0, GetYaw()));

            // Sphere cast to determine safe camera position.
            if (CameraNode != null)
            {
                var parent = CameraNode.Parent;
                var physicsWorld = Scene.GetComponent<PhysicsWorld>();
                if (physicsWorld != null)
                {
                    var ray = new Ray(parent.WorldPosition, -parent.WorldDirection);

                    var cameraDistance = CameraDistance;
                    if (State == CharacterState.InVehicle)
                        cameraDistance *= 2.0f;
                    var target = ray.Origin + ray.Direction * cameraDistance;
                    physicsWorld.SphereCast(_physicsRaycastResult, ray, 0.1f, cameraDistance, CameraCollisionMask);
                    var distance = Math.Min(cameraDistance, _physicsRaycastResult.Distance);
                    if (CameraYaw != null && distance < cameraDistance - 1e-6f)
                    {
                        var from = CameraYaw.WorldPosition;
                        var diff = target - from;
                        var diffLength = diff.Length;
                        diff = diff / diffLength;
                        var alternativeRay = new Ray(from, diff);
                    }

                    CameraNode.WorldPosition = ray.Origin + ray.Direction * distance;
                }
            }
        }

        /// <summary>
        /// Enable or disable character controller physics.
        /// </summary>
        /// <param name="enable"></param>
        public void SetPhysicsEnabled(bool enable)
        {
            CharacterController.IsEnabled = enable;
        }

        /// <summary>
        ///     Input data for state.
        /// </summary>
        public class Inputs
        {
            /// Current frame time step.
            public float TimeStep;

            /// Current frame input velocity defined by AI or controls.
            public Vector3 InputVelocity;

            /// InputVelocity vector length.
            public float InputSpeed;

            /// Normalized input direction.
            public Vector3 InputDirection;

            /// Is jump button pressed.
            public bool Jump;

            /// Is character crouching.
            public bool Crouch;

            /// Last known velocity.
            public Vector3 CurrentVelocity;
        }
    }
}