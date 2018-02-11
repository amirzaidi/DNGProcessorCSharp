using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DNGProcessor
{
    class Program
    {
        enum TIFFTag
        {
            NewSubfileType = 254,
            ImageWidth = 256,
            ImageLength = 257,
            BitsPerSample = 258,
            Compression = 259,
            PhotometricInterpretation = 262,
            ImageDescription = 270,
            Make = 271,
            Model = 272,
            StripOffsets = 273,
            Orientation = 274,
            SamplesPerPixel = 277,
            RowsPerStrip = 278,
            StripByteCounts = 279,
            XResolution = 282,
            YResolution = 283,
            PlanarConfiguration = 284,
            ResolutionUnit = 296,
            Software = 305,
            Hardware = 306,
            CFARepeatPatternDim = 33421,
            CFAPattern = 33422,
            Copyright = 33432,
            ExposureTime = 33434,
            FNumber = 33437,
            ISOSpeedRatings = 34855,
            DateTimeOriginal = 36867,
            FocalLength = 37386,
            EPStandardID = 37398,
            DNGVersion = 50706,
            DNGBackwardVersion = 50707,
            UniqueCameraModel = 50708,
            CFAPlaneColor = 50710,
            CFALayout = 50711,
            BlackLevelRepeatDim = 50713,
            BlackLevel = 50714,
            WhiteLevel = 50717,
            DefaultScale = 50718,
            DefaultCropOrigin = 50719,
            DefaultCropSize = 50720,
            ColorMatrix1 = 50721,
            ColorMatrix2 = 50722,
            CameraCalibration1 = 50723,
            CameraCalibration2 = 50724,
            AsShotNeutral = 50728,
            CalibrationIlluminant1 = 50778,
            CalibrationIlluminant2 = 50779,
            ActiveArea = 50829,
            ForwardMatrix1 = 50964,
            ForwardMatrix2 = 50965,
            OpcodeList2 = 51009,
            OpcodeList3 = 51022,
            NoiseProfile = 51041
        }

        enum TIFFTagValueType
        {
            Byte = 1,
            String = 2,
            Int16 = 3,
            Int32 = 4,
            Frac = 5,
            Undef = 7,
            SFrac = 10,
            Double = 12
        }

        static Dictionary<TIFFTagValueType, int> unitLengths = new Dictionary<TIFFTagValueType, int>()
        {
            { TIFFTagValueType.Byte, 1 },
            { TIFFTagValueType.String, 1 },
            { TIFFTagValueType.Int16, 2 },
            { TIFFTagValueType.Int32, 4 },
            { TIFFTagValueType.Frac, 8 },
            { TIFFTagValueType.Undef, 0 },
            { TIFFTagValueType.SFrac, 8 },
            { TIFFTagValueType.Double, 8 },
        };

        static void Main(string[] args)
        {
            byte[] b = File.ReadAllBytes("C:/Users/Amir/Downloads/Raw/1.dng");

            int offset = 0;
            var format = Encoding.ASCII.GetString(b, offset, 2);
            offset += 2;

            var version = BitConverter.ToUInt16(b, offset);
            offset += 2;

            var imgDir = BitConverter.ToInt32(b, offset);
            offset += 4;

            if (format == "II" && version == 42)
            {
                while (true)
                {
                    int tagCount = b[offset++] + (b[offset++] << 8);
                    for (int i = 0; i < tagCount; i++)
                    {
                        var tag = (TIFFTag)BitConverter.ToUInt16(b, offset);
                        offset += 2;

                        var type = (TIFFTagValueType)BitConverter.ToUInt16(b, offset);
                        offset += 2;

                        int length = BitConverter.ToInt32(b, offset);
                        offset += 4;

                        byte[] d = null;
                        int dataPos = 0;

                        int unitLength = unitLengths[type];
                        int byteLength = length * unitLength;
                        if (byteLength != 0)
                        {
                            d = new byte[byteLength];

                            if (byteLength <= 4)
                            {
                                Buffer.BlockCopy(b, offset, d, 0, d.Length);
                            }
                            else
                            {
                                dataPos = BitConverter.ToInt32(b, offset);
                                Buffer.BlockCopy(b, dataPos, d, 0, d.Length);
                            }
                        }
                        offset += 4;

                        var stringData = new List<string>();;
                        if (d != null) //Should always happen
                        {
                            if (type == TIFFTagValueType.Byte)
                            {
                                stringData.Add(String.Join(" ", d));
                            }
                            else if (type == TIFFTagValueType.String)
                            {
                                stringData.Add('"' + Encoding.ASCII.GetString(d, 0, d.Length - 1) + '"');
                            }
                            else if (type == TIFFTagValueType.Int16 || type == TIFFTagValueType.Int32)
                            {
                                for (int j = 0; j < length; j++)
                                {
                                    int data = 0;
                                    for (int k = 0; k < unitLength; k++)
                                    {
                                        int index = k + j * unitLength;
                                        //For inline data
                                        if (index >= d.Length)
                                            break;

                                        data += d[k + j * unitLength] << (8 * k);
                                    }

                                    stringData.Add(data.ToString());
                                }
                            }
                            else if (type == TIFFTagValueType.Frac)
                            {
                                for (int j = 0; j < byteLength; j += unitLength)
                                {
                                    var num = BitConverter.ToUInt32(d, j);
                                    var denom = BitConverter.ToUInt32(d, j + 4);

                                    stringData.Add(num.ToString() + '/' + denom);
                                }
                            }
                            else if (type == TIFFTagValueType.SFrac)
                            {
                                for (int j = 0; j < byteLength; j += unitLength)
                                {
                                    var num = BitConverter.ToInt32(d, j);
                                    var denom = BitConverter.ToInt32(d, j + 4);

                                    stringData.Add(num.ToString() + '/' + denom);
                                }
                            }
                            else if (type == TIFFTagValueType.Double)
                            {
                                for (int j = 0; j < byteLength; j += unitLength)
                                {
                                    var dbl = BitConverter.ToDouble(d, j);
                                    stringData.Add(dbl.ToString());
                                }
                            }
                        }

                        int count = stringData.Count;
                        if (count == 0)
                        {
                            stringData.Add($"*{dataPos}");
                        }
                        else if (count > 9)
                        {
                            stringData = stringData.GetRange(0, 9);
                            stringData.Add($"({(count - 9)} more..)");
                        }

                        Console.WriteLine(type + "\tL" + length + "\t" + tag.ToString().PadRight(25) + "\t" + string.Join(" ", stringData));
                    }

                    break;
                }
            }

            while (true) Console.ReadKey();
        }
    }
}
