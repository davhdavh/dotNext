using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Cheats
{
    using Runtime.InteropServices;

    /// <summary>
    /// Provides fast memory operations to work with value type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ValueType<T>
        where T: struct
    {
        private sealed class BitwiseComparer: IEqualityComparer<T>, IComparer<T>
        {
            internal static readonly BitwiseComparer Instance = new BitwiseComparer();
            private BitwiseComparer()
            {
            }

            bool IEqualityComparer<T>.Equals(T first, T second) => ValueType<T>.Equals(first, second);

            int IEqualityComparer<T>.GetHashCode(T obj) => ValueType<T>.GetHashCode(obj);
            int IComparer<T>.Compare(T first, T second) => ValueType<T>.Compare(first, second);
        }

        /// <summary>
        /// Size of value type, in bytes.
        /// </summary>
        public static readonly int Size = Unsafe.SizeOf<T>();

        /// <summary>
        /// Default value of type <typeparamref name="T"/>.
        /// </summary>
        public static T Default => default;

        public static readonly bool IsPrimitive = typeof(T).IsPrimitive;

        /// <summary>
        /// Equality comparer for the value type based on its bitwise representation.
        /// </summary>
        public static IEqualityComparer<T> EqualityComparer
        {
            get
            {
                if(IsPrimitive)
                    return EqualityComparer<T>.Default;
                else
                    return BitwiseComparer.Instance;
            }
        }

        /// <summary>
        /// Value comparer for the value type based on its bitwise representation.
        /// </summary>
        public static IComparer<T> Comparer
        {
            get
            {
                if(IsPrimitive)
                    return Comparer<T>.Default;
                else
                    return BitwiseComparer.Instance;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals<U>(T first, U second)
			where U: struct
            => Size == ValueType<U>.Size && Memory.Equals(Unsafe.AsPointer(ref first), Unsafe.AsPointer(ref second), Size);
                

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe bool Equals(T first, T second)
            => Memory.Equals(Unsafe.AsPointer(ref first), Unsafe.AsPointer(ref second), Size);
        
        public static unsafe int GetHashCode(T value, int hash, Func<int, int, int> hashFunction, bool useSalt = true)
			=> Memory.GetHashCode(Unsafe.AsPointer(ref value), Size, hash, hashFunction, useSalt);

		/// <summary>
		/// Computes hash code for the structure content.
		/// </summary>
		/// <param name="value"></param>
		/// <returns>Content hash code.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]      
		public static unsafe int GetHashCode(T value, bool useSalt = true)
            => Memory.GetHashCode(Unsafe.AsPointer(ref value), Size, useSalt);
        
        internal unsafe static ReadOnlySpan<byte> RawBits(ref T value)
            => new ReadOnlySpan<byte>(Unsafe.AsPointer(ref value), Size);
        
        /// <summary>
        /// Indicates that specified value type is the default value.
        /// </summary>
        /// <param name="value">Value to check.</param>
        /// <returns></returns>
        public static bool IsDefault(T value)
            => Equals(value, default(T));

        public static byte[] AsBinary(T value)
            => RawBits(ref value).ToArray();

        public static unsafe int Compare(T first, T second)
            => Memory.Compare(Unsafe.AsPointer(ref first), Unsafe.AsPointer(ref second), Size);
        
        public static unsafe int Compare<U>(T first, U second)
            where U: struct
            => Size == ValueType<U>.Size ? 
					Memory.Compare(Unsafe.AsPointer(ref first), Unsafe.AsPointer(ref second), Size) :
					Size.CompareTo(ValueType<U>.Size);
    }
}