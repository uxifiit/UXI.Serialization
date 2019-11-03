using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UXI.Serialization.Csv;
using UXI.Serialization.Csv.Converters;

namespace UXI.Serialization.Fakes.Csv.Converters
{
    class SingleNullableStructValueCsvConverter : CsvConverter<SingleNullableStructValue>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref SingleNullableStructValue result)
        {
            CompositeStruct? value;

            if (TryGetMember<CompositeStruct?>(reader, serializer, naming, nameof(SingleNullableStructValue.Value), out value))
            {
                result = new SingleNullableStructValue()
                {
                    Value = value
                };

                return true;
            }

            return false;
        }

        protected override void Write(SingleNullableStructValue data, CsvWriter writer, CsvSerializerContext serializer)
        {
            serializer.Serialize<CompositeStruct?>(writer, data.Value);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            serializer.WriteHeader<CompositeStruct?>(writer, naming, nameof(SingleNullableStructValue.Value));
        }
    }
}
