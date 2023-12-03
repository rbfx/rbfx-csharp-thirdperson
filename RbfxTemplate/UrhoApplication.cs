using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     This class represents an Urho3D application.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class UrhoApplication : Application
    {
        /// <summary>
        ///     Safe pointer to settings screen.
        /// </summary>
        private SharedPtr<UrhoPluginApplication> _pluginApplication;

        /// <summary>
        ///     Safe pointer to debug HUD.
        /// </summary>
        private SharedPtr<DebugHud> _debugHud;

        public UrhoApplication(Context context) : base(context)
        {
        }



        /// <summary>
        ///     Setup application.
        ///     This method is executed before most of the engine system initialized.
        /// </summary>
        public override void Setup()
        {
            // Set up engine parameters
            EngineParameters[Urho3D.EpFullScreen] = false;
            EngineParameters[Urho3D.EpWindowResizable] = true;
            EngineParameters[Urho3D.EpWindowTitle] = "RbfxTemplate";
            EngineParameters[Urho3D.EpApplicationName] = "RbfxTemplate";
            EngineParameters[Urho3D.EpOrganizationName] = "RbfxTemplate";
            EngineParameters[Urho3D.EpFrameLimiter] = true;
            EngineParameters[Urho3D.EpConfigName] = "";

            // Run shaders via SpirV-Cross to eliminate potential driver bugs
            //EngineParameters[Urho3D.EpShaderPolicy] = 0;
            // Enable this if you need to debug translated shaders.
            //EngineParameters[Urho3D.EpShaderLogSources] = true;

            base.Setup();
        }

        /// <summary>
        ///     Start application.
        /// </summary>
        public override void Start()
        {
            // Subscribe for log messages.
            SubscribeToEvent(E.LogMessage, OnLogMessage);

            // Limit frame rate tp 60 FPS as a workaround for kinematic character controller movement.
            Context.Engine.MaxFps = 60;


#if DEBUG
            // Setup Debug HUD when building in Debug configuration.
            _debugHud = Context.Engine.CreateDebugHud();
            _debugHud.Ptr.Mode = DebugHudMode.DebughudShowAll;
#endif

            _pluginApplication = new UrhoPluginApplication(Context);
            _pluginApplication.Ptr.LoadPlugin();
            _pluginApplication.Ptr.StartApplication(true);

            base.Start();
        }

        public override void Stop()
        {
            if (_pluginApplication)
            {
                _pluginApplication.Ptr.StopApplication();
                _pluginApplication.Ptr.UnloadPlugin();
                _pluginApplication.Dispose();
            }

            _debugHud?.Dispose();
            base.Stop();
        }


        private void OnLogMessage(VariantMap args)
        {
            var logLevel = (LogLevel)args[E.LogMessage.Level].Int;
            switch (logLevel)
            {
                case LogLevel.LogError:
                    throw new ApplicationException(args[E.LogMessage.Message].String);
                default:
                    Debug.WriteLine(args[E.LogMessage.Message].String);
                    break;
            }
        }
    }
}