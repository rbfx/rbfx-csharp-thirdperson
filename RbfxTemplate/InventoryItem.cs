using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class InventoryItem : Selectable
    {
        public InventoryItem(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Inventory Key")]
        public string InventoryKey { get; set; } = string.Empty;

        /// <inheritdoc/>
        public override bool InteractionEnabled { get; } = true;

        public override void Interact(Player player)
        {
            player.AddToInventory(InventoryKey);
            OnHoverEnd(player);
            Node.Remove();
        }
    }
}