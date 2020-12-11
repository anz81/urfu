using Newtonsoft.Json;
using System;

namespace Ext.Utilities
{
    public class SortDirectionTypeEnumConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var enumString = (string)reader.Value;

            if (enumString == "ASC")
            {
                return SortDirection.Ascending;
            }
            else if (enumString == "DESC")
            {
                return SortDirection.Descending;
            }
            throw new NotSupportedException("Неизвестное значение: " + enumString);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }
    }
}
