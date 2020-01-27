using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SignalGenerator
{
    class DataContainer
    {
        private List<DataItem> dataSet = new List<DataItem>();

        public DataContainer(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string[] data = sr.ReadLine().Split(',');
                    dataSet.Add(new DataItem()
                    {
                        ChannelA = ushort.Parse(data[0]),
                        ChannelB = ushort.Parse(data[1])
                    });
                }
            }
        }

        public Bitmap GetPicture(int h)
        {
            const int k = 10;

            Bitmap img = new Bitmap(dataSet.Count * k, h);
            Graphics imgGraphics = Graphics.FromImage(img);

            Pen penA = new Pen(Color.Blue, 2);
            Pen penB = new Pen(Color.Red, 2);


            float kY = h / (float)(ushort.MaxValue * 8);
            float offset = h / 8;

            for (int i = 1; i < img.Width; i++)
            {
                int y1 = (int)((ushort.MaxValue - (dataSet[(i-1) / k].ChannelA)) * kY);
                int y2 = (int)((ushort.MaxValue - (dataSet[i / k].ChannelA)) * kY);

                imgGraphics.DrawLine(penA, i - 1, offset * 2 + y1, i, offset * 2 + y2);

                y1 = (int)((ushort.MaxValue - (dataSet[(i - 1) / k].ChannelB)) * kY);
                y2 = (int)((ushort.MaxValue - (dataSet[i / k].ChannelB)) * kY);

                imgGraphics.DrawLine(penB, i - 1, offset * 4 + y1, i, offset * 4 + y2);
            }

            return img;
        }

        public byte[] GetSoundBuffer(int rpm)
        {
            ushort[] left = null;
            ushort[] right = null;
            FillSamplesBuffer(ref left,ref right, rpm);
            return GenerateWav(left, right);
        }

        private void FillSamplesBuffer(ref ushort[] left, ref ushort[] right, int rpm)
        {
            double pointPerSec = ((rpm / 2.0) / 60.0) * (double)dataSet.Count;
            double k = pointPerSec / (44100.0 * 2.0);

            List<ushort> leftBuffer = new List<ushort>();
            List<ushort> rightBuffer = new List<ushort>();

            for (int i = 0; k * i < dataSet.Count; i++)
            {
                leftBuffer.Add(dataSet[(int)(k * i)].ChannelA);
                rightBuffer.Add(dataSet[(int)(k * i)].ChannelB);
            }

            left = leftBuffer.ToArray();
            right = rightBuffer.ToArray();
        }

        private byte[] GenerateWav(ushort[] left, ushort[] right)
        {
            const int mNumChannels = 2;
            const int mSampleRateHz = 44100;
            const int mBitsPerSample = 16;

            MemoryStream mMemoryStream = new MemoryStream();

            // RIFF chunk (12 bytes total)
            // Write the chunk IDD ("RIFF", 4 bytes)
            byte[] buffer = StrToByteArray("RIFF");
            mMemoryStream.Write(buffer, 0, 4);      // gets stuck here..won't write to the stream

            // File size size (4 bytes)
            buffer = BitConverter.GetBytes(left.Length * 4 + 36);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            mMemoryStream.Write(buffer, 0, 4);

            // RIFF type ("WAVE")
            buffer = StrToByteArray("WAVE");
            mMemoryStream.Write(buffer, 0, 4);

            // Format chunk (24 bytes total)
            // "fmt " (ASCII characters)
            buffer = StrToByteArray("fmt ");
            mMemoryStream.Write(buffer, 0, 4);

            // Length of format chunk (always 16, 4 bytes)
            Array.Clear(buffer, 0, buffer.GetLength(0));
            buffer[0] = 16;
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            mMemoryStream.Write(buffer, 0, 4);

            // 2 bytes (always 1)
            Array.Clear(buffer, 0, buffer.GetLength(0));
            buffer[0] = 1;
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer, 0, 2);
            mMemoryStream.Write(buffer, 0, 2);

            // # of channels (2 bytes)
            Array.Clear(buffer, 0, buffer.GetLength(0));
            buffer[0] = mNumChannels;
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer, 0, 2);
            mMemoryStream.Write(buffer, 0, 2);

            // Sample rate (4 bytes)
            buffer = BitConverter.GetBytes(mSampleRateHz);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            mMemoryStream.Write(buffer, 0, 4);

            // Calculate the # of bytes per sample: 1=8 bit Mono, 2=8 bit Stereo or
            // 16 bit Mono, 4=16 bit Stereo
            short bytesPerSample = (short)((mBitsPerSample / 8) * 2);

            // Write the # of bytes per second (4 bytes)
            int mBytesPerSec = mSampleRateHz * bytesPerSample;
            buffer = BitConverter.GetBytes(mBytesPerSec);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            mMemoryStream.Write(buffer, 0, 4);

            // Write the # of bytes per sample (2 bytes)
            byte[] buffer_2bytes = BitConverter.GetBytes(bytesPerSample);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer_2bytes);
            mMemoryStream.Write(buffer_2bytes, 0, 2);

            // Bits per sample (2 bytes)
            buffer_2bytes = BitConverter.GetBytes(mBitsPerSample);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer_2bytes);
            mMemoryStream.Write(buffer_2bytes, 0, 2);

            // Data chunk
            // "data" (ASCII characters)
            buffer = StrToByteArray("data");
            mMemoryStream.Write(buffer, 0, 4);
            // Length of data to follow (4 bytes)
            buffer = BitConverter.GetBytes(left.Length * 4 + 44);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(buffer);
            mMemoryStream.Write(buffer, 0, 4);

            for (int i = 0; i < left.Length; i++)
            {
                buffer = BitConverter.GetBytes(left[i]);
                mMemoryStream.Write(buffer, 0, 2);
                buffer = BitConverter.GetBytes(right[i]);
                mMemoryStream.Write(buffer, 0, 2);
            }

            byte[] outbuffer = mMemoryStream.ToArray();
            mMemoryStream.Close();

            return outbuffer;
        }

        private static byte[] StrToByteArray(String pStr)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(pStr);
        }
    }

    struct DataItem
    {
        public ushort ChannelA;
        public ushort ChannelB;
    }
}
