using System;
using Newtonsoft.Json;
using Urho3DNet;

namespace RbfxTemplate
{
    /// <summary>
    ///     Setting file content.
    /// </summary>
    public class SettingFile
    {
        private static readonly string SOUND_MASTER = "Master";

        private static readonly string SOUND_EFFECT = "Effect";

        //static readonly string SOUND_AMBIENT = "Ambient";
        //static readonly string SOUND_VOICE = "Voice";
        private static readonly string SOUND_MUSIC = "Music";

        /// <summary>
        ///     Is bloom enabled.
        /// </summary>
        public bool Bloom { get; set; } = true;

        /// <summary>
        ///     Is SSAO enabled.
        /// </summary>
        public bool SSAO { get; set; } = true;

        /// <summary>
        ///     Get or set master volume.
        /// </summary>
        public float MasterVolume { get; set; } = 1.0f;

        /// <summary>
        ///     Get or set music volume.
        /// </summary>
        public float MusicVolume { get; set; } = 1.0f;

        /// <summary>
        ///     Get or set effects volume.
        /// </summary>
        public float EffectVolume { get; set; } = 1.0f;

        /// <summary>
        ///     Load setting file if exists.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <returns>Loaded or created settings file content.</returns>
        public static SettingFile Load(Context context)
        {
            var json = context.GetSubsystem<VirtualFileSystem>()
                .ReadAllText(new FileIdentifier("conf", "settings.json"));

            if (!string.IsNullOrWhiteSpace(json))
            {
                try
                {
                    return JsonConvert.DeserializeObject<SettingFile>(json);
                }
                catch (Exception)
                {
                }
            }

            return new SettingFile();

        }

        /// <summary>
        ///     Save setting file.
        /// </summary>
        /// <param name="context">Application context.</param>
        /// <returns>Returns true is operation succeeds.</returns>
        public bool Save(Context context)
        {
            return context.GetSubsystem<VirtualFileSystem>().WriteAllText(new FileIdentifier("conf", "settings.json"),
                JsonConvert.SerializeObject(this));
        }

        /// <summary>
        ///     Apply settings to the application global settings.
        /// </summary>
        /// <param name="context">Application context.</param>
        public void Apply(Context context)
        {
            var audio = context.GetSubsystem<Audio>();

            audio.SetMasterGain(SOUND_MASTER, MasterVolume);
            audio.SetMasterGain(SOUND_MUSIC, MusicVolume);
            audio.SetMasterGain(SOUND_EFFECT, EffectVolume);
        }

        /// <summary>
        ///     Apply settings to the scene's render pipeline settings.
        /// </summary>
        /// <param name="renderPipeline">Render pipeline component.</param>
        public void Apply(RenderPipeline renderPipeline)
        {
            var settings = renderPipeline.Settings;
            settings.Bloom.Enabled = Bloom;
            settings.Ssao.Enabled = SSAO;

            renderPipeline.Settings = settings;
        }
    }
}