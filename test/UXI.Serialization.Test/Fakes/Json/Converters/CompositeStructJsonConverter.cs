using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UXI.Serialization.Json.Converters;
using UXI.Serialization.Json.Extensions;

namespace UXI.Serialization.Fakes.Json.Converters
{
    class CompositeStructJsonConverter : GenericJsonConverter<CompositeStruct>
    {
        protected override CompositeStruct Convert(JToken token, JsonSerializer serializer)
        {
            JObject obj = (JObject)token;

            int id = obj.GetValueOrDefault<int>(nameof(CompositeStruct.Id), serializer);
            double duration = obj.GetValueOrDefault<double>(nameof(CompositeStruct.Duration), serializer);

            return new CompositeStruct()
            {
                Id = id,
                Duration = duration
            };
        }
    }
}
