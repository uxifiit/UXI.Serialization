using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UXI.Serialization.Json.Converters;

namespace UXI.Serialization.Json
{
    public class JsonSerializationFactory : ISerializationFactory
    {
        public JsonSerializationFactory()
            : this(Enumerable.Empty<ISerializationConfiguration>())
        { }


        public JsonSerializationFactory(params ISerializationConfiguration[] configurations)
            : this(configurations?.AsEnumerable())
        { }


        public JsonSerializationFactory(IEnumerable<ISerializationConfiguration> configurations)
        {
            Configurations = configurations?.ToList() ?? new List<ISerializationConfiguration>();
        }


        public FileFormat Format => FileFormat.JSON;


        public List<ISerializationConfiguration> Configurations { get; }


        public IDataReader CreateReaderForType(TextReader reader, Type dataType, object settings)
        {
            var serializer = CreateSerializer(DataAccess.Read, settings);

            return new JsonDataReader(reader, dataType, serializer);
        }


        public IDataWriter CreateWriterForType(TextWriter writer, Type dataType, object settings)
        {
            var serializer = CreateSerializer(DataAccess.Write, settings);

            return new JsonDataWriter(writer, serializer);
        }


        public JsonSerializer CreateSerializer(DataAccess access, object settings)
        {
            var serializer = new JsonSerializer()
            {
                Culture = System.Globalization.CultureInfo.GetCultureInfo("en-US")
            };

            foreach (var configuration in Configurations.ToArray())
            {
                serializer = (JsonSerializer)configuration.Configure(serializer, access, settings)
                           ?? serializer;
            }

            return serializer;
        }
    }
}
