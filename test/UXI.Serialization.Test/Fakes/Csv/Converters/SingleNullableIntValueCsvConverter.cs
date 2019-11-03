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
    class SingleNullableIntValueCsvConverter : CsvConverter<SingleNullableIntValue>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref SingleNullableIntValue result)
        {
            int? value;

            if (reader.TryGetField<int?>(naming.GetDefault(nameof(SingleNullableIntValue.Value)), out value))
            {
                result = new SingleNullableIntValue()
                {
                    Value = value
                };

                return true;
            }

            return false;
        }

        protected override void Write(SingleNullableIntValue data, CsvWriter writer, CsvSerializerContext serializer)
        {
            writer.WriteField(data.Value);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            writer.WriteField(naming.GetDefault(nameof(SingleNullableIntValue.Value)));
        }
    }
}
