namespace APIFORD.Util;

using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class DataBrasileiraConverter : JsonConverter<DateTime>
{
    private const string Formato = "dd/MM/yyyy-HH:mm";
    private static readonly TimeZoneInfo FusoBrasil = TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo");

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var texto = reader.GetString();
        var horaLocal = DateTime.ParseExact(texto!, Formato, CultureInfo.InvariantCulture);
        // O que chegou é horário de Brasília -> converte pra UTC antes de qualquer comparação/salvamento
        return TimeZoneInfo.ConvertTimeToUtc(DateTime.SpecifyKind(horaLocal, DateTimeKind.Unspecified), FusoBrasil);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // O que está guardado é sempre UTC -> converte pra Brasília só na hora de mostrar
        var horaLocal = TimeZoneInfo.ConvertTimeFromUtc(DateTime.SpecifyKind(value, DateTimeKind.Utc), FusoBrasil);
        writer.WriteStringValue(horaLocal.ToString(Formato, CultureInfo.InvariantCulture));
    }
}