using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class Pickable : Selectable
    {
        public Pickable(Context context) : base(context)
        {
        }

        /// <inheritdoc/>
        public override bool InteractionEnabled { get; } = true;

        public override void Interact(Player player)
        {
            player.TakeInHands(Node);
            base.Interact(player);
        }
    }
}