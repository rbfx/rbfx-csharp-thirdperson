using System;
using System.Collections.Generic;
using System.Diagnostics;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     This class represents an Urho3D application.
    /// </summary>
    public partial class UrhoApplication : Application
    {
        /// <summary>
        ///     Safe pointer to settings screen.
        /// </summary>
        private SharedPtr<UrhoPluginApplication> _pluginApplication;

#if DEBUG
        /// <summary>
        ///     Safe pointer to debug HUD.
        /// </summary>
        private SharedPtr<DebugHud> _debugHud;
#endif

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
            EngineParameters[Urho3D.EpFullScreen] = false; //Use !Debugger.IsAttached if you need true fullscreen in production.
            EngineParameters[Urho3D.EpWindowResizable] = false;
            EngineParameters[Urho3D.EpWindowTitle] = "RbfxTemplate";
            EngineParameters[Urho3D.EpApplicationName] = "RbfxTemplate";
            EngineParameters[Urho3D.EpOrganizationName] = "RbfxTemplate";
            EngineParameters[Urho3D.EpFrameLimiter] = true;
            EngineParameters[Urho3D.EpConfigName] = "";

            ApplyCommandLineArguments();

            base.Setup();
        }

        private void ApplyCommandLineArguments()
        {
            try
            {
                var commandLineArgs = Environment.GetCommandLineArgs();
                for (var index = 0; index < commandLineArgs.Length; index++)
                {
                    switch (commandLineArgs[index])
                    {
                        case "--log-shader-sources": EngineParameters[Urho3D.EpShaderLogSources] = true; break;
                        case "--discard-shader-cache": EngineParameters[Urho3D.EpDiscardShaderCache] = true; break;
                        case "--no-save-shader-cache": EngineParameters[Urho3D.EpSaveShaderCache] = false; break;
                        case "--d3d11": EngineParameters[Urho3D.EpRenderBackend] = (int)RenderBackend.D3D11; break;
                        case "--d3d12": EngineParameters[Urho3D.EpRenderBackend] = (int)RenderBackend.D3D12; break;
                        case "--opengl": EngineParameters[Urho3D.EpRenderBackend] = (int)RenderBackend.OpenGl; break;
                        case "--vulkan": EngineParameters[Urho3D.EpRenderBackend] = (int)RenderBackend.Vulkan; break;
                        case "--fullscreen":
                        {
                            EngineParameters[Urho3D.EpFullScreen] = true;
                            EngineParameters[Urho3D.EpWindowResizable] = false;
                            EngineParameters[Urho3D.EpBorderless] = true;
                            break;
                        }
                        case "--windowed":
                        {
                            EngineParameters[Urho3D.EpFullScreen] = false;
                            EngineParameters[Urho3D.EpWindowResizable] = true;
                            EngineParameters[Urho3D.EpBorderless] = false;
                            break;
                        }
                        default: Log.Warning("Unknown argument " + commandLineArgs[index]); break;
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }
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
#if DEBUG
            _debugHud?.Dispose();
#endif
            base.Stop();
        }


        private void OnLogMessage(VariantMap args)
        {
            var logLevel = (LogLevel)args[E.LogMessage.Level].Int;
            switch (logLevel)
            {
#if DEBUG
                case LogLevel.LogError:
                    throw new ApplicationException(args[E.LogMessage.Message].String);
#endif
                default:
                    Debug.WriteLine(args[E.LogMessage.Message].String);
                    break;
            }
        }
    }
}