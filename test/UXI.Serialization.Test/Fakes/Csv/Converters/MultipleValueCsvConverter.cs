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
    class CompositeValueCsvConverter : CsvConverter<MultipleValues>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref MultipleValues result)
        {
            int id;
            string name;

            if (reader.TryGetField<int>(naming.Get(nameof(MultipleValues.Id)), out id)
                && reader.TryGetField<string>(naming.Get(nameof(MultipleValues.Name)), out name))
            {
                result = new MultipleValues()
                {
                    Id = id,
                    Name = name
                };
                return true;
            }

            return false;
        }

        protected override void Write(MultipleValues data, CsvWriter writer, CsvSerializerContext serializer)
        {
            writer.WriteField(data.Id);
            writer.WriteField(data.Name);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            writer.WriteField(naming.Get(nameof(MultipleValues.Id)));
            writer.WriteField(naming.Get(nameof(MultipleValues.Name)));
        }
    }
}
