using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UXI.Serialization.Formats.Csv;
using UXI.Serialization.Formats.Csv.Converters;
using UXI.Serialization.Fakes;

namespace UXI.Serialization.Fakes.Csv.Converters
{
    class SingleStringValueCsvConverter : CsvConverter<SingleStringValue>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref SingleStringValue result)
        {
            string value;

            if (reader.TryGetField<string>(naming.GetDefault("Value"), out value))
            {
                result = new SingleStringValue()
                {
                    Value = value
                };

                return true;
            }

            return false;
        }

        protected override void Write(SingleStringValue data, CsvWriter writer, CsvSerializerContext serializer)
        {
            writer.WriteField(data.Value);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            writer.WriteField(naming.GetDefault("Value"));
        }
    }
}
