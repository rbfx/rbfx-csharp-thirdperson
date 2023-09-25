using Urho3DNet;

namespace RbfxTemplate
{
    public abstract class MenuStateBase : ApplicationState
    {
        protected readonly SharedPtr<Scene> _scene;
        private readonly Viewport _viewport;
        private readonly MenuComponent _uiComponent;

        public MenuStateBase(UrhoPluginApplication app, string rmlResource) : base(app.Context)
        {
            MouseMode = MouseMode.MmFree;
            IsMouseVisible = true;
            Application = app;

            _scene = Context.CreateObject<Scene>();
            var scene = _scene.Ptr;
            scene.CreateComponent<Octree>();
            var zone = scene.CreateComponent<Zone>();
            zone.FogColor = Color.Blue;
            zone.SetBoundingBox(new BoundingBox(-100, 100));
            var cameraNode = scene.CreateChild();
            var camera = cameraNode.CreateComponent<Camera>();
            _viewport = Context.CreateObject<Viewport>();
            _viewport.Scene = _scene;
            _viewport.Camera = camera;
            SetViewport(0, _viewport);
            _uiComponent = _scene.Ptr.CreateComponent<MenuComponent>();
            _uiComponent.IsEnabled = false;
            _uiComponent.State = this;
            _uiComponent.SetResource(rmlResource);
            Deactivate();
        }

        public Scene Scene => _scene;

        public UrhoPluginApplication Application { get; }

        public abstract void OnDataModelInitialized(MenuComponent menuComponent);

        public override void Activate(StringVariantMap bundle)
        {
            _uiComponent.IsEnabled = true;
            _scene.Ptr.IsUpdateEnabled = true;
            _uiComponent.UpdateProperties();
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            _uiComponent.IsEnabled = false;
            _scene.Ptr.IsUpdateEnabled = false;
            UnsubscribeFromEvent(E.KeyUp);
            base.Deactivate();
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