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
    class CompositeObjectCsvConverter : CsvConverter<CompositeObject>
    {
        protected override bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref CompositeObject result)
        {
            SingleIntValue single;
            MultipleValues composite;
            double doubleValue;

            if (
                   TryGetMember<SingleIntValue>(reader, serializer, naming, nameof(CompositeObject.Single), out single)
                && TryGetMember<MultipleValues>(reader, serializer, naming, nameof(CompositeObject.Composite), out composite)
                && reader.TryGetField<double>(naming.Get(nameof(CompositeObject.Double)), out doubleValue)
               )
            {
                result = new CompositeObject()
                {
                    Single = single,
                    Composite = composite,
                    Double = doubleValue
                };

                return true;
            }

            return false;
        }

        protected override void Write(CompositeObject data, CsvWriter writer, CsvSerializerContext serializer)
        {
            serializer.Serialize(writer, data.Single);
            serializer.Serialize(writer, data.Composite);
            writer.WriteField(data.Double);
        }

        protected override void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            serializer.WriteHeader<SingleIntValue>(writer, naming, nameof(CompositeObject.Single));
            serializer.WriteHeader<MultipleValues>(writer, naming, nameof(CompositeObject.Composite));
            writer.WriteField(naming.Get(nameof(CompositeObject.Double)));
        }
    }
}
