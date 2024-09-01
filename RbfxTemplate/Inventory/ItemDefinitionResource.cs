using RbfxTemplate.Inventory;
using RbfxTemplate.Utils;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    /// Resource container for ItemDefinition.
    /// </summary>
    [ObjectFactory]
    public partial class ItemDefinitionResource: JsonResource<ItemDefinition>
    {
        public ItemDefinitionResource(Context context) : base(context)
        {
        }
    }
}