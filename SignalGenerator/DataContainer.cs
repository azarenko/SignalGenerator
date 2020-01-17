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

        public Bitmap GetPicture()
        {
            Bitmap img = new Bitmap(dataSet.Count * 10, (int)short.MaxValue * 4);
            Graphics flagGraphics = Graphics.FromImage(img);



            int red = 0;
            int white = 11;

            while (white <= 100)
            {
                flagGraphics.FillRectangle(Brushes.Red, 0, red, 200, 10);
                flagGraphics.FillRectangle(Brushes.White, 0, white, 200, 10);
                red += 20;
                white += 20;
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
