using System.Collections.Generic;
using RbfxTemplate.CharacterStates;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Player : LogicComponent
    {
        private readonly PhysicsRaycastResult _raycastResult;

        private readonly HashSet<string> _inventory = new HashSet<string>();
        private bool _usePressed;
        private Node _selectedNode;
        private IInteractable _interactable;
        private Character _character;

        /// <summary>
        /// How much time elapsed since interaction started.
        /// </summary>
        private float _interactionElapsed;

        public Player(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate | UpdateEvent.UseFixedupdate;
            _raycastResult = new PhysicsRaycastResult();
        }

        public Camera Camera { get; set; }

        public Node SelectedNode
        {
            get => _selectedNode;
            set
            {
                if (_selectedNode != value)
                { 
                    if (_selectedNode != null && !_selectedNode.IsExpired)
                    {
                        _interactable?.OnHoverEnd(this);
                    }

                    _selectedNode = value;
                    _interactable = null;
                    _interactionElapsed = 0.0f;

                    if (_selectedNode != null)
                    {
                        _interactable = _selectedNode.GetDerivedComponent<IInteractable>() ??
                                        _selectedNode.GetParentDerivedComponent<IInteractable>();

                        _interactable?.OnHoverStart(this);
                    }
                }
            }
        }

        public IInteractable Interactable
        {
            get => _interactable;
        }

        public Node AttractionTarget { get; set; }

        public RigidBody BodyInArms { get; set; }

        public Constraint Constraint { get; set; }

        public InputMap InputMap { get; set; }

        public float InteractionProgress
        {
            get
            {
                if (_selectedNode == null || _selectedNode.IsExpired || _interactable == null || _interactionElapsed == 0.0f)
                    return 0.0f;

                return _interactionElapsed / _interactable.InteractionDuration;
            }
        }

        public override void DelayedStart()
        {
            base.DelayedStart();
            _character = Node.GetDerivedComponent<MoveAndOrbitComponent>() as Character;
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            if (_selectedNode != null && _selectedNode.IsExpired)
            {
                SelectedNode = null;
            }

            _character.Jump = InputMap.Evaluate("Jump") > 0.5f;

            var usePressed = InputMap.Evaluate("Use") > 0.5f;

            if (usePressed && _interactionElapsed > 0.0f && _interactable != null)
            {
                _interactionElapsed += timeStep;
                if (_interactionElapsed >= _interactable.InteractionDuration)
                {
                    CompleteInteraction();
                }
            }

            if (usePressed != _usePressed)
            {
                _usePressed = usePressed;
                if (_usePressed)
                {
                    if (_character.State == CharacterState.InVehicle)
                    {
                        _character.TransitionToState(CharacterState.Start);
                        return;
                    }

                    BodyInArms = null;
                    if (_interactable != null && _interactable.InteractionEnabled)
                    {
                        if (_interactable.InteractionDuration > timeStep)
                        {
                            _interactionElapsed = timeStep;
                        }
                        else
                        {
                            CompleteInteraction();
                        }
                    }
                }
                else
                {
                    Constraint.OtherBody = null;
                    BodyInArms = null;
                    _interactionElapsed = 0.0f;
                }
            }


            if (BodyInArms == null)
            {
                var world = Scene.GetComponent<PhysicsWorld>();
                world.RaycastSingle(_raycastResult, new Ray(Camera.Node.WorldPosition, Camera.Node.WorldDirection),
                    4.0f - Camera.Node.Position.Z);
                var selectedNode = _raycastResult.Body?.Node;
                SelectedNode = selectedNode;
            }
        }

        private void CompleteInteraction()
        {
            _interactionElapsed = 0.0f;
            _interactable?.Interact(this);
        }

        public void AddToInventory(string inventoryKey)
        {
            _inventory.Add(inventoryKey);
        }

        public bool HasInInventory(string inventoryKey)
        {
            return _inventory.Contains(inventoryKey);
        }

        public void GetIntoVehicle(Vehicle vehicle)
        {
            _character.TransitionToState(CharacterState.InVehicle, vehicle);
        }

        public void TakeInHands(Node node)
        {
            BodyInArms = node.GetComponent<RigidBody>();
            if (BodyInArms.Mass <= 0)
                BodyInArms = null;
            Constraint.OtherBody = BodyInArms;
            SelectedNode = null;
        }
    }
}