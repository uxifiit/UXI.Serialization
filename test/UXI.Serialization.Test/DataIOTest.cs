using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UXI.Serialization.Csv;
using UXI.Serialization.Json;

namespace UXI.Serialization
{
    [TestClass]
    public class DataIOTest
    {
        [TestMethod]
        public void EnsureCorrectFileFormat_MismatchedWithExistingFormat_CorrectsFormat()
        {
            var io = new DataIO(new JsonSerializationFactory(), new CsvSerializationFactory());

            var expected = FileFormat.CSV;

            var output = io.EnsureCorrectFileFormat("P003_ET_data.fix3.csv", FileFormat.JSON);

            Assert.AreEqual(expected, output);
        }


        [TestMethod]
        public void EnsureCorrectFileFormat_MismatchedWithMissingFormat_CorrectsFormat()
        {
            var io = new DataIO(new JsonSerializationFactory());

            var expected = FileFormat.JSON;

            var output = io.EnsureCorrectFileFormat("P003_ET_data.fix3.csv", FileFormat.JSON);

            Assert.AreEqual(expected, output);
        }


        [TestMethod]
        public void EnsureCorrectFileFormat_MatchingWithMissingFormat_UsesRequested()
        {
            var io = new DataIO(new JsonSerializationFactory());

            var expected = FileFormat.CSV;

            var output = io.EnsureCorrectFileFormat("P003_ET_data.fix3.csv", FileFormat.CSV);

            Assert.AreEqual(expected, output);
        }


        [TestMethod]
        public void EnsureCorrectFileFormat_MatchingWithExistingFormat_UsesExisting()
        {
            var io = new DataIO(new JsonSerializationFactory());

            var expected = FileFormat.JSON;

            var output = io.EnsureCorrectFileFormat("P003_ET_data.fix3.json", FileFormat.JSON);

            Assert.AreEqual(expected, output);
        }
    }
}
