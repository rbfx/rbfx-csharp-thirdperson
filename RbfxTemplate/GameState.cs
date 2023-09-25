using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    [Preserve(AllMembers = true)]
    public class GameState : ApplicationState
    {
        protected readonly SharedPtr<Scene> _scene;
        protected readonly SharedPtr<Sprite> _cross;
        private readonly UrhoPluginApplication _app;
        private readonly Node _cameraNode;
        private readonly Viewport _viewport;
        private readonly Node _character;
        private readonly Node _cameraRoot;

        public GameState(UrhoPluginApplication app) : base(app.Context)
        {
            MouseMode = MouseMode.MmRelative;
            IsMouseVisible = false;

            var inputMap = Context.ResourceCache.GetResource<InputMap>("Input/MoveAndOrbit.inputmap");

            _app = app;
            _scene = Context.CreateObject<Scene>();
            _scene.Ptr.LoadXML("Scenes/Sample.xml");

            var selectables = _scene.Ptr.GetChildrenWithTag("Selectable", true);
            foreach (var box in selectables) box.CreateComponent<Selectable>();

            {
                var doorButtons = _scene.Ptr.GetChildrenWithTag("DoorButton", true);
                var openAnimation = Context.ResourceCache.GetResource<Animation>("Animations/SlidingDoor/Open.xml");
                var closeAnimation = Context.ResourceCache.GetResource<Animation>("Animations/SlidingDoor/Close.xml");
                foreach (var box in doorButtons)
                {
                    var c = box.CreateComponent<DoorButton>();
                    c.OpenAnimation = openAnimation;
                    c.CloseAnimation = closeAnimation;
                }
            }

            {
                var doorKeys = _scene.Ptr.GetChildrenWithTag("DoorKey", true);
                foreach (var box in doorKeys)
                {
                    var c = box.CreateComponent<Pickable>();
                    c.InventoryKey = box.Name;
                }

                var redKeyDoor = _scene.Ptr.GetChild("RedKeyDoor", true);
                var triggerAnimator = redKeyDoor.GetComponent<TriggerAnimator>(true);
                var newTrigger = triggerAnimator.Node.CreateComponent<DoorTrigger>();
                newTrigger.InventoryKey = "RedKey";
                newTrigger.EnterAnimation = triggerAnimator.EnterAnimation;
                newTrigger.ExitAnimation = triggerAnimator.ExitAnimation;
                triggerAnimator.Remove();
            }

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
            var player = _character.CreateComponent<Player>();
            player.InputMap = inputMap;
            player.AttractionTarget = character.ModelPivot.CreateChild("AttractionTarget");
            player.AttractionTarget.Position = new Vector3(0, 1.0f, 1.0f);
            player.AttractionTarget.CreateComponent<RigidBody>();
            player.Constraint = player.AttractionTarget.CreateComponent<Constraint>();
            player.Constraint.ConstraintType = ConstraintType.ConstraintSlider;
            _cameraRoot = _character.CreateChild();
            var cameraPrefab = _cameraRoot.CreateComponent<PrefabReference>();
            cameraPrefab.SetPrefab(
                Context.ResourceCache.GetResource<PrefabResource>("Models/Characters/Camera.prefab"));
            cameraPrefab.Inline(PrefabInlineFlag.None);
            player.Camera = _character.GetComponent<Camera>(true);
            _cameraNode = player.Camera.Node;
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