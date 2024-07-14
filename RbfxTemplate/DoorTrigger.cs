using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    [Preserve(AllMembers = true)]
    public partial class DoorTrigger : TriggerAnimator
    {
        public DoorTrigger(Context context) : base(context)
        {
        }

        public string InventoryKey { get; set; }

        public override bool Filter(Node node)
        {
            if (!string.IsNullOrEmpty(InventoryKey))
            {
                var player = node.GetComponent<Player>();
                if (player != null) return player.HasInInventory(InventoryKey);
                return false;
            }

            return true;
        }
    }
}