using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    public partial class GameState : RmlUIStateBase
    {
        protected readonly SharedPtr<Scene> _scene;
        protected readonly SharedPtr<Sprite> _cross;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Viewport _viewport;
        private readonly Node _character;
        private readonly Node _cameraRoot;
        private readonly Player _player;

        private string _interactableTooltip = string.Empty;

        public string InteractableTooltip
        {
            get => _interactableTooltip;
            set => SetRmlVariable(ref _interactableTooltip, value);
        }

        public GameState(UrhoPluginApplication app) : base(app, "UI/GameUI.rml")
        {
            MouseMode = MouseMode.MmRelative;
            IsMouseVisible = false;

            var inputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");

            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/Sample.xml");

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
            _player.InputMap = inputMap;
            _player.AttractionTarget = character.ModelPivot.CreateChild("AttractionTarget");
            _player.AttractionTarget.Position = new Vector3(0, 1.0f, 1.0f);
            _player.AttractionTarget.CreateComponent<RigidBody>();
            _player.Constraint = _player.AttractionTarget.CreateComponent<Constraint>();
            _player.Constraint.ConstraintType = ConstraintType.ConstraintSlider;
            _cameraRoot = _character.CreateChild();
            var cameraPrefab = _cameraRoot.CreateComponent<PrefabReference>();
            cameraPrefab.SetPrefab(
                Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/Camera.prefab"));
            cameraPrefab.Inline(PrefabInlineFlag.None);
            _player.Camera = _character.GetComponent<Camera>(true);
            _cameraNode = _player.Camera.Node;
            _character.CreateComponent<MoveAndOrbitController>().InputMap = inputMap;
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

        public override void OnDataModelInitialized(GameRmlUIComponent menuComponent)
        {
            menuComponent.BindDataModelProperty(nameof(InteractableTooltip), _ => _.Set(_interactableTooltip), _ => { });
        }

        public override void Activate(StringVariantMap bundle)
        {
            SubscribeToEvent(E.KeyUp, HandleKeyUp);

            _scene.Ptr.IsUpdateEnabled = true;

            _app.Settings.Apply(_scene.Ptr.GetComponent<RenderPipeline>());

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _scene.Ptr.IsUpdateEnabled = false;
            UnsubscribeFromEvent(E.KeyUp);
            base.Deactivate();
        }

        public override void Update(float timeStep)
        {
            base.Update(timeStep);

            InteractableTooltip = _player.Interactable?.Title ?? string.Empty;
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
            player.AnimationController = _character.GetComponent<AnimationController>(true);
            player.ModelPivot = _character.GetChild("ModelPivot");
            player.Idle = Context.ResourceCache.GetResource<Animation>("Animations/Idle.ani");
            //player.Idle = Context.ResourceCache.GetResource<Animation>("Animations/CrouchIdle.ani");
            player.Walk = Context.ResourceCache.GetResource<Animation>("Animations/Walking.ani");
            player.Run = Context.ResourceCache.GetResource<Animation>("Animations/Running.ani");
            player.Falling = Context.ResourceCache.GetResource<Animation>("Animations/FallingIdle.ani");
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
                    _app.HandleBackKey();
                    return;
            }
        }
    }
}