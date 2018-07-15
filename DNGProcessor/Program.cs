using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNGProcessor
{
    partial class Program
    {
        static void Main(string[] args)
        {
            byte[] b = File.ReadAllBytes("C:/Users/Amir/Downloads/Raw/1.dng");

            int offset = 0;
            var format = Encoding.ASCII.GetString(b, offset, 2);
            offset += 2;

            if (format != "II")
                throw new Exception("Can only parse Intel byte order");

            var version = BitConverter.ToUInt16(b, offset);
            offset += 2;

            if (version != 42)
                throw new Exception("Can only parse v42");

            var start = BitConverter.ToInt32(b, offset);
            offset = start;

            var tags = parseTags(ref b, ref offset);

            foreach (var kvp in tags)
            {
                Console.Write(kvp.Key.ToString().PadRight(25) + "\t");
                var value = kvp.Value;
                if (value.Type == TIFFTagValueType.String)
                {
                    var bytes = new byte[value.Data.Length];
                    Array.Copy(value.Data, bytes, value.Data.Length);
                    Console.Write(Encoding.ASCII.GetString(bytes, 0, bytes.Length - 1));
                }
                else
                {
                    for (int elementNum = 0; elementNum < value.Data.Length && elementNum < 20; elementNum++)
                    {
                        var element = value.Data[elementNum];
                        if (element != null)
                        {
                            Console.Write(element.ToString() + " ");
                        }
                    }
                }

                Console.WriteLine();
            }

            // ?
            for (int i = offset; i < b.Length && i < offset + 200; i += 2)
            {
                int data = b[i];
                data += b[i] << 8;

                Console.Write(data.ToString() + " ");
            }

            while (true) Console.ReadKey();
        }

        private static Dictionary<TIFFTag, TagData> parseTags(ref byte[] b, ref int offset)
        {
            var result = new Dictionary<TIFFTag, TagData>();
            int tagCount = b[offset++] + (b[offset++] << 8);
            for (int tagNum = 0; tagNum < tagCount; tagNum++)
            {
                var tag = (TIFFTag)BitConverter.ToUInt16(b, offset);
                offset += 2;

                var type = (TIFFTagValueType)BitConverter.ToUInt16(b, offset);
                offset += 2;

                int elementCount = BitConverter.ToInt32(b, offset);
                offset += 4;

                int elementSize = unitLengths[type];

                var buffer = new byte[elementCount * elementSize];
                var values = new object[elementCount];
                
                if (buffer.Length <= 4)
                {
                    Buffer.BlockCopy(b, offset, buffer, 0, buffer.Length);
                }
                else
                {
                    int dataPos = BitConverter.ToInt32(b, offset);
                    Buffer.BlockCopy(b, dataPos, buffer, 0, buffer.Length);
                }

                for (int elementNum = 0; elementNum < elementCount; elementNum++)
                {
                    int index = elementNum * elementSize;

                    if (type == TIFFTagValueType.Byte || type == TIFFTagValueType.String)
                    {
                        values[index] = buffer[index];
                    }
                    else if (type == TIFFTagValueType.Int16 || type == TIFFTagValueType.Int32)
                    {
                        int data = 0;
                        for (int byteNum = 0; byteNum < elementSize; byteNum++)
                            data += buffer[index + byteNum] << (8 * byteNum);

                        values[elementNum] = data;
                    }
                    else if (type == TIFFTagValueType.Frac)
                    {
                        var num = BitConverter.ToUInt32(buffer, index);
                        var denom = BitConverter.ToUInt32(buffer, index + 4);

                        values[elementNum] = new Fraction(num, denom);
                    }
                    else if (type == TIFFTagValueType.SFrac)
                    {
                        var num = BitConverter.ToInt32(buffer, index);
                        var denom = BitConverter.ToInt32(buffer, index + 4);

                        values[elementNum] = new Fraction(num, denom);
                    }
                    else if (type == TIFFTagValueType.Double)
                    {
                        values[elementNum] = BitConverter.ToDouble(buffer, index);
                    }
                }

                result.Add(tag, new TagData(type, values));
                offset += 4;
            }

            return result;
        }
    }
}
