using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UXI.Serialization.Common;

namespace UXI.Serialization.Formats.Json
{
    public class JsonDataWriter : DisposableBase, IDataWriter, IDisposable
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonTextWriter _writer;
        private bool _isOpen = true;

        private ConcurrentQueue<object> _cache = new ConcurrentQueue<object>();

        public JsonDataWriter(TextWriter writer, JsonSerializer serializer)
        {
            _serializer = serializer;

            _writer = new JsonTextWriter(writer)
            {
                Culture = serializer.Culture,
                DateTimeZoneHandling = serializer.DateTimeZoneHandling,
                DateFormatHandling = serializer.DateFormatHandling,
                AutoCompleteOnClose = true
            };
        }


        public bool CanWrite(Type objectType)
        {
            return _isOpen;
        }


        public void Close()
        {
            if (_isOpen)
            {
                // Check cache before closing. If it is non-empty, the serializer was called for write only once, 
                // what means it should serialize a single item, not an array. To not leave out the single item, 
                // it is serialized here before closing the writer.

                ConcurrentQueue<object> cache = _cache;
                _cache = null;

                if (cache != null)
                {
                    object cachedData;
                    if (cache.TryDequeue(out cachedData))
                    {
                        _serializer.Serialize(_writer, cachedData);
                    }
                }

                _isOpen = false;
                _writer.Flush();
            }
            _writer.Close();
        }


        public void Write(object data)
        {
            if (_isOpen)
            {
                // First check the cache. If it is first write, cache the object and do not serialize it yet.
                // If it is second write, first take the cached item, serialize it, then serialize the new data. Remove cache after second write.
                // The cache is used for disambiguation whether the serializer writes an array or a single item.

                // TODO This is not ideal solution to concurrent access to the cache, but is ok for now.
                ConcurrentQueue<object> cache = _cache;
                if (cache != null)
                {
                    object cachedData = null;

                    if (cache.IsEmpty)
                    {
                        cache.Enqueue(data);
                        return;
                    }
                    else if (cache.TryDequeue(out cachedData))
                    {
                        _cache = null;

                        _writer.WriteStartArray();

                        _serializer.Serialize(_writer, cachedData);
                    }
                }

                _serializer.Serialize(_writer, data);
            }
        }
        

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (_disposed == false)
            {
                if (disposing)
                {
                    Close();
                }
                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
