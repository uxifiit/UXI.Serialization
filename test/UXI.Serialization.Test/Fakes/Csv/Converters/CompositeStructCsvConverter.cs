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
    class CompositeStructCsvConverter : CsvConverter<CompositeStruct>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref CompositeStruct result)
        {
            int id;
            double duration;

            if (
                    reader.TryGetField<int>(naming.Get(nameof(CompositeStruct.Id)), out id)
                 && reader.TryGetField<double>(naming.Get(nameof(CompositeStruct.Duration)), out duration)
               )
            {
                result = new CompositeStruct()
                {
                    Id = id,
                    Duration = duration
                };

                return true;
            }

            return false;
        }

        protected override void Write(CompositeStruct data, CsvWriter writer, CsvSerializerContext serializer)
        {
            writer.WriteField(data.Id);
            writer.WriteField(data.Duration);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            writer.WriteField(naming.Get(nameof(CompositeStruct.Id)));
            writer.WriteField(naming.Get(nameof(CompositeStruct.Duration)));
        }
    }
}
