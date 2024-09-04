using System.Collections.Generic;
using System.Linq;
using RbfxTemplate.CharacterStates;
using RbfxTemplate.Inventory;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Player : LogicComponent
    {
        private readonly PhysicsRaycastResult _raycastResult;

        private static readonly StringHash FieldNameResource = "Resource";
        private static readonly StringHash FieldNameCount = "Count";

        private readonly Dictionary<string, InventorySlot> _inventory = new Dictionary<string, InventorySlot>();

        //private readonly HashSet<ResourceRef> _inventory = new HashSet<ResourceRef>();
        private bool _usePressed;
        private Node _selectedNode;
        private PrefabReference _wieldAttachment;
        private IInteractable _interactable;
        private Character _character;

        /// <summary>
        /// How much time elapsed since interaction started.
        /// </summary>
        private float _interactionElapsed;

        private IKSolver _solver;

        public Player(Context context) : base(context)
        {
            UpdateEventMask = UpdateEvent.UseUpdate | UpdateEvent.UseFixedupdate;
            _raycastResult = new PhysicsRaycastResult();
        }

        public IReadOnlyDictionary<string, InventorySlot> Inventory
        {
            get => _inventory;
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
                                        _selectedNode.FindComponent<IInteractable>(ComponentSearchFlag.SelfOrParentRecursive | ComponentSearchFlag.Derived);

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

        public Node PistolAttachment { get; set; }
        public Node RifleAttachment { get; set; }
        public Node BowPivot { get; set; }

        public override void DelayedStart()
        {
            base.DelayedStart();
            _solver = Node.FindComponent<IKSolver>(ComponentSearchFlag.SelfOrChildrenRecursive | ComponentSearchFlag.Disabled);
            _character = Node.GetDerivedComponent<MoveAndOrbitComponent>() as Character;
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            if (BowPivot != null)
            {
                BowPivot.Rotation = new Quaternion(new Vector3( _character.GetPitch(), 0, 0));
            }

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

        public void AddToInventory(ResourceRef inventoryKey)
        {
            if (inventoryKey == null || string.IsNullOrEmpty(inventoryKey.Name))
            {
                return;
            }

            if (!_inventory.TryGetValue(inventoryKey.Name, out var inventorySlot))
            {
                inventorySlot = new InventorySlot()
                    { ItemDefinition = Context.ResourceCache.GetResource<ItemDefinitionResource>(inventoryKey.Name) };
                _inventory.Add(inventoryKey.Name, inventorySlot);
            }

            ++inventorySlot.Count;

            ItemDefinition itemDefinition = inventorySlot.ItemDefinition?.Value;
            if (itemDefinition != null)
            {
                if (itemDefinition.HoldingStyle != HoldingStyle.NotWieldable)
                {
                    TakeWieldable(inventorySlot);
                }
            }
        }

        private void TakeWieldable(InventorySlot inventorySlot)
        {
            var itemDefinition = inventorySlot?.ItemDefinition?.Value;
            if (itemDefinition == null || itemDefinition.HoldingStyle == HoldingStyle.NotWieldable)
            {
                return;
            }

            Node node = null;
            switch (itemDefinition.HoldingStyle)
            {
                case HoldingStyle.Pistol:
                    node = PistolAttachment ?? node;
                    break;
                case HoldingStyle.Rifle:
                    node = RifleAttachment ?? node;
                    break;
            }

            if (node == null)
                return;

            Holster();

            _wieldAttachment = node.GetOrCreateComponent<PrefabReference>();
            if (_wieldAttachment != null)
            {
                _wieldAttachment.SetPrefab(Context.ResourceCache.GetResource<PrefabResource>(itemDefinition.Prefab.Name));
                var rigidBody = node.FindComponent<RigidBody>();
                if (rigidBody != null)
                {
                    rigidBody.IsEnabled = false;
                }

                _solver.IsEnabled = true;

                //foreach (var solver in node.FindComponents<IKSolverComponent>())
                //{
                //    solver.IsEnabled = false;
                //    solver.IsEnabled = true;
                //}
            }
        }

        public bool HasInInventory(ResourceRef inventoryKey)
        {
            if (inventoryKey == null || string.IsNullOrEmpty(inventoryKey.Name))
            {
                return false;
            }

            return _inventory.ContainsKey(inventoryKey.Name);
        }

        public void GetIntoVehicle(Vehicle vehicle)
        {
            _character.TransitionToState(CharacterState.InVehicle, vehicle);
        }

        public void TakeInHands(Node node)
        {
            Holster();
            BodyInArms = node.GetComponent<RigidBody>();
            if (BodyInArms != null)
            {
                if (BodyInArms.Mass <= 0)
                    BodyInArms = null;
                Constraint.OtherBody = BodyInArms;
                SelectedNode = null;
            }
        }

        private void Holster()
        {
            if (BodyInArms != null)
            {
                Constraint.OtherBody = null;
                BodyInArms = null;
                _interactionElapsed = 0.0f;
            }

            if (_wieldAttachment != null)
            {
                _solver.IsEnabled = false;
                _wieldAttachment.SetPrefab(null);
                _wieldAttachment = null;
            }
        }
    }
}