using Urho3DNet;

namespace RbfxTemplate.Utils
{
    /// <summary>
    /// Non-abstract wrapper for config files.
    /// This will never work as a resource because generic types can't be registered in engine reflection.
    /// Use this only via LoadConfig/SaveConfig API.
    /// </summary>
    public sealed class ConfigFileContainer<TValue> : JsonResource<TValue> where TValue : new()
    {
        public ConfigFileContainer(Context context) : base(context)
        {
        }

        /// <summary>
        /// Load resource from config folder.
        /// </summary>
        /// <param name="context">Engine context.</param>
        /// <param name="configFileName">Config file name. If null then config value type is used as a file name.</param>
        public static SharedPtr<ConfigFileContainer<TValue>> LoadConfig(Context context, string configFileName = null)
        {
            configFileName = configFileName ?? GetDefaultFileName();
            var fileId = new FileIdentifier("conf", configFileName);

            SharedPtr<ConfigFileContainer<TValue>> ptr = new ConfigFileContainer<TValue>(context);
            ptr.Ptr.Name = configFileName;
            if (context.VirtualFileSystem.Exists(fileId))
            {
                if (ptr.Ptr.LoadFile(fileId) && ptr.Ptr.Value != null)
                {
                    return ptr;
                }
            }

            ptr.Ptr.Value = new TValue();
            return ptr;
        }

        /// <summary>
        /// Save value as configuration file.
        /// </summary>
        public void SaveConfig()
        {
            var name = Name;
            if (string.IsNullOrWhiteSpace(name))
                name = GetDefaultFileName();
            var fileId = new FileIdentifier("conf", name);
            SaveFile(fileId);
        }

        private static string GetDefaultFileName()
        {
            return typeof(TValue).Name + ".json";
        }

    }
}
