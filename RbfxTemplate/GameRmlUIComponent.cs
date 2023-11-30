using System;
using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public class GameRmlUIComponent : RmlUIComponent
    {
        public GameRmlUIComponent(Context context) : base(context)
        {
        }

        public RmlUIStateBase State { get; set; }

        public void UpdateProperties()
        {
            DirtyAllVariables();
        }

        protected override void OnDataModelInitialized()
        {
            State?.OnDataModelInitialized(this);

            base.OnDataModelInitialized();
        }
    }
}