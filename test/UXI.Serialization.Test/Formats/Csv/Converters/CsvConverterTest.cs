using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UXI.Serialization.Fakes;
using UXI.Serialization.Fakes.Csv.Converters;

namespace UXI.Serialization.Csv.Converters
{
    [TestClass]
    public class CsvConverterTest
    {
        [TestMethod]
        public void WriteHeader_SingleIntValue_ColumnsIs1()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleIntValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            Assert.AreEqual(1, converter.Columns);
        }


        [TestMethod]
        public void WriteHeader_CompositeValue_ColumnsIs2()
        {
            var serializer = new CsvSerializerContext();
            var converter = new CompositeValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(MultipleValues), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            Assert.AreEqual(2, converter.Columns);
        }


        [TestMethod]
        public void WriteHeader_CompositeObject_ColumnsIs4()
        {
            var serializer = new CsvSerializerContext();
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(CompositeObject), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            Assert.AreEqual(4, converter.Columns);
        }


        [TestMethod]
        public void WriteCsv_SingleIntValue_NullRecordOnly_EmptyRecord()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleIntValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(null, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Value",    // header
                "",         // empty record
                ""          // empty line after the last record
            };


            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }



        [TestMethod]
        public void WriteCsv_SingleIntValue_ValidRecords()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleIntValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 3 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 4 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 5 }, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Value",    // header
                "3",        // first record
                "4",        // second record
                "5",        // third record
                ""          // empty line after the last record
            };


            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }



        [TestMethod]
        public void WriteCsv_SingleIntValue_ValidRecordsWithNullRecord()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleIntValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 3 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(null, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 4 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleIntValue() { Value = 5 }, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Value",    // header
                "3",        // first record
                "",         // second empty record
                "4",        // third record
                "5",        // fourth record
                ""          // empty line after the last record
            };


            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }


        [TestMethod]
        public void WriteCsv_CompositeObject_ValidRecords()
        {
            var serializer = new CsvSerializerContext();
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(CompositeObject), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = new MultipleValues() { Id = 1, Name = "Apple" }, Single = new SingleIntValue() { Value = 15 }, Double = 52.1 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = new MultipleValues() { Id = 2, Name = "Strawberry" }, Single = new SingleIntValue() { Value = 158792 }, Double = -15.2 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = new MultipleValues() { Id = 3, Name = "Orange" }, Single = new SingleIntValue() { Value = 0 }, Double = 0 }, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Single,CompositeId,CompositeName,Double",    // header
                "15,1,Apple,52.1",                             // first record
                "158792,2,Strawberry,-15.2",                   // second record
                "0,3,Orange,0",                                // third record
                ""                                             // empty line after the last record
            };


            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }


        [TestMethod]
        public void WriteCsv_CompositeObject_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext();
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(CompositeObject), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = new MultipleValues() { Id = 1, Name = null }, Single = new SingleIntValue() { Value = 15 }, Double = 52.1 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = new MultipleValues() { Id = 2, Name = "Strawberry" }, Single = null, Double = -15.2 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new CompositeObject() { Composite = null, Single = new SingleIntValue() { Value = 0 }, Double = 0 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(null, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Single,CompositeId,CompositeName,Double",    // header
                "15,1,,52.1",                                  // first record
                ",2,Strawberry,-15.2",                         // second record
                "0,,,0",                                       // third record
                ",,,",                                         // fourth null record
                ""                                             // empty line after the last record
            };


            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }

        
        [TestMethod]
        public void WriteCsv_ClassWithNullablePrimitive_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleNullableIntValueCsvConverter();
            serializer.Converters.Add(converter);

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleNullableIntValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableIntValue() { Value = 1 }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableIntValue() { Value = null }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableIntValue() { Value = 2 }, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "Value",        // header
                "1",            // first record
                "",             // second empty record
                "2",            // third record
                ""              // empty line after the last record
            };

            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }


        [TestMethod]
        public void WriteCsv_ClassWithNullableCompositeStruct_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleNullableStructValueCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new CompositeStructCsvConverter());

            var target = new StringWriter();
            var writer = new CsvWriter(target, serializer.Configuration);

            converter.WriteCsvHeader(writer, typeof(SingleNullableStructValue), serializer, new CsvHeaderNamingContext());
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableStructValue() { Value = new CompositeStruct() { Id = 1, Duration = 15.24 } }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableStructValue() { Value = null }, writer, serializer);
            writer.NextRecord();

            converter.WriteCsv(new SingleNullableStructValue() { Value = new CompositeStruct() { Id = 2, Duration = -99.24 } }, writer, serializer);
            writer.NextRecord();

            writer.Dispose();
            target.Close();

            string[] expected = new string[] {
                "ValueId,ValueDuration",        // header
                "1,15.24",                      // first record
                ",",                            // second empty record
                "2,-99.24",                     // third record
                ""                              // empty line after the last record
            };

            Assert.AreEqual(String.Join(Environment.NewLine, expected), target.ToString());
        }


        [TestMethod]
        public void ReadCsv_ClassWithNullableCompositeStruct_ObjectsWithNullValues()
        {
            var serializer = new CsvSerializerContext() { ThrowOnFailedDeserialize = false };
            var converter = new SingleNullableStructValueCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new CompositeStructCsvConverter());

            string[] csv = new string[] {
                "ValueId,ValueDuration",        // header
                "1,15.24",                      // first record
                ",",                            // second empty record
                "2,-99.24",                     // third record
                ""                              // empty line after the last record
            };

            List<SingleNullableStructValue> result = new List<SingleNullableStructValue>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((SingleNullableStructValue)converter.ReadCsv(reader, typeof(SingleNullableStructValue), serializer, naming));
                }
            }

            Assert.AreEqual(3, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.IsTrue(result[0].Value.HasValue);
            Assert.AreEqual(1, result[0].Value.Value.Id);
            Assert.AreEqual(15.24, result[0].Value.Value.Duration);

            Assert.IsNotNull(result[1]);
            Assert.IsFalse(result[1].Value.HasValue);

            Assert.IsNotNull(result[2]);
            Assert.IsTrue(result[2].Value.HasValue);
            Assert.AreEqual(2, result[2].Value.Value.Id);
            Assert.AreEqual(-99.24, result[2].Value.Value.Duration);
        }



        [TestMethod]
        public void ReadCsv_ClassWithNullablePrimitive_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleNullableIntValueCsvConverter();
            serializer.Converters.Add(converter);

            string[] csv = new string[] {
                "Value",        // header
                "1",            // first record
                "",             // second empty record
                "2",            // third record
                ""              // empty line after the last record
            };

            List<SingleNullableIntValue> result = new List<SingleNullableIntValue>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((SingleNullableIntValue)converter.ReadCsv(reader, typeof(SingleNullableIntValue), serializer, naming));
                }
            }

            Assert.AreEqual(2, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.IsTrue(result[0].Value.HasValue);
            Assert.AreEqual(1, result[0].Value.Value);

            Assert.IsNotNull(result[1]);
            Assert.IsTrue(result[1].Value.HasValue);
            Assert.AreEqual(2, result[1].Value.Value);
        }


        [TestMethod]
        public void ReadCsv_SingleIntValue_ValidRecords()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            string[] csv = new string[] {
                "Value",    // header
                "3",        // first record
                "4",        // second record
                "5",        // third record
                ""          // empty line after the last record
            };

            List<SingleIntValue> result = new List<SingleIntValue>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((SingleIntValue)converter.ReadCsv(reader, typeof(SingleIntValue), serializer, naming));
                }
            }

            Assert.AreEqual(3, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.AreEqual(3, result[0].Value);

            Assert.IsNotNull(result[1]);
            Assert.AreEqual(4, result[1].Value);

            Assert.IsNotNull(result[2]);
            Assert.AreEqual(5, result[2].Value);
        }


        [TestMethod]
        public void ReadCsv_SingleIntValue_NullRecords()
        {
            var serializer = new CsvSerializerContext();
            var converter = new SingleIntValueCsvConverter();
            serializer.Converters.Add(converter);

            string[] csv = new string[] {
                "Value",    // header
                "",        // first record
                "",        // second record
                "",        // third record
                ""          // empty line after the last record
            };

            List<SingleIntValue> result = new List<SingleIntValue>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((SingleIntValue)converter.ReadCsv(reader, typeof(SingleIntValue), serializer, naming));
                }
            }

            Assert.AreEqual(0, result.Count);
        }


        [TestMethod]
        public void ReadCsv_CompositeObject_ValidRecords()
        {
            var serializer = new CsvSerializerContext();
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            string[] csv = new string[] {
                "Single,CompositeId,CompositeName,Double",    // header
                "15,1,Apple,52.1",                             // first record
                "158792,2,Strawberry,-15.2",                   // second record
                "0,3,Orange,0",                                // third record
                ""                                             // empty line after the last record
            };

            List<CompositeObject> result = new List<CompositeObject>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((CompositeObject)converter.ReadCsv(reader, typeof(CompositeObject), serializer, naming));
                }
            }

            Assert.AreEqual(3, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[0].Single);
            Assert.AreEqual(15, result[0].Single.Value);
            Assert.IsNotNull(result[0].Composite);
            Assert.AreEqual(1, result[0].Composite.Id);
            Assert.AreEqual("Apple", result[0].Composite.Name);
            Assert.AreEqual(52.1, result[0].Double);

            Assert.IsNotNull(result[1]);
            Assert.IsNotNull(result[1].Single);
            Assert.AreEqual(158792, result[1].Single.Value);
            Assert.IsNotNull(result[1].Composite);
            Assert.AreEqual(2, result[1].Composite.Id);
            Assert.AreEqual("Strawberry", result[1].Composite.Name);
            Assert.AreEqual(-15.2, result[1].Double);

            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[2].Single);
            Assert.AreEqual(0, result[2].Single.Value);
            Assert.IsNotNull(result[2].Composite);
            Assert.AreEqual(3, result[2].Composite.Id);
            Assert.AreEqual("Orange", result[2].Composite.Name);
            Assert.AreEqual(0, result[2].Double);
        }


        [TestMethod]
        public void ReadCsv_CompositeObject_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext() { ThrowOnFailedDeserialize = false };
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            string[] csv = new string[] {
                "Single,CompositeId,CompositeName,Double",    // header
                "15,1,,52.1",                                  // first record
                ",2,Strawberry,-15.2",                         // second record
                "0,,,0",                                       // third record
                ",,,",                                         // fourth null record
                ""                                             // empty line after the last record
            };

            List<CompositeObject> result = new List<CompositeObject>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((CompositeObject)converter.ReadCsv(reader, typeof(CompositeObject), serializer, naming));
                }
            }

            Assert.AreEqual(4, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[0].Single);
            Assert.AreEqual(15, result[0].Single.Value);
            Assert.IsNotNull(result[0].Composite);
            Assert.AreEqual(1, result[0].Composite.Id);
            Assert.AreEqual(String.Empty, result[0].Composite.Name);
            Assert.AreEqual(52.1, result[0].Double);

            Assert.IsNotNull(result[1]);
            Assert.IsNull(result[1].Single);
            Assert.IsNotNull(result[1].Composite);
            Assert.AreEqual(2, result[1].Composite.Id);
            Assert.AreEqual("Strawberry", result[1].Composite.Name);
            Assert.AreEqual(-15.2, result[1].Double);

            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[2].Single);
            Assert.AreEqual(0, result[2].Single.Value);
            Assert.IsNull(result[2].Composite);
            Assert.AreEqual(0, result[2].Double);

            Assert.IsNull(result[3]);
        }


        [TestMethod]
        public void ReadCsv_CompositeObject_MissingMember_RecordsWithNullValues()
        {
            var serializer = new CsvSerializerContext() { ThrowOnFailedDeserialize = false };
            var converter = new CompositeObjectCsvConverter();
            serializer.Converters.Add(converter);
            serializer.Converters.Add(new SingleIntValueCsvConverter());
            serializer.Converters.Add(new CompositeValueCsvConverter());

            string[] csv = new string[] {
                "Single,Double",    // header
                "15,52.1",                                  // first record
                ",-15.2",                         // second record
                "0,0",                                       // third record
                ",",                                         // fourth null record
                ""                                             // empty line after the last record
            };

            List<CompositeObject> result = new List<CompositeObject>();

            using (var source = new StringReader(String.Join(Environment.NewLine, csv)))
            using (var reader = new CsvReader(source, serializer.Configuration))
            {
                var naming = new CsvHeaderNamingContext();

                if (reader.Read())
                {
                    converter.ReadCsvHeader(reader, serializer);
                }

                while (reader.Read())
                {
                    result.Add((CompositeObject)converter.ReadCsv(reader, typeof(CompositeObject), serializer, naming));
                }
            }

            Assert.AreEqual(4, result.Count);

            Assert.IsNotNull(result[0]);
            Assert.IsNotNull(result[0].Single);
            Assert.AreEqual(15, result[0].Single.Value);
            Assert.IsNull(result[0].Composite);
            Assert.AreEqual(52.1, result[0].Double);

            Assert.IsNotNull(result[1]);
            Assert.IsNull(result[1].Single);
            Assert.IsNull(result[1].Composite);
            Assert.AreEqual(-15.2, result[1].Double);

            Assert.IsNotNull(result[2]);
            Assert.IsNotNull(result[2].Single);
            Assert.AreEqual(0, result[2].Single.Value);
            Assert.IsNull(result[2].Composite);
            Assert.AreEqual(0, result[2].Double);

            Assert.IsNull(result[3]);
        }
    }
}
