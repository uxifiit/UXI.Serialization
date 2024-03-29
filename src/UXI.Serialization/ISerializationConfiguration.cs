﻿using System;

namespace UXI.Serialization
{
    public interface ISerializationConfiguration
    {
        Type SerializerType { get; }

        object Configure(object serializer, DataAccess access, Type dataType, object settings);
    }
}
