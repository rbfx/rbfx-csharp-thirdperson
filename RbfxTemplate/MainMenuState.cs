using System.Diagnostics;
using System.Runtime.InteropServices;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    [Preserve(AllMembers = true)]
    public class MainMenuState : MenuStateBase
    {
        public MainMenuState(UrhoPluginApplication app) : base(app, "UI/MainMenu.rml")
        {
        }

        public override void OnDataModelInitialized(MenuComponent menuComponent)
        {
            menuComponent.BindDataModelProperty("is_game_played", _ => _.Set(Application?.IsGameRunning == true),
                _ => { });
            //menuComponent.BindDataModelProperty("bloom", _ => _.Set(_bloom), _ => { _bloom = _.Bool; });
            menuComponent.BindDataModelProperty("game_title", _ => _.Set("Awesome game"), _ => { });
            menuComponent.BindDataModelEvent("Continue", OnContinue);
            menuComponent.BindDataModelEvent("NewGame", OnNewGame);
            menuComponent.BindDataModelEvent("Settings", OnSettings);
            menuComponent.BindDataModelEvent("Exit", OnExit);
            menuComponent.BindDataModelEvent("Discord", OnDiscord);
        }

        public void OnNewGame(VariantList variantList)
        {
            Application.ToNewGame();
        }

        public void OnSettings(VariantList variantList)
        {
            Application.ToSettings();
        }

        public void OnExit(VariantList variantList)
        {
            Application.Quit();
        }

        public void OnContinue(VariantList variantList)
        {
            Application.ContinueGame();
        }

        private void OpenUrlInBrowser(string url)
        {
            // Open URL in default browser: https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        private void OnDiscord(VariantList obj)
        {
            OpenUrlInBrowser("https://discord.gg/46aKYFQj7W");
        }
    }
}