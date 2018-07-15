using System.Collections.Generic;

namespace DNGProcessor
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

    struct TagData
    {
        internal TIFFTagValueType Type;
        internal object[] Data;

        internal TagData(TIFFTagValueType Type, object[] Data)
        {
            this.Type = Type;
            this.Data = Data;
        }
    }

    partial class Program
    {
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
    }
}
