using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Configurations
{
    public abstract class SerializationConfiguration<TSerializer> : ISerializationConfiguration
        where TSerializer : class
    {
        public virtual Type SerializerType { get; } = typeof(TSerializer);


        protected abstract TSerializer Configure(TSerializer serializer, DataAccess acess, Type dataType, object settings);


        public object Configure(object serializer, DataAccess access, Type dataType, object settings)
        {
            return (serializer is TSerializer)
                 ? Configure((TSerializer)serializer, access, dataType, settings)
                 : serializer;
        }
    }



    public abstract class SerializationConfiguration<TSerializer, TSettings> : SerializationConfiguration<TSerializer>
        where TSerializer : class
    {
        protected abstract TSerializer Configure(TSerializer serializer, DataAccess access, Type dataType, TSettings settings);


        protected sealed override TSerializer Configure(TSerializer serializer, DataAccess access, Type dataType, object settings)
        {
            var typedSettings = (settings is TSettings)
                              ? (TSettings)settings 
                              : default(TSettings);

            return Configure(serializer, access, dataType, typedSettings);
        }
    }
}
