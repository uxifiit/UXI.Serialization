using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Formats.Csv
{
    public class CsvHeaderNamingContext
    {
        private readonly Func<string, string> _prepareHeaderName;
        private readonly string _prefix = String.Empty;
        private readonly string _delimiter = String.Empty;

        public CsvHeaderNamingContext() { }
        

        public CsvHeaderNamingContext(Func<string, string> prepareHeaderName)
        {
            _prepareHeaderName = prepareHeaderName;
        }


        public CsvHeaderNamingContext(string prefix, Func<string, string> prepareHeaderName)
            : this(prepareHeaderName)
        {
            _prefix = prefix?.Trim() ?? String.Empty;
        }


        public CsvHeaderNamingContext(string prefix, string delimiter, Func<string, string> prepareHeaderName)
            : this(prefix, prepareHeaderName)
        {
            _delimiter = delimiter?.Trim() ?? String.Empty;
        }


        public string Get(string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName), "Property name cannot be null, empty, nor whitespace.");
            }

            string prefix = _prefix;

            string name = propertyName.Trim();

            string delimiter = String.IsNullOrWhiteSpace(prefix)
                             ? String.Empty
                             : _delimiter;

            return prefix + delimiter + (_prepareHeaderName?.Invoke(name) ?? name);
        }


        public string GetDefault(string defaultName)
        {
            if (String.IsNullOrWhiteSpace(defaultName))
            {
                throw new ArgumentNullException(nameof(defaultName), "Default name cannot be null, empty, nor whitespace.");
            }

            string prefix = _prefix;

            string name = String.IsNullOrWhiteSpace(prefix)
                        ? defaultName.Trim()
                        : String.Empty;

            string delimiter = String.IsNullOrWhiteSpace(prefix) || String.IsNullOrWhiteSpace(name)
                             ? String.Empty
                             : _delimiter;

            return prefix + delimiter + (_prepareHeaderName?.Invoke(name) ?? name);
        }


        public CsvHeaderNamingContext GetNextLevel(string name)
        {
            return GetNextLevel(name, _delimiter);
        }


        public CsvHeaderNamingContext GetNextLevel(string name, string delimiter)
        {
            return new CsvHeaderNamingContext(Get(name), delimiter, _prepareHeaderName);
        }
    }
}
