using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Urho3DNet;

namespace RbfxTemplate.Utils
{
    public class JsonResource: Resource
    {
        /// <summary>
        /// Create json resource.
        /// </summary>
        /// <param name="context">Engine context.</param>
        protected JsonResource(Context context) : base(context)
        {
            if (JsonSerializerOptions == null)
            {
                JsonSerializerOptions = new JsonSerializerOptions(JsonSerializerOptions.Default)
                {
                    WriteIndented = true,
                    IgnoreReadOnlyProperties = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                };
                JsonSerializerOptions.Converters.Add(new IntVector2JsonConverter());
                JsonSerializerOptions.Converters.Add(new IntVector3JsonConverter());
                JsonSerializerOptions.Converters.Add(new IntRectJsonConverter());
                JsonSerializerOptions.Converters.Add(new Vector2JsonConverter());
                JsonSerializerOptions.Converters.Add(new Vector3JsonConverter());
                JsonSerializerOptions.Converters.Add(new Vector4JsonConverter());
                JsonSerializerOptions.Converters.Add(new RectJsonConverter());
                JsonSerializerOptions.Converters.Add(new QuaternionJsonConverter());
                JsonSerializerOptions.Converters.Add(new BoundingBoxJsonConverter());
                JsonSerializerOptions.Converters.Add(new BoundingBoxJsonConverter());
                JsonSerializerOptions.Converters.Add(new ResourceJsonConverter(context));
                JsonSerializerOptions.Converters.Add(new ResourceRefJsonConverter(context));
                JsonSerializerOptions.Converters.Add(new ResourceRefListJsonConverter(context));
                JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            }
        }

        /// <summary>
        /// Json Serializer Options.
        /// </summary>
        public static JsonSerializerOptions JsonSerializerOptions { get; private set; }

        #region IntVector* Converters
        internal class IntVector2JsonConverter : JsonConverter<IntVector2>
        {
            public override IntVector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => IntVector2.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, IntVector2 value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class IntVector3JsonConverter : JsonConverter<IntVector3>
        {
            public override IntVector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => IntVector3.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, IntVector3 value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class IntRectJsonConverter : JsonConverter<IntRect>
        {
            public override IntRect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => IntRect.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, IntRect value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        #endregion

        #region Vector* Converters
        internal class Vector2JsonConverter : JsonConverter<Vector2>
        {
            public override Vector2 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Vector2.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, Vector2 value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class Vector3JsonConverter : JsonConverter<Vector3>
        {
            public override Vector3 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Vector3.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, Vector3 value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class Vector4JsonConverter : JsonConverter<Vector4>
        {
            public override Vector4 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Vector4.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, Vector4 value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class RectJsonConverter : JsonConverter<Rect>
        {
            public override Rect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Rect.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, Rect value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }
        #endregion

        #region Other Converters
        internal class ResourceJsonConverter : JsonConverter<Resource>
        {
            private readonly Context _context;

            public override bool CanConvert(Type typeToConvert)
            {
                return typeof(Resource).IsAssignableFrom(typeToConvert);
            }

            public ResourceJsonConverter(Context context)
            {
                _context = context;
            }

            public override Resource Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                var str = reader.GetString();
                if (string.IsNullOrWhiteSpace(str))
                    return null;
                var split = str.IndexOf(';');
                if (split < 0)
                    return null;

                return _context.ResourceCache.GetResource(str.Substring(0, split), str.Substring(split + 1));
            }


            public override void Write(
                Utf8JsonWriter writer,
                Resource value,
                JsonSerializerOptions options) =>
                writer.WriteStringValue((value == null) ? "" : string.Format(CultureInfo.InvariantCulture, "{0};{1}", value.GetTypeName(), value.Name));
        }

        internal class QuaternionJsonConverter : JsonConverter<Quaternion>
        {
            public override Quaternion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Quaternion.Parse(reader.GetString());

            public override void Write(Utf8JsonWriter writer, Quaternion value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString());
        }

        internal class BoundingBoxJsonConverter : JsonConverter<BoundingBox>
        {
            private static readonly Vector3JsonConverter Vector3JsonConverter = new Vector3JsonConverter();

            public override BoundingBox Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options)
            {
                if (reader.TokenType != JsonTokenType.StartObject)
                {
                    throw new JsonException();
                }

                BoundingBox bbox = new BoundingBox(0.0f, 0.0f);
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        return bbox;
                    }

                    // Get the key.
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    string propertyName = reader.GetString();

                    // Get the value.
                    reader.Read();

                    switch (propertyName)
                    {
                        case "min": bbox.Min = Vector3JsonConverter.Read(ref reader, typeof(Vector3), options); break;
                        case "max": bbox.Max = Vector3JsonConverter.Read(ref reader, typeof(Vector3), options); break;
                        default: throw new JsonException($"Unknown property \"{propertyName}\".");
                    }
                }

                throw new JsonException();
            }

            public override void Write(
                Utf8JsonWriter writer,
                BoundingBox value,
                JsonSerializerOptions options)

            {
                writer.WriteStartObject();
                writer.WritePropertyName("min");
                Vector3JsonConverter.Write(writer, value.Min, options);
                writer.WritePropertyName("max");
                Vector3JsonConverter.Write(writer, value.Max, options);
                writer.WriteEndObject();
            }
        }

        internal class ResourceRefJsonConverter : JsonConverter<ResourceRef>
        {
            private readonly Context _context;

            public ResourceRefJsonConverter(Context context)
            {
                _context = context;
            }

            public override ResourceRef Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) =>
                ResourceRef.Parse(reader.GetString());

            public override void Write(
                Utf8JsonWriter writer,
                ResourceRef value,
                JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString(_context));
        }

        internal class ResourceRefListJsonConverter : JsonConverter<ResourceRefList>
        {
            private readonly Context _context;

            public ResourceRefListJsonConverter(Context context)
            {
                _context = context;
            }

            public override ResourceRefList Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) =>
                ResourceRefList.Parse(reader.GetString());

            public override void Write(
                Utf8JsonWriter writer,
                ResourceRefList value,
                JsonSerializerOptions options) =>
                writer.WriteStringValue(value.ToString(_context));
        }
        #endregion

    }

    /// <summary>
    /// Base class for resource files. The file contains value serialized to json via .net serialization.
    /// </summary>
    /// <typeparam name="TValue">Resource value type.</typeparam>
    public abstract class JsonResource<TValue> : JsonResource where TValue : new()
    {
        /// <summary>
        /// Stored resource value.
        /// </summary>
        private TValue _value;

        protected JsonResource(Context context) : base(context)
        {
        }

        /// <summary>
        /// Stored resource value.
        /// </summary>
        public TValue Value { get => _value; set => _value = value; }

        /// <summary>
        /// Parses the text representing a single JSON value into a <typeparamref name="TValue"/>.
        /// </summary>
        /// <param name="jsonString">JSON text to parse.</param>
        public bool DeserializeJson(string jsonString)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonString))
                    _value = default;
                else
                    _value = JsonSerializer.Deserialize<TValue>(jsonString, JsonResource.JsonSerializerOptions);
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Converts the provided value into a <see cref="string"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> representation of the value.</returns>
        public string SerializeJson()
        {
            try
            {
                if (_value == null)
                    return string.Empty;
                return JsonSerializer.Serialize(_value, JsonResource.JsonSerializerOptions);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return string.Empty;
            }
        }


        public override bool SaveFile(FileIdentifier fileName)
        {
            var vfs = Context.GetSubsystem<VirtualFileSystem>();
            var file = vfs.OpenFile(fileName, FileMode.FileWrite);
            if (file != null)
            {
                var res = Save(file);
                file.Close();
                return res;
            }
            return false;
        }

        public override bool BeginLoad(IDeserializer source)
        {
            return DeserializeJson(source.ReadString());
        }

        public override bool EndLoad()
        {
            return true;
        }

        public override bool Save(ISerializer dest)
        {
            var jsonString = SerializeJson();

            using (var binaryFile = new BinaryFile(Context))
            {
                binaryFile.Text = jsonString;
                return binaryFile.Save(dest);
            }
        }
    }
}
