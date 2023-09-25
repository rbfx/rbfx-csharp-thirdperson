using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public class MenuComponent : RmlUIComponent
    {
        private bool _bloom = true;

        public MenuComponent(Context context) : base(context)
        {
        }

        public MenuStateBase State { get; set; }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        protected override void OnDataModelInitialized()
        {
            State.OnDataModelInitialized(this);
            base.OnDataModelInitialized();
        }
    }
}