using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UXI.Serialization.Csv;
using UXI.Serialization.Csv.Converters;
using UXI.Serialization.Fakes;

namespace UXI.Serialization.Fakes.Csv.Converters
{
    class SingleIntValueCsvConverter : CsvConverter<SingleIntValue>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref SingleIntValue result)
        {
            int value;

            if (reader.TryGetField<int>(naming.GetDefault(nameof(SingleIntValue.Value)), out value))
            {
                result = new SingleIntValue()
                {
                    Value = value
                };
                return true;
            }

            return false;
        }

        protected override void Write(SingleIntValue data, CsvWriter writer, CsvSerializerContext serializer)
        {
            writer.WriteField(data.Value);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            writer.WriteField(naming.GetDefault(nameof(SingleIntValue.Value)));
        }
    }
}
