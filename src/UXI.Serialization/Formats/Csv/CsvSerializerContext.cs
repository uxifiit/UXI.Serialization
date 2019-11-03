using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper.Configuration;
using UXI.Serialization.Common;
using UXI.Serialization.Csv.Converters;

namespace UXI.Serialization.Csv
{
    public class CsvSerializerContext
    {
        public CsvHelper.Configuration.Configuration Configuration { get; set; } = new CsvHelper.Configuration.Configuration();


        public Collection<CsvConverter> Converters { get; set; } = new Collection<CsvConverter>();


        public string HeaderNestingDelimiter { get; set; } = "";


        /// <summary>
        /// Gets or sets a boolean flag whether to throw SerializationException if no converter is available for deserializing requested type.
        /// If set to false, the default EmptyConverter is used for that type, returning the default value (null or default for value types), if the deserialized type is the main type; 
        /// or, if it is a member property, it is ignored. Default value is <b>true</b>.
        /// </summary>
        public bool ThrowIfNoConverterDefined { get; set; } = true;


        /// <summary>
        /// Gets or sets a boolean flag whether the converters should throw SerializationException if data conversion fails and the field is of a non-nullable type.
        /// If set to true, the exception is thrown; otherwise, the field is set to the default value of its type. Default value for this property is <b>false</b>. 
        /// </summary>
        public bool ThrowOnFailedDeserialize { get; set; } = false;


        private bool TryDeserialize(CsvHelper.CsvReader reader, Type dataType, CsvHeaderNamingContext naming, out object result)
        {
            var converter = Converters.FirstOrDefault(c => c.CanConvert(dataType) && c.CanRead);

            if (converter != null)
            {
                result = converter.ReadCsv(reader, dataType, this, naming);
                return true;
            }

            result = TypeHelper.GetDefault(dataType);
            return false;
        }


        public object Deserialize(CsvHelper.CsvReader reader, Type dataType)
        {
            return Deserialize(reader, dataType, new CsvHeaderNamingContext(Configuration.PrepareHeaderForMatch));
        }


        public object Deserialize(CsvHelper.CsvReader reader, Type dataType, CsvHeaderNamingContext naming)
        {
            var converter = Converters?.FirstOrDefault(c => c.CanConvert(dataType) && c.CanRead);
            if (converter != null)
            {
                return converter.ReadCsv(reader, dataType, this, naming);
            }
            else if (ThrowIfNoConverterDefined)
            {
                throw new ArgumentOutOfRangeException(nameof(dataType), $"No converter defined for the requested type '{dataType.FullName}' to deserialize.");
            }
            else
            {
                return TypeHelper.GetDefault(dataType);
            }
        }


        public T Deserialize<T>(CsvHelper.CsvReader reader, CsvHeaderNamingContext naming)
        {
            return (T)Deserialize(reader, typeof(T), naming);
        }


        public T Deserialize<T>(CsvHelper.CsvReader reader, CsvHeaderNamingContext naming, string referenceName)
        {
            return (T)Deserialize(reader, typeof(T), naming.GetNextLevel(referenceName, HeaderNestingDelimiter));
        }


        public void Serialize(CsvHelper.CsvWriter writer, object data, Type dataType)
        {
            var converter = Converters.FirstOrDefault(c => c.CanConvert(dataType) && c.CanWrite);

            converter.WriteCsv(data, writer, this);
        }


        public void Serialize<T>(CsvHelper.CsvWriter writer, T data)
        {
            Serialize(writer, data, typeof(T));
        }


        public void WriteHeader(CsvHelper.CsvWriter writer, Type dataType)
        {
            WriteHeader(writer, dataType, new CsvHeaderNamingContext(Configuration.PrepareHeaderForMatch));
        }


        public void WriteHeader(CsvHelper.CsvWriter writer, Type dataType, CsvHeaderNamingContext naming)
        {
            var converter = Converters.FirstOrDefault(c => c.CanConvert(dataType) && c.CanWrite);

            converter.WriteCsvHeader(writer, dataType, this, naming);
        }


        public void WriteHeader<T>(CsvHelper.CsvWriter writer, CsvHeaderNamingContext naming)
        {
            WriteHeader(writer, typeof(T), naming);
        }


        public void WriteHeader<T>(CsvHelper.CsvWriter writer, CsvHeaderNamingContext naming, string referenceName)
        {
            WriteHeader<T>(writer, naming.GetNextLevel(referenceName, HeaderNestingDelimiter));
        }


        public void ReadHeader(CsvHelper.CsvReader reader, Type dataType)
        {
            var converter = Converters.FirstOrDefault(c => c.CanConvert(dataType) && c.CanRead);

            converter.ReadCsvHeader(reader, this);
        }
    }
}
