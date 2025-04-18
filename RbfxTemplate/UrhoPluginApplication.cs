﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using RbfxTemplate.Utils;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     This class represents an Urho3D plugin application.
    /// </summary>
    [LoadablePlugin]
    public partial class UrhoPluginApplication : PluginApplication
    {
        /// <summary>
        /// Safe pointer to splash screen.
        /// </summary>
        private SharedPtr<SplashScreen> _splashScreen;

        /// <summary>
        ///     Safe pointer to game screen.
        /// </summary>
        private SharedPtr<GameState> _gameState;

        /// <summary>
        /// Inventory application state.
        /// </summary>
        private SharedPtr<InventoryState> _inventoryState;

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

        private SharedPtr<ConfigFileContainer<GameSettings>> _settings;


        public UrhoPluginApplication(Context context) : base(context)
        {
        }

        /// <summary>
        ///     Game settings
        /// </summary>
        public GameSettings Settings => _settings.Ptr.Value;

        /// <summary>
        ///     Gets a value indicating whether the game is running.
        /// </summary>
        public bool IsGameRunning => _gameState;

        /// <summary>
        /// Current game state or null if no game is running.
        /// </summary>
        public GameState Game => _gameState.Ptr;

        protected override void Load()
        {
            Context.RegisterFactories(GetType().Assembly);
        }

        protected override void Unload()
        {
            Context.RemoveFactories(GetType().Assembly);
        }

        protected override void Suspend(Archive output)
        {
            base.Suspend(output);
        }

        protected override void Resume(Archive input, bool differentVersion)
        {
            base.Resume(input, differentVersion);
        }

        public override bool IsMain()
        {
            return true;
        }

        protected override void Start(bool isMain)
        {
            // Load settings.
            ResetSettings();

            _stateStack = new StateStack(Context.GetSubsystem<StateManager>());

            _mainMenuState = new MainMenuState(this);

            // Setup state manager.
            var stateManager = Context.GetSubsystem<StateManager>();
            stateManager.FadeInDuration = 0.1f;
            stateManager.FadeOutDuration = 0.1f;

            // Setup end enqueue splash screen.
            _splashScreen = new SplashScreen(Context);
            _splashScreen.Ptr.Duration = 1.0f;
            _splashScreen.Ptr.BackgroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Background.png");
            _splashScreen.Ptr.ForegroundImage = Context.ResourceCache.GetResource<Texture2D>("Images/Splash.png");
            stateManager.EnqueueState(_splashScreen);

            // Crate end enqueue main menu screen.
            _stateStack.Push(_mainMenuState);

            base.Start(isMain);
        }

        protected override void Stop()
        {
            _mainMenuState?.Dispose();
            _gameState?.Dispose();
            _inventoryState?.Dispose();

            base.Stop();
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
        /// Queue all resources from the given scene to be loaded in background.
        /// </summary>
        /// <param name="sceneName"></param>
        public void QueueSceneResourcesAsync(string sceneName)
        {
            _splashScreen.Ptr.QueueSceneResourcesAsync(sceneName);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ToNewGame()
        {
            _gameState?.Dispose();
            _gameState = new GameState(this);
            _stateStack.Push(_splashScreen);
            _stateStack.Switch(_gameState);
        }

        /// <summary>
        ///     Transition to game
        /// </summary>
        public void ContinueGame()
        {
            if (_gameState) _stateStack.Push(_gameState);
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
                if (_stateStack.Count > 1)
                    _stateStack.Pop();
            }
        }

        public void SaveSettings()
        {
            _settings.Ptr.SaveConfig();
        }

        public void ResetSettings()
        {
            _settings?.Dispose();
            _settings = ConfigFileContainer<GameSettings>.LoadConfig(Context);
        }

        public void OpenInventory()
        {
            _inventoryState = _inventoryState ?? new SharedPtr<InventoryState>(new InventoryState(this));
            _stateStack.Push(_inventoryState);
        }
    }
}