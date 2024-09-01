using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class InventoryItem : Selectable
    {
        public InventoryItem(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Item Definition")]
        public ResourceRef ItemDefinition { get; set; } = new ResourceRef(nameof(ItemDefinitionResource));

        /// <inheritdoc/>
        public override bool InteractionEnabled { get; } = true;

        public override void Interact(Player player)
        {
            player.AddToInventory(ItemDefinition);
            OnHoverEnd(player);
            Node.Remove();
        }
    }
}