using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Serialization.Formats.Csv.Converters;

namespace UXI.Serialization.Formats.Csv
{
    public class CsvSerializationFactory : ISerializationFactory
    {
        public CsvSerializationFactory()
            : this(Enumerable.Empty<ISerializationConfiguration>())
        { }


        public CsvSerializationFactory(params ISerializationConfiguration[] configurations)
            : this(configurations?.AsEnumerable())
        { }


        public CsvSerializationFactory(IEnumerable<ISerializationConfiguration> configurations)
        {
            Configurations = configurations?.ToList() ?? new List<ISerializationConfiguration>();
        }


        public FileFormat Format => FileFormat.CSV;


        public List<ISerializationConfiguration> Configurations { get; }


        public IDataReader CreateReaderForType(TextReader reader, Type dataType, object settings)
        {
            var serializer = CreateSerializer(DataAccess.Read, settings);

            return new CsvDataReader(reader, dataType, serializer);
        }


        public IDataWriter CreateWriterForType(TextWriter writer, Type dataType, object settings)
        {
            var serializer = CreateSerializer(DataAccess.Write, settings);

            return new CsvDataWriter(writer, dataType, serializer);
        }


        private CsvSerializerContext CreateSerializer(DataAccess access, object settings)
        {
            var serializer = new CsvSerializerContext();

            serializer.Configuration.CultureInfo = System.Globalization.CultureInfo.GetCultureInfo("en-US");

            foreach (var configuration in Configurations.ToArray())
            {
                serializer = (CsvSerializerContext)configuration.Configure(serializer, access, settings)
                           ?? serializer;
            }

            return serializer;
        }
    }
}