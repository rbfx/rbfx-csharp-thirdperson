using System.Diagnostics;
using System.Runtime.InteropServices;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory]
    public partial class MainMenuState : RmlUIStateBase
    {
        public MainMenuState(UrhoPluginApplication app) : base(app, "UI/MainMenu.rml")
        {
        }

        public override void OnDataModelInitialized(GameRmlUIComponent menuComponent)
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

        private void OnDiscord(VariantList obj)
        {
            Urho3D.OpenURL("https://discord.gg/46aKYFQj7W");
        }
    }
}