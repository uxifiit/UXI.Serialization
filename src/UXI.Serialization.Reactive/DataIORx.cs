using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Reactive
{
    public static class DataIORx
    {
        public static IObservable<object> ReadInputAsObservable(this DataIO io, string filePath, FileFormat fileFormat, Type dataType, object settings = null)
        {
            FileFormat format = io.EnsureCorrectFileFormat(filePath, fileFormat);

            return Observable.Using(() => FileHelper.CreateInputReader(filePath), (reader) =>
            {
                return ReadInputAsObservable(io, reader, format, dataType, settings);
            });
        }


        public static IObservable<object> ReadInputAsObservable(this DataIO io, TextReader reader, FileFormat format, Type dataType, object settings = null)
        {
            return Observable.Using(() => io.GetInputDataReader(reader, format, dataType, settings), (dataReader) =>
            {
                return Observable.Create<object>(observer =>
                {
                    try
                    {
                        object data;
                        while (dataReader.TryRead(out data))
                        {
                            observer.OnNext(data);
                        }

                        observer.OnCompleted();
                    }
                    catch (Exception ex)
                    {
                        observer.OnError(ex);
                    }

                    return Disposable.Empty;
                });
            });
        }


        public static IObservable<object> WriteOutput(this DataIO io, IObservable<object> data, string filePath, FileFormat fileFormat, Type dataType, object settings = null)
        {
            FileFormat format = io.EnsureCorrectFileFormat(filePath, fileFormat);

            return Observable.Using(() => FileHelper.CreateOutputWriter(filePath), (writer) =>
            {
                return WriteOutput(io, data, writer, fileFormat, dataType, settings);
            });
        }


        public static IObservable<object> WriteOutput(this DataIO io, IObservable<object> data, TextWriter writer, FileFormat format, Type dataType, object settings = null)
        {
            return Observable.Using(() => io.GetOutputDataWriter(writer, format, dataType, settings), (IDataWriter dataWriter) =>
            {
                return data.Finally(dataWriter.Close)
                           .Do(d => dataWriter.Write(d));
            });
        }
    }
}
