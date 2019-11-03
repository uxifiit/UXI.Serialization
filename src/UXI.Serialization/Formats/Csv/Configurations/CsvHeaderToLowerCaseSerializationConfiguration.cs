using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UXI.Serialization.Configurations;
using UXI.Serialization.Formats.Csv;

namespace UXI.Serialization.Formats.Csv.Configurations
{
    public class CsvHeaderToLowerCaseSerializationConfiguration : SerializationConfiguration<CsvSerializerContext>
    {
        protected override CsvSerializerContext Configure(CsvSerializerContext serializer, DataAccess acess, object settings)
        {
            serializer.Configuration.PrepareHeaderForMatch = header => header.ToLower();

            return serializer;
        }
    }
}
