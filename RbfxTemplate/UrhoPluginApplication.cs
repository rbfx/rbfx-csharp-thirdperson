using System.IO;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     This class represents an Urho3D plugin application.
    /// </summary>
    [LoadablePlugin]
    [Preserve(AllMembers = true)]
    public class UrhoPluginApplication : PluginApplication
    {
        /// <summary>
        ///     Safe pointer to game screen.
        /// </summary>
        private SharedPtr<GameState> _gameState;

        /// <summary>
        ///     Safe pointer to menu screen.
        /// </summary>
        private SharedPtr<MainMenuState> _mainMenuState;

        /// <summary>
        ///     Safe pointer to settings screen.
        /// </summary>
        private SharedPtr<SettingsMenuState> _settingsMenuState;

        /// <summary>
        ///     Application state manager.
        /// </summary>
        private StateStack _stateStack;

        public UrhoPluginApplication(Context context) : base(context)
        {
        }


        /// <summary>
        ///     Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning => _gameState;

        /// <summary>
        ///     Gets or sets the settings file.
        /// </summary>
        public SettingFile Settings { get; set; }

        protected override void Load()
        {
            Context.RegisterFactories(GetType().Assembly);
        }

        protected override void Unload()
        {
            Context.RemoveFactories(GetType().Assembly);
        }

        public override bool IsMain()
        {
            return true;
        }

        protected override void Start(bool isMain)
        {
            // Load settings.
            try
            {
                Settings = SettingFile.Load(Context);
            }
            catch (FileNotFoundException)
            {
                Settings = new SettingFile();
            }

            _stateStack = new StateStack(Context.GetSubsystem<StateManager>());

            // Loads all fonts from the resource cache and adds them to the RmlUI.
            var cache = GetSubsystem<ResourceCache>();
            var ui = GetSubsystem<RmlUI>();
            var fonts = new StringList();
            // Scan for .ttf files and load them
            cache.Scan(fonts, "Fonts/", "*.ttf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");
            // Scan for .otf files and load them
            cache.Scan(fonts, "Fonts/", "*.otf", ScanFlag.ScanFiles);
            foreach (var font in fonts) ui.LoadFont($"Fonts/{font}");

            // Setup state manager.
            var stateManager = Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;

            // Setup end enqueue splash screen.
            using (SharedPtr<SplashScreen> splash = new SplashScreen(Context))
            {
                splash.Ptr.Duration = 1.0f;
                splash.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Background.png");
                splash.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
                stateManager.EnqueueState(splash);
            }


            // Crate end enqueue main menu screen.
            _mainMenuState = _mainMenuState ?? new MainMenuState(this);
            _stateStack.Push(_mainMenuState);

            base.Start(isMain);
        }

        protected override void Stop()
        {
            _mainMenuState?.Dispose();
            _gameState?.Dispose();

            base.Stop();
        }

        protected override void Suspend(Archive output)
        {
            base.Suspend(output);
        }

        protected override void Resume(Archive input, bool differentVersion)
        {
            base.Resume(input, differentVersion);
        }


        /// <summary>
        ///     Transition to settings menu
        /// </summary>
        public void ToSettings()
        {
            _settingsMenuState = _settingsMenuState ?? new SettingsMenuState(this);
            _stateStack.Push(_settingsMenuState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ToNewGame()
        {
            _gameState?.Dispose();
            _gameState = new GameState(this);
            _stateStack.Push(_gameState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ContinueGame()
        {
            if (_gameState) _stateStack.Push(_gameState);
            ;
        }

        public void Quit()
        {
            Context.Engine.Exit();
        }

        public void HandleBackKey()
        {
            if (_stateStack.State == _mainMenuState.Ptr)
            {
                if (IsGameRunning)
                    ContinueGame();
                else
                    Quit();
            }
            else
            {
                _stateStack.Pop();
            }
        }
    }
}