using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Serialization.Configurations;

namespace UXI.Serialization.Formats.Json.Configurations
{
    public class JsonConvertersSerializationConfiguration : SerializationConfiguration<JsonSerializer>
    {
        public JsonConvertersSerializationConfiguration(params JsonConverter[] converters) 
            : this(converters?.AsEnumerable())
        { }


        public JsonConvertersSerializationConfiguration(IEnumerable<JsonConverter> converters)
        {
            Converters = converters?.ToList() ?? new List<JsonConverter>();
        }


        public List<JsonConverter> Converters { get; }


        protected override JsonSerializer Configure(JsonSerializer serializer, DataAccess access, Type dataType, object settings)
        {
            IEnumerable<JsonConverter> converters = Converters;
            if (access == DataAccess.Read)
            {
                converters = Converters.Where(c => c.CanRead);
            }
            else if (access == DataAccess.Write)
            {
                converters = Converters.Where(c => c.CanWrite);
            }

            foreach (var converter in converters)
            {
                serializer.Converters.Add(converter);
            }

            return serializer;
        }
    }
}
