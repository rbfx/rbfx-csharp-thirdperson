using Urho3DNet;

namespace RbfxTemplate
{
    public partial class SettingsMenuState : RmlUIStateBase
    {
        public SettingsMenuState(UrhoPluginApplication app) : base(app, "UI/Options.rml")
        {
            Settings = app.Settings;
        }

        public GameSettings Settings { get; private set; }

        public override void OnDataModelInitialized(GameRmlUIComponent menuComponent)
        {
            menuComponent.BindDataModelEvent("Apply", OnApply);
            menuComponent.BindDataModelEvent("Cancel", OnCancel);
            menuComponent.BindDataModelProperty("bloom", val => val.Set(Settings.Bloom),
                val => Settings.Bloom = val.Bool);
            menuComponent.BindDataModelProperty("ssao", val => val.Set(Settings.SSAO), val => Settings.SSAO = val.Bool);
            menuComponent.BindDataModelProperty("master", val => val.Set(Settings.MasterVolume),
                val => Settings.MasterVolume = val.Float);
            menuComponent.BindDataModelProperty("music", val => val.Set(Settings.MusicVolume),
                val => Settings.MusicVolume = val.Float);
            menuComponent.BindDataModelProperty("effects", val => val.Set(Settings.EffectVolume),
                val => Settings.EffectVolume = val.Float);
            menuComponent.BindDataModelProperty("debughud", val => val.Set(GetDebugHUD()),
                val => SetDebugHUD(val.Bool));
            //menuComponent.BindDataModelProperty("shadows", val => val.Set(_shadowsQuality), (val) => _shadowsQuality = val.Convert(VariantType.VarInt).Int);
        }

        private void SetDebugHUD(bool value)
        {
            if (value)
            {
                Context.Engine.CreateDebugHud().Mode = DebugHudMode.DebughudShowAll;
            }
            else
            {
                var hud = GetSubsystem<DebugHud>();
                if (hud != null)
                {
                    hud.Mode = DebugHudMode.DebughudShowNone;
                }
            }
        }

        private bool GetDebugHUD()
        {
            var hud = GetSubsystem<DebugHud>();
            if (hud == null)
                return false;
            return hud.Mode != DebugHudMode.DebughudShowNone;
        }

        public override void Activate(StringVariantMap bundle)
        {
            Settings = Application.Settings;

            var audio = Context.GetSubsystem<Audio>();

            base.Activate(bundle);
        }

        public override void Deactivate()
        {
            base.Deactivate();
        }

        private void OnCancel(VariantList obj)
        {
            Application.ResetSettings();

            Application.HandleBackKey();
        }

        private void OnApply(VariantList obj)
        {
            Settings.Apply(Context);
            Application.SaveSettings();

            Application.HandleBackKey();
        }
    }
}