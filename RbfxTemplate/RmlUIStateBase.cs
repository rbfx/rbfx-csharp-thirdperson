using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    /// Base class for a game state with RmlUI document.
    /// </summary>
    public abstract class RmlUIStateBase : ApplicationState
    {
        private readonly SharedPtr<Scene> _scene;
        private readonly GameRmlUIComponent _uiComponent;

        /// <summary>
        /// Construct RmlUIStateBase.
        /// </summary>
        /// <param name="app">Application instance.</param>
        /// <param name="rmlResource">Path to RmlUI document resource.</param>
        public RmlUIStateBase(UrhoPluginApplication app, string rmlResource) : base(app.Context)
        {
            MouseMode = MouseMode.MmFree;
            IsMouseVisible = true;
            Application = app;

            _scene = Context.CreateObject<Scene>();
            _uiComponent = new GameRmlUIComponent(Context);
            _uiComponent.IsEnabled = false;
            _uiComponent.State = this;
            _uiComponent.SetResource(rmlResource);
            _uiComponent.IsEnabled = false;
            _scene.Ptr.AddComponent(_uiComponent,0);
        }

        /// <summary>
        /// RmlUI Component for the game state.
        /// </summary>
        public GameRmlUIComponent RmlUiComponent => _uiComponent;

        /// <summary>
        /// Application instance.
        /// </summary>
        public UrhoPluginApplication Application { get; }

        /// <summary>
        /// Data model initializer.
        /// </summary>
        /// <param name="component">RmlUIComponent to initialize.</param>
        public abstract void OnDataModelInitialized(GameRmlUIComponent component);

        /// <summary>
        /// Game state activation handler.
        /// </summary>
        /// <param name="bundle">Game state parameters.</param>
        public override void Activate(StringVariantMap bundle)
        {
            _uiComponent.IsEnabled = true;
            _uiComponent.UpdateProperties();
            SubscribeToEvent(E.KeyUp, HandleKeyUp);
            base.Activate(bundle);
        }

        /// <summary>
        /// Game state deactivation handler.
        /// </summary>
        public override void Deactivate()
        {
            _uiComponent.IsEnabled = false;
            UnsubscribeFromEvent(E.KeyUp);
            base.Deactivate();
        }

        /// <summary>
        /// Key up handler to navigate back in the game state hierarchy.
        /// </summary>
        /// <param name="args"></param>
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