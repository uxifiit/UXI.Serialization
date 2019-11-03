using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Serialization.Configurations;
using UXI.Serialization.Formats.Csv.Converters;

namespace UXI.Serialization.Formats.Csv.Configurations
{
    public class CsvConvertersSerializationConfiguration : SerializationConfiguration<CsvSerializerContext>
    {
        public CsvConvertersSerializationConfiguration(IEnumerable<CsvConverter> converters)
        {
            Converters = converters?.ToList() ?? new List<CsvConverter>();
        }


        public CsvConvertersSerializationConfiguration(params CsvConverter[] converters)
            : this(converters?.AsEnumerable())
        { }


        public List<CsvConverter> Converters { get; }


        protected override CsvSerializerContext Configure(CsvSerializerContext serializer, DataAccess access, object settings)
        {
            IEnumerable<CsvConverter> converters = Converters;
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
