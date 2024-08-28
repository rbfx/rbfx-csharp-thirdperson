using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class VehicleInteraction : Selectable
    {
        public VehicleInteraction(Context context) : base(context)
        {
        }

        public override bool InteractionEnabled { get; } = true;

        public override float InteractionDuration { get; } = 1.0f;

        public override void Interact(Player player)
        {
            player.GetIntoVehicle(Node.GetComponent<Vehicle>());
        }
    }
}