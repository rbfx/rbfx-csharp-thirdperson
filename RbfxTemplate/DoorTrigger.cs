using Urho3DNet;

namespace RbfxTemplate
{
    [ObjectFactory(Category = "Component/Game")]
    public partial class DoorTrigger : TriggerAnimator
    {
        public DoorTrigger(Context context) : base(context)
        {
        }

        [SerializeField(Mode = AttributeMode.AmDefault, Name = "Item Definition")]
        public ResourceRef ItemDefinition { get; set; } = new ResourceRef(nameof(ItemDefinitionResource));

        public override bool Filter(Node node)
        {
            if (ItemDefinition != null)
            {
                var player = node.GetComponent<Player>();
                if (player != null) return player.HasInInventory(ItemDefinition);
                return false;
            }

            return true;
        }
    }
}