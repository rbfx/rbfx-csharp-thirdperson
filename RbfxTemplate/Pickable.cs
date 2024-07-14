using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public partial class Pickable : Component
    {
        public Pickable(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Inventory Key")] public string InventoryKey { get; set; } = string.Empty;

        protected override void OnNodeSet(Node previousNode, Node currentNode)
        {
            if (currentNode != null)
                SubscribeToEvent("Use", currentNode, HandleUse);
            else
                UnsubscribeFromEvent("Use");
            base.OnNodeSet(previousNode, currentNode);
        }

        private void HandleUse(VariantMap args)
        {
            var player = args["Player"].Ptr as Player;
            if (player != null)
            {
                player.AddToInventory(InventoryKey);
                Node.Remove();
            }
        }
    }
}