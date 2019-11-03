using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using UXI.Serialization.Fakes;
using UXI.Serialization.Fakes.Json.Converters;

namespace UXI.Serialization.Formats.Json.Converters
{
    [TestClass]
    public class GenericJsonConverterTest
    {
        [TestMethod]
        public void Convert_EmptyObjectToCompositeStruct_ReturnsInstance()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new CompositeStructJsonConverter());

            var expected = new CompositeStruct();
            string json = "{}";

            using (var sr = new StringReader(json))
            {
                var value = serializer.Deserialize(sr, typeof(CompositeStruct));

                Assert.AreEqual(expected, value);
            }
        }


        [TestMethod]
        public void Convert_ValidStringToCompositeStruct_ReturnsInstance()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new CompositeStructJsonConverter());

            var expected = new CompositeStruct() { Id = 1, Duration = 4.5 };
            string json = "{ \"id\": 1, \"duration\": 4.5 }";

            using (var sr = new StringReader(json))
            {
                var value = serializer.Deserialize(sr, typeof(CompositeStruct));

                Assert.AreEqual(expected, value);
            }
        }


        [TestMethod]
        public void Convert_NullToCompositeStruct_ReturnsDefaultInstance()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new CompositeStructJsonConverter());

            var expected = new CompositeStruct();
            string json = "null";

            using (var sr = new StringReader(json))
            {
                var value = serializer.Deserialize(sr, typeof(CompositeStruct));

                Assert.AreEqual(expected, value);
            }
        }


        [TestMethod]
        public void Convert_NullToNullableOfCompositeStruct_ReturnsNull()
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new CompositeStructJsonConverter());

            CompositeStruct? expected = null;
            string json = "null";

            using (var sr = new StringReader(json))
            {
                var value = serializer.Deserialize(sr, typeof(CompositeStruct?));

                Assert.AreEqual(expected, value);
            }
        }
    }
}
