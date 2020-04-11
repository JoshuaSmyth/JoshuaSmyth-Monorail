using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Monorail.Audio
{
    public enum ALFormat
    {
        Mono8 = 0x1100,
        Mono16 = 0x1101,
        Stereo8 = 0x1102,
        Stereo16 = 0x1103,
        MonoIma4 = 0x1300,
        StereoIma4 = 0x1301,
        MonoMSAdpcm = 0x1302,
        StereoMSAdpcm = 0x1303,
        MonoFloat32 = 0x10010,
        StereoFloat32 = 0x10011,
    }

    public class WaveFileData
    {
        uint sampleRate;
        uint channelCount;
        uint formatParameter = 0;
        bool isADPCM = false;

        ALFormat SoundFormat;

        byte[] data;

        public byte[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public uint SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }

        public ALFormat SoundFormat1
        {
            get { return SoundFormat; }

            set { SoundFormat = value; }
        }

        // TODO?:(Joshua) Load From PackFile?

        // TODO?:(Joshua) Load From byteArray?

        public void LoadFromFile(string filename)
        {
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                LoadFromStream(fs);
            }
        }

        public void LoadFromStream(Stream stream)
        {
            using(BinaryReader reader = new BinaryReader(stream))
            {
                // Note:(Joshua)
                // Adapted from the FNA library

                var riff = new string(reader.ReadChars(4));
                if (riff != "RIFF") {
                    throw new NotSupportedException("File is not a wav file");
                }
                reader.ReadUInt32();    // Chunk Size
                var wave = new string(reader.ReadChars(4));
                if (wave != "WAVE") {
                    throw new NotSupportedException("File in not a wav file");
                }

                // Header
                var format = new string(reader.ReadChars(4));
                while(format != "fmt ")
                {
                    reader.ReadBytes(reader.ReadInt32());
                    format = new string(reader.ReadChars(4));
                }

                var chunkSize = reader.ReadInt32();

                // Header Information
                uint audioFormat = reader.ReadUInt16();
                channelCount = reader.ReadUInt16();
                SampleRate = reader.ReadUInt32();
                reader.ReadUInt32();                    // ByteRate
                var blockAlign = reader.ReadUInt16();
                var bitDepth = reader.ReadUInt16();     // Bits per sample

                if (audioFormat == 1) {
                    System.Diagnostics.Debug.Assert(bitDepth == 8 || bitDepth == 16);
                    formatParameter = (uint)(bitDepth / 16); // 1 for 16, 0 for 8
                } else if(audioFormat != 2) {
                    isADPCM = true;
                    formatParameter = (((blockAlign / channelCount) - 6) * 2);
                } else {
                    throw new NotSupportedException("Wave Format not supported.");
                }

                // Read Remaining Bytes
                if (chunkSize > 16) {
                    reader.ReadBytes(chunkSize - 16);
                }

                // Data Signature
                var dataSignature = new string(reader.ReadChars(4));
                while(dataSignature.ToLowerInvariant() != "data") {
                    reader.ReadBytes(reader.ReadInt32());
                    dataSignature = new string(reader.ReadChars(4));
                }
                if (dataSignature != "data") {
                    throw new NotSupportedException("Specified wav file is not supported");
                }
                int waveDataLength = reader.ReadInt32();
                Data = reader.ReadBytes(waveDataLength);

                SoundFormat1 = GetSoundFormat((int)formatParameter, (int) channelCount, (int)SampleRate);
            }
        }

        internal const int FormatPcm = 1;
        internal const int FormatMsAdpcm = 2;
        internal const int FormatIeee = 3;
        internal const int FormatIma4 = 17;

        public static ALFormat GetSoundFormat(int format, int channels, int bits)
        {
            // This was taken from monogame

            switch (format)
            {
                case FormatPcm:
                    // PCM
                    switch (channels)
                    {
                        case 1: return bits == 8 ? ALFormat.Mono8 : ALFormat.Mono16;
                        case 2: return bits == 8 ? ALFormat.Stereo8 : ALFormat.Stereo16;
                        default: throw new NotSupportedException("The specified channel count is not supported.");
                    }
                case FormatMsAdpcm:
                    // Microsoft ADPCM
                    switch (channels)
                    {
                        case 1: return ALFormat.MonoMSAdpcm;
                        case 2: return ALFormat.StereoMSAdpcm;
                        default: throw new NotSupportedException("The specified channel count is not supported.");
                    }
                case FormatIeee:
                    // IEEE Float
                    switch (channels)
                    {
                        case 1: return ALFormat.MonoFloat32;
                        case 2: return ALFormat.StereoFloat32;
                        default: throw new NotSupportedException("The specified channel count is not supported.");
                    }
                case FormatIma4:
                    // IMA4 ADPCM
                    switch (channels)
                    {
                        case 1: return ALFormat.MonoIma4;
                        case 2: return ALFormat.StereoIma4;
                        default: throw new NotSupportedException("The specified channel count is not supported.");
                    }
                default:
                    throw new NotSupportedException("The specified sound format (" + format.ToString() + ") is not supported.");
            }
        }
    }
}
