#nullable enable
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using WolvenKit.RED4.Archive.Buffer;
using WolvenKit.RED4.Archive.CR2W;
using WolvenKit.RED4.Types;

namespace WolvenKit.RED4.CR2W.JSON;

public interface ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options);
}

#region Fundmentals

public class CBoolConverter : JsonConverter<CBool>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CBool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetByte();

    public override void Write(Utf8JsonWriter writer, CBool value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CDoubleConverter : JsonConverter<CDouble>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CDouble Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetDouble();

    public override void Write(Utf8JsonWriter writer, CDouble value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CFloatConverter : JsonConverter<CFloat>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CFloat Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            var str = reader.GetString();
            if (str == "+inf")
            {
                return float.PositiveInfinity;
            }
            else if (str == "-inf")
            {
                return float.NegativeInfinity;
            }
            else
            {
                throw new JsonException();
            }
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetSingle();
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, CFloat value, JsonSerializerOptions options)
    {
        if (float.IsPositiveInfinity(value))
        {
            writer.WriteStringValue("+inf");
        }
        else if (float.IsNegativeInfinity(value))
        {
            writer.WriteStringValue("-inf");
        }
        else
        {
            writer.WriteNumberValue(value);
        }
    }
}

public class CInt8Converter : JsonConverter<CInt8>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CInt8 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetSByte();

    public override void Write(Utf8JsonWriter writer, CInt8 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CInt16Converter : JsonConverter<CInt16>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CInt16 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt16();

    public override void Write(Utf8JsonWriter writer, CInt16 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CInt32Converter : JsonConverter<CInt32>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt32();

    public override void Write(Utf8JsonWriter writer, CInt32 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CInt64Converter : JsonConverter<CInt64>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CInt64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetInt64();

    public override void Write(Utf8JsonWriter writer, CInt64 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CUInt8Converter : JsonConverter<CUInt8>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CUInt8 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetByte();

    public override void Write(Utf8JsonWriter writer, CUInt8 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CUInt16Converter : JsonConverter<CUInt16>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CUInt16 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetUInt16();

    public override void Write(Utf8JsonWriter writer, CUInt16 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CUInt32Converter : JsonConverter<CUInt32>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CUInt32 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetUInt32();

    public override void Write(Utf8JsonWriter writer, CUInt32 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CUInt64Converter : JsonConverter<CUInt64>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CUInt64 Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetUInt64();

    public override void Write(Utf8JsonWriter writer, CUInt64 value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

#endregion Fundmentals


#region Simples

public class CDateTimeConverter : JsonConverter<CDateTime>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CDateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetUInt64();

    public override void Write(Utf8JsonWriter writer, CDateTime value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CGuidConverter : JsonConverter<CGuid>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CGuid Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetBytesFromBase64();

    public override void Write(Utf8JsonWriter writer, CGuid value, JsonSerializerOptions options) => writer.WriteBase64StringValue((byte[])value);
}

public class CNameConverter : JsonConverter<CName>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CName? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetUInt64();
        }
        else
        {
            throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, CName value, JsonSerializerOptions options)
    {
        if ((string)value != null)
        {
            writer.WriteStringValue(value);
        }
        else
        {
            writer.WriteNumberValue(value);
        }
    }
}

public class CRUIDConverter : JsonConverter<CRUID>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CRUID Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => reader.GetUInt64();

    public override void Write(Utf8JsonWriter writer, CRUID value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
}

public class CStringConverter : JsonConverter<CString>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, CString value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}

public class CVariantConverter : JsonConverter<CVariant>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CVariant? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        Type? type = null;
        IRedType? result = null;

        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                if (result == null)
                {
                    throw new JsonException();
                }

                return new CVariant { Value = result };
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            reader.Read();

            switch (propertyName)
            {
                case "Type":
                {
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException();
                    }

                    (type, _) = RedReflection.GetCSTypeFromRedType(reader.GetString());
                    break;
                }

                case "Value":
                {
                    if (type == null)
                    {
                        throw new JsonException();
                    }

                    var converter = options.GetConverter(type);
                    if (converter is ICustomRedConverter conv)
                    {
                        result = (IRedType?)conv.ReadRedType(ref reader, type, options);
                    }
                    else
                    {
                        result = (IRedType?)JsonSerializer.Deserialize(ref reader, type, options);
                    }

                    break;
                }

                default:
                {
                    throw new JsonException();
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, CVariant value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var redTypeName = RedReflection.GetRedTypeFromCSType(value.Value.GetType());
        writer.WriteString("Type", redTypeName);

        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, (object)value.Value, options);

        writer.WriteEndObject();
    }
}

public class BufferConverterFactory : JsonConverterFactory
{
    private readonly ReferenceResolver<RedBuffer> _referenceResolver;

    public BufferConverterFactory(ReferenceResolver<RedBuffer> bufferResolver)
    {
        _referenceResolver = bufferResolver;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedBufferWrapper).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(DataBuffer))
        {
            return new DataBufferConverter(_referenceResolver);
        }

        if (typeToConvert == typeof(SerializationDeferredDataBuffer))
        {
            return new SerializationDeferredDataBufferConverter(_referenceResolver);
        }

        if (typeToConvert == typeof(SharedDataBuffer))
        {
            return new SharedDataBufferConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class DataBufferConverter : JsonConverter<DataBuffer>, ICustomRedConverter
    {
        private readonly ReferenceResolver<RedBuffer> _referenceResolver;

        public DataBufferConverter(ReferenceResolver<RedBuffer> referenceResolver)
        {
            _referenceResolver = referenceResolver;
        }


        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override DataBuffer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string? id = null;
            var val = new DataBuffer();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "BufferId":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        id = reader.GetString();
                        if (id == null)
                        {
                            throw new JsonException();
                        }

                        break;
                    }

                    case "BufferRefId":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var refId = reader.GetString();
                        if (refId == null)
                        {
                            throw new JsonException();
                        }

                        val.Buffer = _referenceResolver.ResolveReference(refId);

                        break;
                    }

                    case "Data":
                    {
                        var converter = options.GetConverter(typeof(Package04));
                        if (converter is ICustomRedConverter conv)
                        {
                            reader.Read();
                            val.Data = (IParseableBuffer?)conv.ReadRedType(ref reader, typeof(Package04), options);
                        }
                        else
                        {
                            throw new JsonException();
                        }
                        
                        break;
                    }

                    case "Flags":
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        var flags = reader.GetUInt32();

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException();
                        }

                        propertyName = reader.GetString();
                        if (propertyName != "Bytes")
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var bytes = reader.GetBytesFromBase64();
                        val.Buffer = RedBuffer.CreateBuffer(flags, bytes);

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            if (id != null)
            {
                _referenceResolver.AddReference(id, val.Buffer);
            }

            return val;
        }

        public override void Write(Utf8JsonWriter writer, DataBuffer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var refId = _referenceResolver.GetReference(value.Buffer, out var alreadyExists);
            if (alreadyExists)
            {
                writer.WriteString("BufferRefId", refId);
            }
            else
            {
                writer.WriteString("BufferId", refId);

                if (value.Buffer.Data is Package04 pkg)
                {
                    writer.WritePropertyName("Data");
                    JsonSerializer.Serialize(writer, pkg, options);
                }
                else
                {
                    writer.WriteNumber("Flags", value.Buffer.Flags);
                    writer.WritePropertyName("Bytes");
                    writer.WriteBase64StringValue(value.Buffer.GetBytes());
                }
            }

            writer.WriteEndObject();
        }
    }

    private class SerializationDeferredDataBufferConverter : JsonConverter<SerializationDeferredDataBuffer>, ICustomRedConverter
    {
        private readonly ReferenceResolver<RedBuffer> _referenceResolver;

        public SerializationDeferredDataBufferConverter(ReferenceResolver<RedBuffer> referenceResolver)
        {
            _referenceResolver = referenceResolver;
        }


        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override SerializationDeferredDataBuffer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            string? id = null;
            var val = new SerializationDeferredDataBuffer();

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    break;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "BufferId":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        id = reader.GetString();
                        if (id == null)
                        {
                            throw new JsonException();
                        }

                        break;
                    }

                    case "BufferRefId":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var refId = reader.GetString();
                        if (refId == null)
                        {
                            throw new JsonException();
                        }

                        val.Buffer = _referenceResolver.ResolveReference(refId);

                        break;
                    }

                    case "Data":
                    {
                        var converter = options.GetConverter(typeof(Package04));
                        if (converter is ICustomRedConverter conv)
                        {
                            reader.Read();
                            val.Data = (IParseableBuffer?)conv.ReadRedType(ref reader, typeof(Package04), options);
                        }
                        else
                        {
                            throw new JsonException();
                        }

                        break;
                    }

                    case "Flags":
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        var flags = reader.GetUInt32();

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.PropertyName)
                        {
                            throw new JsonException();
                        }

                        propertyName = reader.GetString();
                        if (propertyName != "Bytes")
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var bytes = reader.GetBytesFromBase64();
                        val.Buffer = RedBuffer.CreateBuffer(flags, bytes);

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            if (id != null)
            {
                _referenceResolver.AddReference(id, val.Buffer);
            }

            return val;
        }

        public override void Write(Utf8JsonWriter writer, SerializationDeferredDataBuffer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var refId = _referenceResolver.GetReference(value.Buffer, out var alreadyExists);
            if (alreadyExists)
            {
                writer.WriteString("BufferRefId", refId);
            }
            else
            {
                writer.WriteString("BufferId", refId);

                if (value.Buffer.Data is Package04 pkg)
                {
                    writer.WritePropertyName("Data");
                    JsonSerializer.Serialize(writer, pkg, options);
                }
                else
                {
                    writer.WriteNumber("Flags", value.Buffer.Flags);
                    writer.WritePropertyName("Bytes");
                    writer.WriteBase64StringValue(value.Buffer.GetBytes());
                }
            }

            writer.WriteEndObject();
        }
    }

    private class SharedDataBufferConverter : JsonConverter<SharedDataBuffer>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override SharedDataBuffer? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            reader.Read();

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            reader.Read();

            var val = new SharedDataBuffer();
            switch (propertyName)
            {
                case "File":
                {
                    val.File = JsonSerializer.Deserialize<CR2WFile>(ref reader, options);
                    break;
                }

                case "Data":
                {
                    var converter = options.GetConverter(typeof(Package04));
                    if (converter is ICustomRedConverter conv)
                    {
                        reader.Read();
                        val.Data = (IParseableBuffer?)conv.ReadRedType(ref reader, typeof(Package04), options);
                    }
                    else
                    {
                        throw new JsonException();
                    }

                    break;
                }

                case "Flags":
                {
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    var flags = reader.GetUInt32();

                    reader.Read();
                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        throw new JsonException();
                    }

                    propertyName = reader.GetString();
                    if (propertyName != "Bytes")
                    {
                        throw new JsonException();
                    }

                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException();
                    }

                    var bytes = reader.GetBytesFromBase64();
                    val.Buffer = RedBuffer.CreateBuffer(flags, bytes);

                    break;
                }

                default:
                {
                    throw new JsonException();
                }
            }

            reader.Read();
            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return val;
        }

        public override void Write(Utf8JsonWriter writer, SharedDataBuffer value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            if (value.File is CR2WFile file)
            {
                writer.WritePropertyName("File");
                JsonSerializer.Serialize(writer, file, options);
            }
            else if (value.Data is Package04 pkg)
            {
                writer.WritePropertyName("Data");
                JsonSerializer.Serialize(writer, pkg, options);
            }
            else
            {
                writer.WriteNumber("Flags", value.Buffer.Flags);
                writer.WritePropertyName("Bytes");
                writer.WriteBase64StringValue(value.Buffer.GetBytes());
            }

            writer.WriteEndObject();
        }
    }
}

public class LocalizationStringConverter : JsonConverter<LocalizationString>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override LocalizationString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new LocalizationString();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            switch (propertyName)
            {
                case "unk1":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    result.Unk1 = reader.GetUInt64();
                    break;
                }

                case "value":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.String)
                    {
                        throw new JsonException();
                    }

                    result.Value = reader.GetString();
                    break;
                }

                default:
                {
                    throw new JsonException();
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, LocalizationString value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("unk1", value.Unk1);
        writer.WriteString("value", value.Value);

        writer.WriteEndObject();
    }
}

public class CLegacySingleChannelCurveConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedLegacySingleChannelCurve).IsAssignableFrom(typeToConvert) || typeof(IRedCurvePoint).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(IRedLegacySingleChannelCurve).IsAssignableFrom(typeToConvert))
        {
            return new CLegacySingleChannelCurveConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class CLegacySingleChannelCurveConverter : JsonConverter<IRedLegacySingleChannelCurve>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedLegacySingleChannelCurve? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = (IRedLegacySingleChannelCurve?)RedTypeManager.CreateRedType(typeToConvert);
            if (result == null)
            {
                throw new JsonException();
            }

            var (elementType, _) = RedReflection.GetCSTypeFromRedType(result.ElementType);

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "InterpolationType":
                    {
                        if (result == null)
                        {
                            throw new JsonException();
                        }

                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var enumStr = reader.GetString();
                        if (enumStr == null)
                        {
                            throw new JsonException();
                        }

                        result.InterpolationType = Enum.Parse<Enums.EInterpolationType>(enumStr);

                        break;
                    }

                    case "LinkType":
                    {
                        if (result == null)
                        {
                            throw new JsonException();
                        }

                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var enumStr = reader.GetString();
                        if (enumStr == null)
                        {
                            throw new JsonException();
                        }

                        result.LinkType = Enum.Parse<Enums.ESegmentsLinkType>(enumStr);

                        break;
                    }

                    case "Elements":
                    {
                        if (result == null)
                        {
                            throw new JsonException();
                        }

                        if (reader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException();
                        }

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                break;
                            }

                            if (reader.TokenType != JsonTokenType.StartObject)
                            {
                                throw new JsonException();
                            }

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException();
                            }

                            propertyName = reader.GetString();
                            if (propertyName != "Point")
                            {
                                throw new JsonException();
                            }
                            reader.Read();

                            if (reader.TokenType != JsonTokenType.Number)
                            {
                                throw new JsonException();
                            }
                            var point = reader.GetSingle();

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException();
                            }

                            propertyName = reader.GetString();
                            if (propertyName != "Value")
                            {
                                throw new JsonException();
                            }

                            reader.Read();

                            object? value;
                            var converter = options.GetConverter(elementType);
                            if (converter is ICustomRedConverter conv)
                            {
                                value = conv.ReadRedType(ref reader, elementType, options);
                            }
                            else
                            {
                                value = JsonSerializer.Deserialize(ref reader, elementType, options);
                            }
                            
                            if (value == null)
                            {
                                throw new JsonException();
                            }

                            reader.Read();
                            if (reader.TokenType != JsonTokenType.EndObject)
                            {
                                throw new JsonException();
                            }

                            result.Add(point, value);
                        }

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedLegacySingleChannelCurve value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString("InterpolationType", value.InterpolationType.ToString());
            writer.WriteString("LinkType", value.LinkType.ToString());

            writer.WritePropertyName("Elements");
            writer.WriteStartArray();
            foreach (var point in value)
            {
                writer.WriteStartObject();

                writer.WriteNumber("Point", point.GetPoint());

                writer.WritePropertyName("Value");
                JsonSerializer.Serialize(writer, (object)point.GetValue(), options);

                writer.WriteEndObject();
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}

public class MultiChannelCurveConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedMultiChannelCurve).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(IRedMultiChannelCurve).IsAssignableFrom(typeToConvert))
        {
            return new MultiChannelCurveConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class MultiChannelCurveConverter : JsonConverter<IRedMultiChannelCurve>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedMultiChannelCurve? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = (IRedMultiChannelCurve?)RedTypeManager.CreateRedType(typeToConvert);
            if (result == null)
            {
                throw new JsonException();
            }

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                reader.Read();

                switch (propertyName)
                {
                    case "NumChannels":
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        result.NumChannels = reader.GetUInt32();

                        break;
                    }

                    case "InterpolationType":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var enumStr = reader.GetString();
                        if (enumStr == null)
                        {
                            throw new JsonException();
                        }

                        result.InterpolationType = Enum.Parse<Enums.EInterpolationType>(enumStr);

                        break;
                    }

                    case "LinkType":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var enumStr = reader.GetString();
                        if (enumStr == null)
                        {
                            throw new JsonException();
                        }

                        result.LinkType = Enum.Parse<Enums.EChannelLinkType>(enumStr);

                        break;
                    }

                    case "Alignment":
                    {
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        result.Alignment = reader.GetUInt32();

                        break;
                    }

                    case "Data":
                    {
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        result.Data = reader.GetBytesFromBase64();

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedMultiChannelCurve value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("NumChannels", value.NumChannels);
            writer.WriteString("InterpolationType", value.InterpolationType.ToString());
            writer.WriteString("LinkType", value.LinkType.ToString());
            writer.WriteNumber("Alignment", value.Alignment);
            writer.WriteBase64String("Data", value.Data);

            writer.WriteEndObject();
        }
    }
}

public class NodeRefConverter : JsonConverter<NodeRef>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override NodeRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetString();
    }

    public override void Write(Utf8JsonWriter writer, NodeRef value, JsonSerializerOptions options) => writer.WriteStringValue(value);
}

public class TweakDBIDConverter : JsonConverter<TweakDBID>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override TweakDBID? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            return reader.GetString();
        }
        else if (reader.TokenType == JsonTokenType.Number)
        {
            return reader.GetUInt64();
        }
        else
        {
            throw new JsonException();
        }
    }

    public override void Write(Utf8JsonWriter writer, TweakDBID value, JsonSerializerOptions options)
    {
        if ((string)value != null)
        {
            writer.WriteStringValue(value);
        }
        else
        {
            writer.WriteNumberValue(value);
        }
    }
}

#endregion Simples

#region Internal

public class CByteArrayConverter : JsonConverter<CByteArray>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CByteArray? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        return reader.GetBytesFromBase64();
    }

    public override void Write(Utf8JsonWriter writer, CByteArray value, JsonSerializerOptions options) => writer.WriteBase64StringValue((byte[])value);
}

public class CKeyValuePairConverter : JsonConverter<CKeyValuePair>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CKeyValuePair? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = reader.GetString();
        if (propertyName != "Type")
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var (valType, _) = RedReflection.GetCSTypeFromRedType(reader.GetString());
        if (valType == null)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        propertyName = reader.GetString();

        object? result;
        var converter = options.GetConverter(valType);
        if (converter is ICustomRedConverter conv)
        {
            result = conv.ReadRedType(ref reader, valType, options);
        }
        else
        {
            result = JsonSerializer.Deserialize(ref reader, valType, options);
        }
        
        if (result is not IRedType val)
        {
            throw new JsonException();
        }

        reader.Read();
        if (reader.TokenType != JsonTokenType.EndObject)
        {
            throw new JsonException();
        }

        return new CKeyValuePair(propertyName, val);
    }

    public override void Write(Utf8JsonWriter writer, CKeyValuePair value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        var valType = RedReflection.GetRedTypeFromCSType(value.Value.GetType());
        writer.WriteString("Type", valType);

        writer.WritePropertyName(value.Key);
        JsonSerializer.Serialize(writer, (object)value.Value, options);

        writer.WriteEndObject();
    }
}

#endregion Internal

public class ArrayConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedArray).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.GetGenericTypeDefinition() == typeof(CArray<>))
        {
            return new CArrayConverter();
        }

        if (typeToConvert.GetGenericTypeDefinition() == typeof(CArrayFixedSize<>))
        {
            return new CArrayFixedSizeConverter();
        }

        if (typeToConvert.GetGenericTypeDefinition() == typeof(CStatic<>))
        {
            return new CStaticConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class CArrayConverter : JsonConverter<IRedArray>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedArray? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartArray)
            {
                throw new JsonException();
            }

            var arr = (IRedArray)RedTypeManager.CreateRedType(typeToConvert);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndArray)
                {
                    return arr;
                }

                object? val;
                var converter = options.GetConverter(arr.InnerType);
                if (converter is ICustomRedConverter conv)
                {
                    val = conv.ReadRedType(ref reader, arr.InnerType, options);
                }
                else
                {
                    val = JsonSerializer.Deserialize(ref reader, arr.InnerType, options);
                }

                arr.Add(val);
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedArray value, JsonSerializerOptions options)
        {
            writer.WriteStartArray();
            foreach (var entry in value)
            {
                JsonSerializer.Serialize(writer, entry, options);
            }
            writer.WriteEndArray();
        }
    }

    private class CArrayFixedSizeConverter : JsonConverter<IRedArray>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedArray? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            IRedArray? arr = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (arr == null)
                    {
                        throw new JsonException();
                    }

                    return arr;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Size":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        var size = reader.GetInt32();
                        arr = (IRedArray)RedTypeManager.CreateRedType(typeToConvert, size);

                        break;
                    }

                    case "Elements":
                    {
                        if (arr == null)
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException();
                        }

                        var counter = 0;
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                break;
                            }

                            var converter = options.GetConverter(arr.InnerType);
                            if (converter is ICustomRedConverter conv)
                            {
                                arr[counter++] = conv.ReadRedType(ref reader, arr.InnerType, options);
                            }
                            else
                            {
                                arr[counter++] = JsonSerializer.Deserialize(ref reader, arr.InnerType, options);
                            }
                        }

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedArray value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Size", value.Count);

            writer.WritePropertyName("Elements");
            writer.WriteStartArray();
            foreach (var entry in value)
            {
                JsonSerializer.Serialize(writer, entry, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }

    private class CStaticConverter : JsonConverter<IRedArray>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedArray? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            IRedArray? arr = null;
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    if (arr == null)
                    {
                        throw new JsonException();
                    }

                    return arr;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Size":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        var size = reader.GetInt32();
                        arr = (IRedArray)RedTypeManager.CreateRedType(typeToConvert, size);

                        break;
                    }

                    case "MaxSize":
                    {
                        if (arr == null)
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.Number)
                        {
                            throw new JsonException();
                        }

                        arr.MaxSize = reader.GetInt32();

                        break;
                    }

                    case "Elements":
                    {
                        if (arr == null)
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.StartArray)
                        {
                            throw new JsonException();
                        }

                        var counter = 0;
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndArray)
                            {
                                break;
                            }

                            var converter = options.GetConverter(arr.InnerType);
                            if (converter is ICustomRedConverter conv)
                            {
                                arr[counter++] = conv.ReadRedType(ref reader, arr.InnerType, options);
                            }
                            else
                            {
                                arr[counter++] = JsonSerializer.Deserialize(ref reader, arr.InnerType, options);
                            }
                        }

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedArray value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("Size", value.Count);
            writer.WriteNumber("MaxSize", value.MaxSize);

            writer.WritePropertyName("Elements");
            writer.WriteStartArray();
            foreach (var entry in value)
            {
                JsonSerializer.Serialize(writer, entry, options);
            }
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}

public class EnumConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedBitField).IsAssignableFrom(typeToConvert) || typeof(IRedEnum).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(IRedBitField).IsAssignableFrom(typeToConvert))
        {
            return new CBitFieldConverter();
        }

        if (typeof(IRedEnum).IsAssignableFrom(typeToConvert))
        {
            return new CEnumConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class CBitFieldConverter : JsonConverter<IRedBitField>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedBitField? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var enumType = typeToConvert.GetGenericArguments()[0];
            var str = reader.GetString();
            return CBitField.Parse(enumType, str);
        }

        public override void Write(Utf8JsonWriter writer, IRedBitField value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToBitFieldString());
        }
    }

    private class CEnumConverter : JsonConverter<IRedEnum>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedEnum? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.String)
            {
                throw new JsonException();
            }

            var enumType = typeToConvert.GetGenericArguments()[0];
            var str = reader.GetString();
            return CEnum.Parse(enumType, str);
        }

        public override void Write(Utf8JsonWriter writer, IRedEnum value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToEnumString());
        }
    }
}

public class HandleConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedBaseHandle).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.GetGenericTypeDefinition() == typeof(CHandle<>))
        {
            return new HandleConverter();
        }

        if (typeToConvert.GetGenericTypeDefinition() == typeof(CWeakHandle<>))
        {
            return new HandleConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class HandleConverter : JsonConverter<IRedBaseHandle>, ICustomRedConverter
    {
        public override bool HandleNull => true;

        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedBaseHandle? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            var handle = (IRedBaseHandle)RedTypeManager.CreateRedType(typeToConvert);
            if (reader.TokenType == JsonTokenType.Number)
            {
                if (reader.GetInt32() == -1)
                {
                    return handle;
                }
                throw new JsonException();
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var converter = options.GetConverter(typeof(RedBaseClass));
            if (converter is ICustomRedConverter conv)
            {
                handle.SetValue((RedBaseClass?)conv.ReadRedType(ref reader, typeof(RedBaseClass), options));
            }
            else
            {
                throw new JsonException();
            }

            if (reader.TokenType != JsonTokenType.EndObject)
            {
                throw new JsonException();
            }

            return handle;
        }

        public override void Write(Utf8JsonWriter writer, IRedBaseHandle? value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
            }
            else
            {
                var cls = value.GetValue();
                if (cls != null)
                {
                    JsonSerializer.Serialize(writer, cls, options);
                }
                else
                {
                    writer.WriteNumberValue(-1);
                }
            }
        }
    }
}

public class ResourceConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeof(IRedResourceReference).IsAssignableFrom(typeToConvert) || typeof(IRedResourceAsyncReference).IsAssignableFrom(typeToConvert);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeof(IRedResourceReference).IsAssignableFrom(typeToConvert))
        {
            return new ResourceReferenceConverter();
        }

        if (typeof(IRedResourceAsyncReference).IsAssignableFrom(typeToConvert))
        {
            return new ResourceReferenceConverter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class ResourceReferenceConverter : JsonConverter<IRedRef>, ICustomRedConverter
    {
        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override IRedRef? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            var result = (IRedRef)RedTypeManager.CreateRedType(typeToConvert);
            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return result;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "DepotPath":
                    {
                        var converter = options.GetConverter(typeof(CName));
                        if (converter is ICustomRedConverter conv)
                        {
                            reader.Read();
                            result.DepotPath = (CName?)conv.ReadRedType(ref reader, typeof(CName), options);
                        }
                        else
                        {
                            throw new JsonException();
                        }
                        
                        break;
                    }

                    case "Flags":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var str = reader.GetString();
                        if (str == null)
                        {
                            throw new JsonException();
                        }

                        result.Flags = Enum.Parse<InternalEnums.EImportFlags>(str);
                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, IRedRef value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("DepotPath");
            JsonSerializer.Serialize(writer, value.DepotPath, options);

            writer.WriteString("Flags", value.Flags.ToString());

            writer.WriteEndObject();
        }
    }
}

public class ClassConverterFactory : JsonConverterFactory
{
    private readonly ReferenceResolver<RedBaseClass> _referenceResolver;

    public ClassConverterFactory(ReferenceResolver<RedBaseClass> classResolver)
    {
        _referenceResolver = classResolver;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsSubclassOf(typeof(RedBaseClass)) || typeToConvert == typeof(RedBaseClass);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert.IsSubclassOf(typeof(RedBaseClass)) || typeToConvert == typeof(RedBaseClass))
        {
            return new RedBaseClassConverter(_referenceResolver);
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }

    private class RedBaseClassConverter : JsonConverter<RedBaseClass>, ICustomRedConverter
    {
        private readonly ReferenceResolver<RedBaseClass> _referenceResolver;

        public RedBaseClassConverter(ReferenceResolver<RedBaseClass> referenceResolver)
        {
            _referenceResolver = referenceResolver;
        }


        public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

        public override RedBaseClass? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.Null)
            {
                return null;
            }

            if (reader.TokenType != JsonTokenType.StartObject)
            {
                throw new JsonException();
            }

            RedBaseClass? cls = null;
            RedReflection.ExtendedTypeInfo? typeInfo = null;

            string? clsType;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return cls;
                }

                if (reader.TokenType != JsonTokenType.PropertyName)
                {
                    throw new JsonException();
                }

                var propertyName = reader.GetString();
                switch (propertyName)
                {
                    case "Type":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        clsType = reader.GetString();
                        cls = RedTypeManager.Create(clsType);
                        typeInfo = RedReflection.GetTypeInfo(cls.GetType());

                        break;
                    }

                    case "Id":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var id = reader.GetString();
                        if (id == null || cls == null)
                        {
                            throw new JsonException();
                        }

                        _referenceResolver.AddReference(id, cls);

                        break;
                    }

                    case "RefId":
                    {
                        reader.Read();
                        if (reader.TokenType != JsonTokenType.String)
                        {
                            throw new JsonException();
                        }

                        var refId = reader.GetString();
                        if (refId == null)
                        {
                            throw new JsonException();
                        }

                        cls = _referenceResolver.ResolveReference(refId);

                        break;
                    }

                    case "Properties":
                    {
                        if (cls == null || typeInfo == null)
                        {
                            throw new JsonException();
                        }

                        reader.Read();
                        if (reader.TokenType != JsonTokenType.StartObject)
                        {
                            throw new JsonException();
                        }

                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonTokenType.EndObject)
                            {
                                break;
                            }

                            if (reader.TokenType != JsonTokenType.PropertyName)
                            {
                                throw new JsonException();
                            }

                            var key = reader.GetString();
                            if (key == null)
                            {
                                throw new JsonException();
                            }

                            var valInfo = typeInfo.PropertyInfos.FirstOrDefault(x => x.RedName == key);
                            if (valInfo == null)
                            {
                                throw new JsonException();
                            }

                            object? val;
                            var converter = options.GetConverter(valInfo.Type);
                            if (converter is ICustomRedConverter conv)
                            {
                                reader.Read();
                                val = conv.ReadRedType(ref reader, valInfo.Type, options);
                            }
                            else
                            {
                                val = JsonSerializer.Deserialize(ref reader, valInfo.Type, options);
                            }

                            if (val == null)
                            {
                                var propTypeInfo = RedReflection.GetTypeInfo(valInfo.Type);
                                if (propTypeInfo.IsValueType)
                                {
                                    val = RedTypeManager.CreateRedType(valInfo.Type);
                                }
                            }

                            valInfo.SetValue(cls, (IRedType?)val);
                        }

                        break;
                    }

                    default:
                    {
                        throw new JsonException();
                    }
                }
            }

            throw new JsonException();
        }

        public override void Write(Utf8JsonWriter writer, RedBaseClass value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            var refId = _referenceResolver.GetReference(value, out var alreadyExists);
            if (alreadyExists)
            {
                writer.WriteString("RefId", refId);
            }
            else
            {
                writer.WriteString("Type", RedReflection.GetRedTypeFromCSType(value.GetType()));

                writer.WriteString("Id", refId);

                writer.WritePropertyName("Properties");
                writer.WriteStartObject();

                var typeInfo = RedReflection.GetTypeInfo(value.GetType());
                foreach (var propertyInfo in typeInfo.PropertyInfos.OrderBy(x => x.RedName))
                {
                    writer.WritePropertyName(propertyInfo.RedName);
                    JsonSerializer.Serialize(writer, propertyInfo.GetValue(value), options);
                }

                writer.WriteEndObject();
            }

            writer.WriteEndObject();
        }
    }
}

public class Red4FileConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsSubclassOf(typeof(Red4File)) || typeToConvert == typeof(Red4File);
    }

    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (typeToConvert == typeof(CR2WFile))
        {
            return new CR2WFileConverter();
        }

        if (typeToConvert == typeof(Package04))
        {
            return new Package04Converter();
        }

        throw new NotSupportedException("CreateConverter got called on a type that this converter factory doesn't support");
    }
}

public class CR2WFileConverter : JsonConverter<CR2WFile>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override CR2WFile? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new CR2WFile();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            switch (propertyName)
            {
                case "RootChunk":
                {
                    var converter = options.GetConverter(typeof(RedBaseClass));
                    if (converter is ICustomRedConverter conv)
                    {
                        reader.Read();
                        result.RootChunk = (RedBaseClass?)conv.ReadRedType(ref reader, typeof(RedBaseClass), options);
                    }
                    else
                    {
                        throw new JsonException();
                    }
                    
                    break;
                }

                case "EmbeddedFiles":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException();
                    }

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        var converter = options.GetConverter(typeof(CR2WEmbedded));
                        if (converter is ICustomRedConverter conv)
                        {
                            reader.Read();
                            result.EmbeddedFiles.Add((CR2WEmbedded?)conv.ReadRedType(ref reader, typeof(CR2WEmbedded), options));
                        }
                        else
                        {
                            throw new JsonException();
                        }
                    }

                    break;
                }

                default:
                {
                    throw new JsonException();
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, CR2WFile value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WritePropertyName("RootChunk");
        JsonSerializer.Serialize(writer, value.RootChunk, options);

        writer.WritePropertyName("EmbeddedFiles");
        writer.WriteStartArray();
        foreach (var embeddedFile in value.EmbeddedFiles)
        {
            JsonSerializer.Serialize(writer, embeddedFile, options);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}

public class Package04Converter : JsonConverter<Package04>, ICustomRedConverter
{
    public object? ReadRedType(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => Read(ref reader, typeToConvert, options);

    public override Package04? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        var result = new Package04();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                return result;
            }

            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }

            var propertyName = reader.GetString();
            switch (propertyName)
            {
                case "Version":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    result.Version = reader.GetUInt16();
                    break;
                }

                case "Sections":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    result.Sections = reader.GetUInt16();
                    break;
                }

                case "CruidIndex":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.Number)
                    {
                        throw new JsonException();
                    }

                    result.CruidIndex = reader.GetInt16();
                    break;
                }

                case "RootCruids":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException();
                    }

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        result.RootCruids.Add(reader.GetUInt64());
                    }

                    break;
                }

                case "Chunks":
                {
                    reader.Read();
                    if (reader.TokenType != JsonTokenType.StartArray)
                    {
                        throw new JsonException();
                    }

                    result.Chunks = new List<RedBaseClass>();
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        var converter = options.GetConverter(typeof(RedBaseClass));
                        if (converter is ICustomRedConverter conv)
                        {
                            reader.Read();
                            result.Chunks.Add((RedBaseClass?)conv.ReadRedType(ref reader, typeof(RedBaseClass), options));
                        }
                        else
                        {
                            throw new JsonException();
                        }
                    }

                    break;
                }

                default:
                {
                    throw new JsonException();
                }
            }
        }

        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Package04 value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        writer.WriteNumber("Version", value.Version);
        writer.WriteNumber("Sections", value.Sections);
        writer.WriteNumber("CruidIndex", value.CruidIndex);

        writer.WritePropertyName("RootCruids");
        writer.WriteStartArray();
        foreach (var cruid in value.RootCruids)
        {
            writer.WriteNumberValue(cruid);
        }
        writer.WriteEndArray();

        writer.WritePropertyName("Chunks");
        writer.WriteStartArray();
        foreach (var chunk in value.Chunks)
        {
            JsonSerializer.Serialize(writer, chunk, options);
        }
        writer.WriteEndArray();

        writer.WriteEndObject();
    }
}

public class ReferenceResolver<T>  where T : class
{
    private ConcurrentDictionary<int, uint> _threadedReferenceCount = new();

    private readonly ConcurrentDictionary<int, Dictionary<string, T>> _threadedReferenceIdToObjectMap = new();
    private readonly ConcurrentDictionary<int, Dictionary<T, string>> _threadedObjectToReferenceIdMap = new();

    public void Begin()
    {
        _threadedReferenceCount.TryAdd(Environment.CurrentManagedThreadId, 0);
        _threadedReferenceIdToObjectMap.TryAdd(Environment.CurrentManagedThreadId, new Dictionary<string, T>());
        _threadedObjectToReferenceIdMap.TryAdd(Environment.CurrentManagedThreadId, new Dictionary<T, string>(ReferenceEqualityComparer.Instance));
    }

    public void End()
    {
        _threadedReferenceCount.TryRemove(Environment.CurrentManagedThreadId, out _);
        _threadedReferenceIdToObjectMap.TryRemove(Environment.CurrentManagedThreadId, out _);
        _threadedObjectToReferenceIdMap.TryRemove(Environment.CurrentManagedThreadId, out _);
    }

    private uint GetNextId() => _threadedReferenceCount[Environment.CurrentManagedThreadId]++;

    private Dictionary<string, T> ReferenceIdToObjectMap => _threadedReferenceIdToObjectMap[Environment.CurrentManagedThreadId];
    private Dictionary<T, string> ObjectToReferenceIdMap => _threadedObjectToReferenceIdMap[Environment.CurrentManagedThreadId];

    public void AddReference(string referenceId, T value)
    {
        if (!ReferenceIdToObjectMap.TryAdd(referenceId, value))
        {
            throw new JsonException();
        }
    }

    public string GetReference(T value, out bool alreadyExists)
    {
        if (ObjectToReferenceIdMap.TryGetValue(value, out var referenceId))
        {
            alreadyExists = true;
        }
        else
        {
            referenceId = GetNextId().ToString();
            ObjectToReferenceIdMap.Add(value, referenceId);
            alreadyExists = false;
        }

        return referenceId;
    }

    public T ResolveReference(string referenceId)
    {
        if (!ReferenceIdToObjectMap.TryGetValue(referenceId, out var value))
        {
            throw new JsonException();
        }

        return value;
    }
}


public static class RedJsonSerializer
{
    private static JsonSerializerOptions s_settings;

    private static readonly ReferenceResolver<RedBuffer> s_bufferResolver;
    private static readonly ReferenceResolver<RedBaseClass> s_classResolver;

    static RedJsonSerializer()
    {
        s_bufferResolver = new();
        s_classResolver = new();

        s_settings = new()
        {
            WriteIndented = true,
            MaxDepth = 256,
            Converters =
            {
                new CBoolConverter(),
                new CDoubleConverter(),
                new CFloatConverter(),
                new CInt8Converter(),
                new CInt16Converter(),
                new CInt32Converter(),
                new CInt64Converter(),
                new CUInt8Converter(),
                new CUInt16Converter(),
                new CUInt32Converter(),
                new CUInt64Converter(),
                new CDateTimeConverter(),
                new CGuidConverter(),
                new CNameConverter(),
                new CRUIDConverter(),
                new CStringConverter(),
                new CVariantConverter(),
                new BufferConverterFactory(s_bufferResolver),
                new LocalizationStringConverter(),
                new CLegacySingleChannelCurveConverterFactory(),
                new MultiChannelCurveConverterFactory(),
                new NodeRefConverter(),
                new TweakDBIDConverter(),
                new CByteArrayConverter(),
                new CKeyValuePairConverter(),
                new ArrayConverterFactory(),
                new EnumConverterFactory(),
                new HandleConverterFactory(),
                new ResourceConverterFactory(),
                new ClassConverterFactory(s_classResolver),
                new Red4FileConverterFactory()
            }
        };
    }

    public static string Serialize(object value)
    {
        s_bufferResolver.Begin();
        s_classResolver.Begin();

        var result = JsonSerializer.Serialize(value, s_settings);

        s_bufferResolver.End();
        s_classResolver.End();

        return result;
    }

    public static T? Deserialize<T>(string json)
    {
        s_bufferResolver.Begin();
        s_classResolver.Begin();

        var result = JsonSerializer.Deserialize<T>(json, s_settings);

        s_bufferResolver.End();
        s_classResolver.End();

        return result;
    }
}
