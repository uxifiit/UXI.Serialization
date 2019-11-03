﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UXI.Serialization.Reactive.Extensions
{
    public static class IDataWriterEx
    {
        public static IObserver<T> ToObserver<T>(this IDataWriter writer)
        {
            return System.Reactive.Observer.Create<T>
            (
                onNext: data => writer.Write(data),
                onError: _ => writer.Close(),
                onCompleted: () => writer.Close()
            );
        }
    }
}
