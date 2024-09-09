using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    public partial class GameState : RmlUIStateBase
    {
        private readonly string _sceneResourceName = "Scenes/Sample.xml";

        protected SharedPtr<Scene> _scene;
        protected SharedPtr<Sprite> _cross;
        private Node _cameraNode;
        private Viewport _viewport;
        private Node _character;
        private Node _cameraRoot;
        private Player _player;

        private string _interactableTooltip = string.Empty;
        private bool _interactionEnabled = false;
        private bool _hasInteractable = false;
        private float _interactionProgress;
        private InputMap _inputMap;
        private bool _inTransition = false;

        public Player Player => _player;

        public string InteractableTooltip
        {
            get => _interactableTooltip;
            set => SetRmlVariable(ref _interactableTooltip, value);
        }

        public bool InteractionEnabled
        {
            get => _interactionEnabled;
            set => SetRmlVariable(ref _interactionEnabled, value);
        }

        public bool HasInteractable
        {
            get => _hasInteractable;
            set => SetRmlVariable(ref _hasInteractable, value);
        }

        public float InteractionProgress
        {
            get => _interactionProgress;
            set => SetRmlVariable(ref _interactionProgress, value);
        }
        
        public GameState(UrhoPluginApplication app) : base(app, "UI/GameUI.rml")
        {
            MouseMode = MouseMode.MmRelative;
            IsMouseVisible = false;

            app.QueueSceneResourcesAsync(_sceneResourceName);
        }

        public override void OnDataModelInitialized(GameRmlUIComponent menuComponent)
        {
            menuComponent.BindDataModelProperty(nameof(InteractableTooltip), _ => _.Set(_interactableTooltip), _ => { });
            menuComponent.BindDataModelProperty(nameof(InteractionEnabled), _ => _.Set(_interactionEnabled), _ => { });
            menuComponent.BindDataModelProperty(nameof(HasInteractable), _ => _.Set(_hasInteractable), _ => { });
            menuComponent.BindDataModelProperty(nameof(InteractionProgress), _ => _.Set(_interactionProgress), _ => { });
        }

        public override void Activate(StringVariantMap bundle)
        {
            EnsureScene();

            Application.Settings.Apply(_scene.Ptr.GetComponent<RenderPipeline>());
            Application.Settings.Apply(Context);
            base.Activate(bundle);
        }

        private void EnsureScene()
        {
            if (_scene)
            {
                return;
            }

            _inputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");

            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML(_sceneResourceName);

            var nodeList = _scene.Ptr.GetChildrenWithComponent(nameof(KinematicCharacterController), true);
            foreach (var node in nodeList)
            {
                SetupCharacter(node);
                node.CreateComponent<NonPlayableCharacter>();
            }

            _character = _scene.Ptr.CreateChild();
            _character.Position = new Vector3(0, 0.2f);
            _character.CreateComponent<PrefabReference>()
                .SetPrefab(Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/YBot/YBot.prefab"));

            var character = SetupCharacter(_character);
            _player = _character.CreateComponent<Player>();
            _player.InputMap = _inputMap;
            _player.PistolAttachment = character.ModelPivot.FindChild("PistolAttachment", true);
            _player.RifleAttachment = character.ModelPivot.FindChild("RifleAttachment", true);
            _player.BowPivot = character.ModelPivot.FindChild("BowPivot", true);
            _player.AttractionTarget = character.ModelPivot.CreateChild("AttractionTarget");
            _player.AttractionTarget.Position = new Vector3(0, 1.0f, 1.5f);
            _player.AttractionTarget.CreateComponent<RigidBody>();
            _player.Constraint = _player.AttractionTarget.CreateComponent<Constraint>();
            _player.Constraint.ConstraintType = ConstraintType.ConstraintSlider;
            _cameraRoot = _character.CreateChild();
            var cameraPrefab = _cameraRoot.CreateComponent<PrefabReference>();
            cameraPrefab.SetPrefab(
                Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/Camera.prefab"));
            cameraPrefab.Inline(PrefabInlineFlag.None);
            _player.Camera = _character.FindComponent<Camera>();
            _cameraNode = _player.Camera.Node;
            _character.CreateComponent<MoveAndOrbitController>().InputMap = _inputMap;
            character.CameraYaw = _character.GetChild("CameraYawPivot", true);
            character.CameraPitch = _character.GetChild("CameraPitchPivot", true);
            character.CameraNode = _cameraNode;
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Camera = _cameraNode?.GetComponent<Camera>();
            _viewport.Scene = _scene;
            SetViewport(0, _viewport);
            _scene.Ptr.IsUpdateEnabled = false;

            _cross = SharedPtr.MakeShared<Sprite>(Context);
            var crossTexture = ResourceCache.GetResource<Texture2D>("Images/Cross.png");
            _cross.Ptr.Texture = crossTexture;
            _cross.Ptr.Size = new IntVector2(64, 64);
            _cross.Ptr.VerticalAlignment = VerticalAlignment.VaCenter;
            _cross.Ptr.HorizontalAlignment = HorizontalAlignment.HaCenter;
            _cross.Ptr.HotSpot = new IntVector2(32, 32);
            UIRoot.AddChild(_cross);
        }

        public override void TransitionStarted()
        {
            _inTransition = true;

            _scene.Ptr.IsUpdateEnabled = false;
            UnsubscribeFromEvent(E.KeyUp);
        }

        public override void TransitionComplete()
        {
            _inTransition = false;

            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            _scene.Ptr.IsUpdateEnabled = true;
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        public override void Update(float timeStep)
        {
            if (_inTransition)
                return;

            base.Update(timeStep);

            var interactable = _player.Interactable;
            InteractionProgress = _player.InteractionProgress;
            HasInteractable = interactable != null;
            if (HasInteractable)
            {
                InteractableTooltip = interactable.Tooltip ?? string.Empty;
                InteractionEnabled = interactable.InteractionEnabled;
            }

            if (_inputMap.Evaluate("Inventory") > 0.5f)
            {
                Application.OpenInventory();
                _scene.Ptr.IsUpdateEnabled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            _scene?.Dispose();

            base.Dispose(disposing);
        }

        private Character SetupCharacter(Node _character)
        {
            var player = _character.CreateComponent<Character>();
            player.CharacterController = _character.GetComponent<KinematicCharacterController>();
            player.CameraCollisionMask = uint.MaxValue & ~player.CharacterController.CollisionLayer;
            player.AnimationController = _character.FindComponent<AnimationController>();
            player.ModelPivot = _character.GetChild("ModelPivot");

            player.Idle = Context.ResourceCache.GetResource<Animation>("Animations/EmptyHanded/Idle.ani");
            player.Walk = Context.ResourceCache.GetResource<Animation>("Animations/EmptyHanded/Walking.ani");
            player.Run = Context.ResourceCache.GetResource<Animation>("Animations/EmptyHanded/Running.ani");
            player.Falling = Context.ResourceCache.GetResource<Animation>("Animations/EmptyHanded/FallingIdle.ani");

            //player.Idle = Context.ResourceCache.GetResource<Animation>("Animations/Rifle/Idle.ani");
            //player.Walk = Context.ResourceCache.GetResource<Animation>("Animations/Rifle/WalkForward.ani");
            //player.Run = Context.ResourceCache.GetResource<Animation>("Animations/Rifle/RunForward.ani");
            //player.Falling = Context.ResourceCache.GetResource<Animation>("Animations/EmptyHanded/FallingIdle.ani");


            player.Drive = Context.ResourceCache.GetResource<Animation>("Animations/Driving.ani");
            return player;
        }

        private void HandleKeyUp(VariantMap args)
        {
            var key = (Key)args[E.KeyUp.Key].Int;
            switch (key)
            {
                case Key.KeyEscape:
                case Key.KeyBackspace:
                    Application.HandleBackKey();
                    return;
            }
        }
    }
}