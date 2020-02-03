using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace SignalGenerator
{
    class DataContainer
    {
        public List<DataItem> dataSet = new List<DataItem>();

        public DataContainer(string filePath)
        {
            using (StreamReader sr = new StreamReader(filePath))
            {
                while (!sr.EndOfStream)
                {
                    string[] data = sr.ReadLine().Split(',');
                    dataSet.Add(new DataItem()
                    {
                        Channel0 = byte.Parse(data[0]),
                        Channel1 = byte.Parse(data[1]),
                        Channel2 = byte.Parse(data[2]),
                        Channel3 = byte.Parse(data[3]),
                        Channel4 = byte.Parse(data[4]),
                        Channel5 = byte.Parse(data[5]),
                        Channel6 = byte.Parse(data[6]),
                        Channel7 = byte.Parse(data[7]),
                    });
                }
            }
        }

        public Bitmap GetPicture(int h)
        {
            const int k = 10;

            Bitmap img = new Bitmap(dataSet.Count * k, h);
            Graphics imgGraphics = Graphics.FromImage(img);

            Pen pen0 = new Pen(Color.Blue, 2);
            Pen pen1 = new Pen(Color.Red, 2);
            Pen pen2 = new Pen(Color.Green, 2);
            Pen pen3 = new Pen(Color.Brown, 2);
            Pen pen4 = new Pen(Color.DeepPink, 2);
            Pen pen5 = new Pen(Color.Orange, 2);
            Pen pen6 = new Pen(Color.Gray, 2);
            Pen pen7 = new Pen(Color.Violet, 2);

            float kY = (float)h / 8;

            for (int i = 1; i < img.Width; i++)
            {
                int y1 = (int)(kY - (dataSet[(i-1) / k].Channel0 * kY));
                int y2 = (int)(kY - (dataSet[i / k].Channel0 * kY));

                imgGraphics.DrawLine(pen0, i - 1,  y1, i, y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel1 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel1 * kY));

                imgGraphics.DrawLine(pen1, i - 1, kY * 1 + y1, i, kY * 1 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel2 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel2 * kY));

                imgGraphics.DrawLine(pen2, i - 1, kY * 2 + y1, i, kY * 2 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel3 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel3 * kY));

                imgGraphics.DrawLine(pen3, i - 1, kY * 3 + y1, i, kY * 3 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel4 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel4 * kY));

                imgGraphics.DrawLine(pen4, i - 1, kY * 4 + y1, i, kY * 4 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel5 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel5 * kY));

                imgGraphics.DrawLine(pen5, i - 1, kY * 5 + y1, i, kY * 5 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel6 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel6 * kY));

                imgGraphics.DrawLine(pen6, i - 1, kY * 6 + y1, i, kY * 6 + y2);

                y1 = (int)(kY - (dataSet[(i - 1) / k].Channel7 * kY));
                y2 = (int)(kY - (dataSet[i / k].Channel7 * kY));

                imgGraphics.DrawLine(pen7, i - 1, kY * 7 + y1, i, kY * 7 + y2);
            }

            return img;
        }
    }

    struct DataItem
    {
        public byte Channel0;
        public byte Channel1;
        public byte Channel2;
        public byte Channel3;
        public byte Channel4;
        public byte Channel5;
        public byte Channel6;
        public byte Channel7;
    }
}
