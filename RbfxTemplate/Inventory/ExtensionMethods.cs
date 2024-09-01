namespace RbfxTemplate.Inventory
{
    public static class ExtensionMethods
    {
        public static string GetName(this InventorySlot slot)
        {
            return slot?.ItemDefinition?.Value?.Name ?? string.Empty;
        }
    }
}