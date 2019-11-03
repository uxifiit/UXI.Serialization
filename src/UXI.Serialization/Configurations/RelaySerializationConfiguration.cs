using System;

namespace UXI.Serialization.Configurations
{
    public class RelaySerializationConfiguration<TSerializer> : SerializationConfiguration<TSerializer>
        where TSerializer : class
    {
        private readonly Func<TSerializer, DataAccess, object, TSerializer> _configuration;


        public RelaySerializationConfiguration(Func<TSerializer, DataAccess, object, TSerializer> configuration)
        {
            _configuration = configuration;
        }


        protected override TSerializer Configure(TSerializer serializer, DataAccess access, object settings)
        {
            return _configuration?.Invoke(serializer, access, settings) 
                ?? serializer;
        }
    }
}
