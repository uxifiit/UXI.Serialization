using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UXI.Serialization.Common;

namespace UXI.Serialization.Csv.Converters
{
    public abstract class CsvConverter
    {
        public virtual bool CanRead => true;

        public virtual bool CanWrite => true;

        public abstract bool CanConvert(Type objectType);

        public virtual void ReadCsvHeader(CsvReader reader, CsvSerializerContext serializer)
        {
            reader.ReadHeader();
        }

        public abstract void WriteCsvHeader(CsvWriter writer, Type objectType, CsvSerializerContext serializer, CsvHeaderNamingContext naming);

        public abstract void WriteCsv(object data, CsvWriter writer, CsvSerializerContext serializer);

        public abstract object ReadCsv(CsvReader reader, Type objectType, CsvSerializerContext serializer, CsvHeaderNamingContext naming);
    }


    public abstract class CsvConverter<T> : CsvConverter
    {
        public override bool CanConvert(Type objectType)
        {
            var supportedType = typeof(T);

            return (objectType == supportedType)
                || (supportedType.IsValueType && objectType.IsValueType && Nullable.GetUnderlyingType(objectType) == supportedType);
        }


        public int Columns { get; protected set; }


        /// <summary>
        /// Gets or sets a boolean flag determining whether the SerializationException should be thrown, if both the data can not be read and the return value is of a non-nullable type.
        /// If not set, converter relies on the ThrowOnFailedDeserialize property of the calling serializer. Default value is <b>null</b> and the serializer settings are used.
        /// </summary>
        public virtual bool? ThrowOnFailedRead { get; set; } = null;


        public sealed override object ReadCsv(CsvReader reader, Type objectType, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            T result = default(T);

            if (TryRead(reader, serializer, naming, ref result))
            {
                return result;
            }
            else if (TypeHelper.CanBeNull(objectType) == false
                    && serializer.ThrowOnFailedDeserialize 
                    && (ThrowOnFailedRead.HasValue == false || ThrowOnFailedRead.Value))
            {
                throw new SerializationException($"Failed to read the data of type [{objectType.FullName}] with the converter for type [{typeof(T).FullName}].");
            }
            else
            {
                // Instead of returning just default(T), we must construct the correct default value more robustly.
                // For example, requested objectType can be Nullable<T>, if the converter generic type T is struct.
                return TypeHelper.GetDefault(objectType);
            }
        }


        protected bool TryGetMember<TMember>(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, out TMember member)
        {
            member = serializer.Deserialize<TMember>(reader, naming);
            return true;
        }


        protected bool TryGetMember<TMember>(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, string name, out TMember member)
        {
            member = serializer.Deserialize<TMember>(reader, naming, name);
            return true;
        }


        protected abstract bool TryRead(CsvReader reader, CsvSerializerContext serializer, CsvHeaderNamingContext naming, ref T result);


        protected abstract void WriteHeader(CsvWriter writer, CsvSerializerContext serializer, CsvHeaderNamingContext naming);


        public sealed override void WriteCsvHeader(CsvWriter writer, Type objectType, CsvSerializerContext serializer, CsvHeaderNamingContext naming)
        {
            int columnsCountBefore = writer.Context.Record.Count;

            WriteHeader(writer, serializer, naming);
            
            int columnsCountAfter = writer.Context.Record.Count;

            Columns = Math.Max(columnsCountAfter - columnsCountBefore, 0);
        }


        protected abstract void Write(T data, CsvWriter writer, CsvSerializerContext serializer);


        public sealed override void WriteCsv(object data, CsvWriter writer, CsvSerializerContext serializer)
        {
            if (data is T)
            {
                try
                {
                    Write((T)data, writer, serializer);
                }
                catch (Exception exception)
                {
                    throw new SerializationException($"Failed to write or serialize next data to the CSV file. See inner exception for more details.", exception);
                }
            }
            else if (Object.ReferenceEquals(data, null)) 
            {
                try
                {
                    WriteEmptyFields(writer);
                }
                catch (Exception exception)
                {
                    throw new SerializationException($"Failed to write null record for the type [{typeof(T).FullName}] to the CSV file. See inner exception for more details.", exception);
                }
            }
            else
            {
                throw new ArgumentException($"Type of the passed object [{data?.GetType().FullName}] does not match the type supported by this converter [{typeof(T).FullName}].");
            }
        }


        protected virtual void WriteEmptyFields(CsvWriter writer)
        {
            int count = Columns;

            while (count-- > 0)
            {
                writer.WriteField(null);
            }
        }
    }
}
