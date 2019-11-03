using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UXI.Serialization.Common;

namespace UXI.Serialization.Formats.Csv
{
    public class CsvDataWriter : DisposableBase, IDataWriter, IDisposable
    {
        private readonly CsvWriter _writer;
        private readonly CsvSerializerContext _serializer;

        private bool _isOpen = true;
        private bool _shouldWriteHeader = true;
    
        public CsvDataWriter(TextWriter writer, Type dataType, CsvSerializerContext serializer)
        {
            _serializer = serializer;
            _writer = new CsvWriter(writer, serializer.Configuration);

            DataType = dataType;
        }


        public Type DataType { get; }


        public bool CanWrite(Type objectType)
        {
            return _isOpen 
                && (DataType == objectType
                    || objectType.IsSubclassOf(DataType)
                    || DataType.IsAssignableFrom(objectType));
        }


        public void Write(object data)
        {
            TryWriteHeader();

            if (_isOpen)
            {
                try
                {
                    _serializer.Serialize(_writer, data, DataType);

                    _writer.NextRecord();
                }
                catch (Exception exception)
                {
                    throw new SerializationException($"Failed to write or serialize next data to the CSV file. See inner exception for more details.", exception);
                }
            }
        }


        private void TryWriteHeader()
        {
            if (_shouldWriteHeader)
            {
                try
                {
                    _serializer.WriteHeader(_writer, DataType);

                    _writer.NextRecord();
                }
                catch (Exception ex)
                {
                    throw new SerializationException($"Failed to write CSV header for the type [{DataType.FullName}]. See inner exception for more details.", ex);
                }
                _shouldWriteHeader = false;
            }
        }


        public void Close()
        {
            if (_isOpen)
            {
                TryWriteHeader();
                _isOpen = false;
                _writer.Flush();
            }
        }


        private bool _disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    _writer.Dispose();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
