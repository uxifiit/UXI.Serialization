using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UXI.Serialization.Common;

namespace UXI.Serialization.Formats.Json.Converters
{
    public abstract class GenericJsonConverter<T> : Newtonsoft.Json.JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var supportedType = typeof(T);

            return (objectType == supportedType)
                || (supportedType.IsValueType && objectType.IsValueType && Nullable.GetUnderlyingType(objectType) == supportedType);
        }


        protected abstract T Convert(JToken token, JsonSerializer serializer);


        public sealed override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return TypeHelper.GetDefault(objectType);
            }
            else
            {
                // Load the JSON for the Result into a JObject
                JToken token = JToken.Load(reader);

                // Construct the Result object using the conversion function
                T result = Convert(token, serializer);

                // Return the result
                return result;
            }
        }


        public override bool CanWrite => false;


        protected virtual JToken ConvertBack(T value, JsonSerializer serializer)
        {
            throw new NotSupportedException();
        }


        public sealed override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JToken token = ConvertBack((T)value, serializer);

            token.WriteTo(writer);
        }
    }
}
