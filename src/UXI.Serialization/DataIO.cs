using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization
{
    public class DataIO
    {
        public DataIO(params ISerializationFactory[] factories)
            : this(factories?.AsEnumerable())
        {
        }


        public DataIO(IEnumerable<ISerializationFactory> factories)
        {
            Formats = factories?.ToDictionary(f => f.Format) ?? new Dictionary<FileFormat, ISerializationFactory>();
        }


        public IDictionary<FileFormat, ISerializationFactory> Formats { get; }


        public IEnumerable<object> ReadInput(string filePath, FileFormat fileFormat, Type dataType, object settings)
        {
            FileFormat format = EnsureCorrectFileFormat(filePath, fileFormat);

            using (var reader = FileHelper.CreateInputReader(filePath))
            {
                foreach (var item in ReadInput(reader, format, dataType, settings))
                {
                    yield return item;
                }
            }
        }


        public IEnumerable<T> ReadInput<T>(string filePath, FileFormat fileFormat, object settings)
        {
            return ReadInput(filePath, fileFormat, typeof(T), settings).OfType<T>();
        }


        public IEnumerable<object> ReadInput(TextReader reader, FileFormat format, Type dataType, object settings)
        {
            using (var dataReader = GetInputDataReader(reader, format, dataType, settings))
            {
                object data;
                while (dataReader.TryRead(out data))
                {
                    yield return data;
                }

                yield break;
            }
        }


        public IEnumerable<T> ReadInput<T>(TextReader reader, FileFormat format, Type dataType, object settings)
        {
            return ReadInput(reader, format, dataType, settings).OfType<T>();
        }


        public IDataReader GetInputDataReader(TextReader reader, FileFormat fileType, Type dataType, object settings)
        {
            ISerializationFactory factory;

            if (Formats.TryGetValue(fileType, out factory))
            {
                return factory.CreateReaderForType(reader, dataType, settings);
            }

            throw new ArgumentOutOfRangeException(nameof(fileType));
        }


        public void WriteOutput(IEnumerable<object> data, string filePath, FileFormat fileFormat, Type dataType, object settings)
        {
            FileFormat format = EnsureCorrectFileFormat(filePath, fileFormat);

            using (var writer = FileHelper.CreateOutputWriter(filePath))
            {
                WriteOutput(data, writer, format, dataType, settings);
            }
        }


        public void WriteOutput<T>(IEnumerable<T> data, string filePath, FileFormat format, object settings)
        {
            WriteOutput(data?.Cast<object>(), filePath, format, typeof(T), settings);
        }


        public void WriteOutput(IEnumerable<object> data, TextWriter writer, FileFormat format, Type dataType, object settings)
        {
            using (var dataWriter = GetOutputDataWriter(writer, format, dataType, settings))
            {
                foreach (var item in data)
                {
                    dataWriter.Write(item);
                }

                dataWriter.Close();
            }
        }


        public void WriteOutput<T>(IEnumerable<T> data, TextWriter writer, FileFormat format, object settings)
        {
            WriteOutput(data?.Cast<object>(), writer, format, typeof(T), settings);
        }


        public IDataWriter GetOutputDataWriter(TextWriter writer, FileFormat fileType, Type dataType, object settings)
        {
            ISerializationFactory factory;

            if (Formats.TryGetValue(fileType, out factory))
            {
                return factory.CreateWriterForType(writer, dataType, settings);
            }

            throw new ArgumentOutOfRangeException(nameof(fileType));
        }


        public FileFormat EnsureCorrectFileFormat(string filename, FileFormat requestedFormat)
        {
            string extension = Path.GetExtension(filename)?.TrimStart('.');

            if (String.IsNullOrWhiteSpace(extension) == false)
            {
                var matchingFormat = Formats.Where(f => f.Key.ToString().Equals(extension, StringComparison.CurrentCultureIgnoreCase))
                                            .Select(f => f.Value)
                                            .FirstOrDefault();

                return matchingFormat != null && matchingFormat.Format != requestedFormat
                     ? matchingFormat.Format
                     : requestedFormat;
            }

            return requestedFormat;
        }
    }
}
