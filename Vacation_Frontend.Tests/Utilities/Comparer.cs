using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Vacation_Frontend.Tests.Utilities
{
    public class Comparer
    {
        public static Comparer<U?> Get<U>(Func<U?, U?, bool> predicate)
        {
            return new Comparer<U?>(predicate);
        }
    }

    public class Comparer<T> : Comparer, IEqualityComparer<T>
    {
        private Func<T?, T?, bool> func;

        public Comparer(Func<T?, T?, bool> func) { this.func = func; }

        public bool Equals(T? x, T? y)
        {
            return func(x, y);
        }

        public int GetHashCode([DisallowNull] T obj)
        {
            return obj?.GetHashCode() ?? 0;
        }
    }
} 