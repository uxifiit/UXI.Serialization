using System;

namespace UXI.Serialization.Configurations
{
    public class RelaySerializationConfiguration<TSerializer> : SerializationConfiguration<TSerializer>
        where TSerializer : class
    {
        private readonly Func<TSerializer, DataAccess, Type, object, TSerializer> _configuration;


        public RelaySerializationConfiguration(Func<TSerializer, DataAccess, Type, object, TSerializer> configuration)
        {
            _configuration = configuration;
        }


        protected override TSerializer Configure(TSerializer serializer, DataAccess access, Type dataType, object settings)
        {
            return _configuration?.Invoke(serializer, access, dataType, settings) 
                ?? serializer;
        }
    }
}
