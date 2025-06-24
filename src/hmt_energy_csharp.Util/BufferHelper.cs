using System.Runtime.InteropServices;

namespace hmt_energy_csharp
{
    [StructLayout(LayoutKind.Explicit)]
    public class ByteRingBuffer
    {
        [FieldOffset(0)]
        public long start = 0;

        [FieldOffset(64)]
        public long end = 0;

        [FieldOffset(128)]
        public byte[] buffer;

        public static string getMemory(object o)
        {
            GCHandle h = GCHandle.Alloc(o, GCHandleType.Pinned);
            IntPtr addr = h.AddrOfPinnedObject();
            return addr.ToString();
        }

        public ByteRingBuffer(int size)
        {
            buffer = new byte[size + 1];
        }

        public bool Empty()
        {
            return start == end;
        }

        public bool CanPush(int contentLength)
        {
            if (end >= start)
            {
                var freeLen = buffer.Length - (end - start);
                if (freeLen <= contentLength + 1)
                    return false;
            }
            else
            {
                var freeLen = start - end;
                if (freeLen <= contentLength + 1)
                    return false;
            }
            return true;
        }

        public bool Push(byte data)
        {
            if (end >= start)
            {
                var freeLen = buffer.Length - (end - start);
                if (freeLen <= 1)
                {
                    return false;
                }

                var cutLen0 = buffer.Length - end;
                if (cutLen0 >= 1)
                {
                    buffer[end] = data;
                    ++end;
                }
                else
                {
                    buffer[0] = data;
                    end = 1;
                }
            }
            else
            {
                var freeLen = start - end;
                if (freeLen <= 1)
                {
                    return false;
                }

                buffer[end] = data;
                ++end;
            }

            return true;
        }

        public byte Pop()
        {
            byte retObj = default;

            if (end == start)
            {
                return retObj;
            }
            else if (end > start)
            {
                var usedLen = end - start;
                if (usedLen < 1)
                {
                    return retObj;
                }

                retObj = buffer[start];
                ++start;
                return retObj;
            }
            else
            {
                var usedLen = buffer.Length - (start - end);
                if (usedLen < 1)
                {
                    return retObj;
                }

                var cutLen0 = buffer.Length - start;
                if (cutLen0 >= 1)
                {
                    retObj = buffer[start];
                    ++start;
                    return retObj;
                }
                else
                {
                    retObj = buffer[0];
                    start = 1;
                    return retObj;
                }
            }
        }
    }
}