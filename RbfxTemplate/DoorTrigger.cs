using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class DoorTrigger : TriggerAnimator
    {
        public DoorTrigger(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Inventory Key")]
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