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

        public Bitmap GetPicture(int w, int h)
        {
            const int k = 64;

            Bitmap img = new Bitmap(w, h);
            Graphics imgGraphics = Graphics.FromImage(img);

            Pen penA = new Pen(Color.Blue);
            Pen penB = new Pen(Color.Red);

            for (int i = 1; i < img.Width; i++)
            {
                int y1 = dataSet[(i-1) / k].ChannelA;
                int y2 = dataSet[i / k].ChannelA;

                imgGraphics.DrawLine(penA, i - 1, y1, i, y2);

                y1 = dataSet[(i - 1) / k].ChannelB;
                y2 = dataSet[i / k].ChannelB;

                imgGraphics.DrawLine(penB, i - 1, y1, i, y2);
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
