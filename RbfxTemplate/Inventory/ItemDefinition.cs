using System.Text.Json.Serialization;
using Urho3DNet;

namespace RbfxTemplate.Inventory
{
    /// <summary>
    /// Item definition stored in <see cref="ItemDefinitionResource"/> container serialized in json via <see cref="System.Text.Json.JsonSerializer"/>.
    /// </summary>
    public class ItemDefinition
    {
        [JsonPropertyName("name")]
        public string Name
        {
            get; set;
        }

        [JsonPropertyName("icon")]
        public ResourceRef Icon
        {
            get; set;
        }

        [JsonPropertyName("prefab")]
        public ResourceRef Prefab
        {
            get; set;
        }

        [JsonPropertyName("weight")]
        public float Weight
        {
            get; set;
        }
    }
}