using Helmut.General.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Helmut.General;

internal sealed class CoordinatesConverter : JsonConverter<Coordinates>
{
    public override Coordinates Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType is not JsonTokenType.StartObject) return Coordinates.Empty;

        var values = new Dictionary<string, double>();

        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.EndObject) break;

            if (reader.TokenType is not JsonTokenType.PropertyName) continue;

            var property = reader.GetString();

            if (string.IsNullOrEmpty(property)) return Coordinates.Empty;

            reader.Read();

            if (reader.TryGetDouble(out var value) is false) return Coordinates.Empty;

            values[property] = value;
        }

        if (values.TryGetValue(nameof(Coordinates.Latitude), out var latitude)
            && values.TryGetValue(nameof(Coordinates.Longitude), out var longitude))
        {
            return new Coordinates(latitude, longitude);
        }

        return Coordinates.Empty;
    }

    public override void Write(Utf8JsonWriter writer, Coordinates value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName(nameof(Coordinates.Latitude));
        writer.WriteNumberValue(value.Latitude);
        writer.WritePropertyName(nameof(Coordinates.Longitude));
        writer.WriteNumberValue(value.Longitude);
        writer.WriteEndObject();
    }
}
