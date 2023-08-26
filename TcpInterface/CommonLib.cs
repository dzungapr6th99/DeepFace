using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpInterface
{
    public static class ArrayExtensions
    {
        public static void Merge<T>(this T[] x, T[] y, ref int index)
        {
            //if (x == null) return x;
            if (y == null) return;
            int oldLen = index;
            if (index + y.Length > x.Length)
                Array.Resize(ref x, x.Length * 2);
            Buffer.BlockCopy(y, 0, x, oldLen, y.Length);
            index += y.Length;

        }



        public static T[] RemoveAt<T>(this T[] source, int index)
        {
            if (source == null || source.Length == 0) return source;
            if (index < 0 || index > (source.Length - 1)) return source;

            T[] dest = new T[source.Length - 1];
            if (dest.Length == 0)
            {
                return dest;
            }

            if (index > 0)
            {
                Buffer.BlockCopy(source, 0, dest, 0, index);
            }

            if (index < source.Length - 1)
            {
                Buffer.BlockCopy(source, index + 1, dest, index, source.Length - index - 1);
            }

            return dest;
        }

        public static T[] Remove<T>(this T[] source, int index, int length)
        {
            if (source == null || source.Length == 0) return source;
            if (index < 0 || index > (source.Length - 1)) return source;

            T[] dest = new T[source.Length];
            Buffer.BlockCopy(source, 0, dest, 0, source.Length);

            for (int i = 0; i < length; i++)
            {
                dest = dest.RemoveAt(index);
            }

            return dest;
        }

        public static T[] GetCopy<T>(this T[] source, int index, int length)
        {
            T[] dest = new T[length];
            Buffer.BlockCopy(source, index, dest, 0, length);
            return dest;
        }
    }
}
