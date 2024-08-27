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
                    _interactable?.OnHoverEnd(this);

                    _selectedNode = value;
                    _interactable = null;
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
        public float InteractionProgress { get; set; }

        public override void DelayedStart()
        {
            base.DelayedStart();
            _character = Node.GetDerivedComponent<MoveAndOrbitComponent>() as Character;
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            _character.Jump = InputMap.Evaluate("Jump") > 0.5f;

            var usePressed = InputMap.Evaluate("Use") > 0.5f;
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
                    if (SelectedNode != null)
                    {
                        Interactable?.Interact(this);
                    }
                }
                else
                {
                    Constraint.OtherBody = null;
                    BodyInArms = null;
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