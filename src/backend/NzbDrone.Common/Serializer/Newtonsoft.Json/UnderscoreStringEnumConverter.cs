using System;
using System.Text;
using Newtonsoft.Json;

namespace NzbDrone.Common.Serializer;

public class UnderscoreStringEnumConverter : JsonConverter
{
    public object? UnknownValue { get; set; }

    public UnderscoreStringEnumConverter(object? unknownValue)
    {
        UnknownValue = unknownValue;
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
        {
            return UnknownValue;
        }

        var enumString = reader.Value.ToString()?.Replace("_", string.Empty)
            ?? throw new JsonSerializationException("Null enum string");

        try
        {
            return Enum.Parse(objectType, enumString, true);
        }
        catch
        {
            if (UnknownValue == null)
            {
                throw;
            }

            return UnknownValue;
        }
    }

    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var enumText = value.ToString() ?? string.Empty;
        var builder = new StringBuilder(enumText.Length + 4);

        if (enumText.Length > 0)
        {
            builder.Append(char.ToLower(enumText[0]));
            for (var i = 1; i < enumText.Length; i++)
            {
                if (char.IsUpper(enumText[i]))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLower(enumText[i]));
            }
        }

        writer.WriteValue(builder.ToString());
    }
}
