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


        protected abstract TSerializer Configure(TSerializer serializer, DataAccess acess, object settings);


        public object Configure(object serializer, DataAccess access, object settings)
        {
            return (serializer is TSerializer)
                 ? Configure((TSerializer)serializer, access, settings)
                 : serializer;
        }
    }



    public abstract class SerializationConfiguration<TSerializer, TSettings> : SerializationConfiguration<TSerializer>
        where TSerializer : class
    {
        protected abstract TSerializer Configure(TSerializer serializer, DataAccess access, TSettings settings);


        protected sealed override TSerializer Configure(TSerializer serializer, DataAccess access, object settings)
        {
            var typedSettings = (settings is TSettings)
                              ? (TSettings)settings 
                              : default(TSettings);

            return Configure(serializer, access, typedSettings);
        }
    }
}
