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
                        ChannelA = short.Parse(data[0]),
                        ChannelB = short.Parse(data[1])
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


            float kY = h / (float)(short.MaxValue * 8);
            float offset = h / 8;

            for (int i = 1; i < img.Width; i++)
            {
                int y1 = (int)((short.MaxValue - (dataSet[(i-1) / k].ChannelA)) * kY);
                int y2 = (int)((short.MaxValue - (dataSet[i / k].ChannelA)) * kY);

                imgGraphics.DrawLine(penA, i - 1, offset * 2 + y1, i, offset * 2 + y2);

                y1 = (int)((short.MaxValue - (dataSet[(i - 1) / k].ChannelB)) * kY);
                y2 = (int)((short.MaxValue - (dataSet[i / k].ChannelB)) * kY);

                imgGraphics.DrawLine(penB, i - 1, offset * 4 + y1, i, offset * 4 + y2);
            }

            return img;
        }
    }

    struct DataItem
    {
        public short ChannelA;
        public short ChannelB;
    }
}
