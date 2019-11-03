using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Formats.Csv
{
    [TestClass]
    public class CsvHeaderNamingContextText
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_NullName_ThrowsException()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.Get(null);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Get_WhitespaceName_ThrowsException()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.Get(" ");
        }


        [TestMethod]
        public void Get_NoPrefixNoDelimWithName_ReturnsName()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.Get("Column");

            Assert.AreEqual("Column", name);
        }


        [TestMethod]
        public void Get_NoPrefixWithDelimWithName_ReturnsName()
        {
            var naming = new CsvHeaderNamingContext(String.Empty, ".", _ => _);

            string name = naming.Get("Column");

            Assert.AreEqual("Column", name);
        }

        
        [TestMethod]
        public void Get_WithPrefixNoDelimWithName_ReturnsPrefixName()
        {
            var naming = new CsvHeaderNamingContext("Object", _ => _);

            string name = naming.Get("Column");

            Assert.AreEqual("ObjectColumn", name);
        }


        [TestMethod]
        public void Get_WithPrefixWithDelimWithName_ReturnsPrefixDelimName()
        {
            var naming = new CsvHeaderNamingContext("Object", ".", _ => _);

            string name = naming.Get("Column");

            Assert.AreEqual("Object.Column", name);
        }


        [TestMethod]
        public void GetNextLevel_WithPrefixWithDelimiterWithName_GetReturnsPrefixDelimName()
        {
            var naming = new CsvHeaderNamingContext();

            var nextLevel = naming.GetNextLevel("Object", ".");

            string name = nextLevel.Get("Column");

            Assert.AreEqual("Object.Column", name);
        }


        [TestMethod]
        public void GetNextLevel_WithPrefixNoDelimiterWithName_GetReturnsPrefixName()
        {
            var naming = new CsvHeaderNamingContext();

            var nextLevel = naming.GetNextLevel("Object");

            string name = nextLevel.Get("Column");

            Assert.AreEqual("ObjectColumn", name);
        }


        [TestMethod]
        public void GetNextLevel_WithPrefixWithDelimiterNoName_GetDefaultReturnsPrefix()
        {
            var naming = new CsvHeaderNamingContext();

            var nextLevel = naming.GetNextLevel("Object", ".");

            string name = nextLevel.GetDefault("Value");

            Assert.AreEqual("Object", name);
        }


        [TestMethod]
        public void GetNextLevel_WithPrefixWithDelimiterNoName_Nested_GetDefaultReturnsPrefix()
        {
            var naming = new CsvHeaderNamingContext();

            var nextLevel = naming.GetNextLevel("Object", ".");
            var nextNextLevel = nextLevel.GetNextLevel("Nested");

            string name = nextNextLevel.GetDefault("Value");

            Assert.AreEqual("Object.Nested", name);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDefault_WhitespaceName_ThrowsException()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.GetDefault(" ");
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetDefault_NullName_ThrowsException()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.GetDefault(null);
        }


        [TestMethod]
        public void GetDefault_WithPrefixWithDelimNoName_ReturnsPrefix()
        {
            var naming = new CsvHeaderNamingContext("Object", ".", _ => _);

            string name = naming.GetDefault("Value");

            Assert.AreEqual("Object", name);
        }


        [TestMethod]
        public void GetDefault_NoPrefixNoDelimNoName_ReturnsDefault()
        {
            var naming = new CsvHeaderNamingContext();

            string name = naming.GetDefault("Value");

            Assert.AreEqual("Value", name);
        }


        [TestMethod]
        public void GetDefault_NoPrefixWithDelimNoName_ReturnsDefault()
        {
            var naming = new CsvHeaderNamingContext(String.Empty, ".", _ => _);

            string name = naming.GetDefault("Value");

            Assert.AreEqual("Value", name);
        }


        [TestMethod]
        public void GetDefault_WithPrefixNoDelimNoName_ReturnsPrefix()
        {
            var naming = new CsvHeaderNamingContext("Object", _ => _);

            string name = naming.GetDefault("Value");

            Assert.AreEqual("Object", name);
        }
    }
}
