using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization
{
    public interface ISerializationFactory
    {
        List<ISerializationConfiguration> Configurations { get; }

        FileFormat Format { get; }

        IDataWriter CreateWriterForType(TextWriter writer, Type dataType, object settings);

        IDataReader CreateReaderForType(TextReader reader, Type dataType, object settings);
    }
}
