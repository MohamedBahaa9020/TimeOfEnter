using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeOfEnter.DateTimeMiddlleWare
{
    public class DateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
           return reader.GetDateTime().ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            TimeZoneInfo egypt = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            value=DateTime.SpecifyKind(value, DateTimeKind.Utc); 
            writer.WriteStringValue(TimeZoneInfo.ConvertTimeFromUtc(value, egypt)
                .ToString("yyyy-MM-dd dddd HH:mm:ss"));
        }
    }

    public class NullableDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return reader.GetDateTime().ToUniversalTime();
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            TimeZoneInfo egypt = TimeZoneInfo.FindSystemTimeZoneById("Egypt Standard Time");
            if (value is null)
            {
                writer.WriteNullValue();                                  
                return;
            }
            if  ( value is not null)
            writer.WriteStringValue(TimeZoneInfo.ConvertTimeFromUtc(value.Value, egypt)
                .ToString("yyyy-MM-dd dddd HH:mm:ss"));

        }
    }

}
