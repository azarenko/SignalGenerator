using FTD2XX_NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace SignalGenerator
{
    public partial class MainForm : Form
    {
        private DataContainer _dataContainer = null;
        private FTD2XX_NET.FTDI _ftdi = new FTD2XX_NET.FTDI();
        private static bool isRun = false;

        public MainForm()
        {
            InitializeComponent();
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                _filePath.Text = openFileDialog1.FileName;
                _dataContainer = new DataContainer(_filePath.Text);
                _pictureBox.Image = _dataContainer.GetPicture(_pictureBox.Height);
            }
        }

        private void _start_Click(object sender, EventArgs e)
        {
            if (_dataContainer != null && !_ftdi.IsOpen)
            {
                if (_ftdi.OpenBySerialNumber((string)_deviceName.Text) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    MessageBox.Show("Can not open FTDI devices");
                    this.Close();
                }

                if (_ftdi.SetBitMode(0xFF, FTDI.FT_BIT_MODES.FT_BIT_MODE_ASYNC_BITBANG) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
                {
                    MessageBox.Show("Can not open FTDI devices");
                    this.Close();
                }

                isRun = true;
            }
        }

        private void _stop_Click(object sender, EventArgs e)
        {
            if (_dataContainer != null)
            {
                isRun = false;
                _ftdi.Close();
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            uint numOfDevices = 0;

            if (_ftdi.GetNumberOfDevices(ref numOfDevices) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
            {
                MessageBox.Show("Can not get the count of FTDI devices");
                this.Close();
            }

            if (numOfDevices == 0)
            {
                MessageBox.Show("Can not find any FTDI devices");
                this.Close();
            }

            FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[] devices = new FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE[numOfDevices];

            if (_ftdi.GetDeviceList(devices) != FTD2XX_NET.FTDI.FT_STATUS.FT_OK)
            {
                MessageBox.Show("Can not get the list of FTDI devices");
                this.Close();
            }

            foreach(FTD2XX_NET.FTDI.FT_DEVICE_INFO_NODE device in devices)
            {
                _deviceName.Items.Add(device.SerialNumber);
            }

            _deviceName.SelectedIndex = 0;

            backgroundWorker1.RunWorkerAsync();
        }

        private int GetDelay()
        {
            return (int)((1000L * 1000L * 60L) / (((long)_rpm.Value / 2) * _dataContainer.dataSet.Count));
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            uint w = 0;
            do
            {
                while (isRun && _ftdi.IsOpen)
                {
                    int delay = GetDelay();

                    for (int i = 0; i < _dataContainer.dataSet.Count; i++)
                    {
                        byte b = 0;

                        b += _dataContainer.dataSet[i].Channel0;
                        b += (byte)(_dataContainer.dataSet[i].Channel1 << 1);
                        b += (byte)(_dataContainer.dataSet[i].Channel2 << 2);
                        b += (byte)(_dataContainer.dataSet[i].Channel3 << 3);
                        b += (byte)(_dataContainer.dataSet[i].Channel4 << 4);
                        b += (byte)(_dataContainer.dataSet[i].Channel5 << 5);
                        b += (byte)(_dataContainer.dataSet[i].Channel6 << 6);
                        b += (byte)(_dataContainer.dataSet[i].Channel7 << 7);

                        b = (byte)~b;

                        _ftdi.Write(new byte[] { b }, 1, ref w);
                        uSleep(delay);
                    }
                }
                Thread.Sleep(300);
            } while (true);
        }

        private static Stopwatch stopWatch = new Stopwatch();
        private static void uSleep(int microseconds)
        {
            long ticks = ((1000L * 1000L * 1000L) / Stopwatch.Frequency) * microseconds;
            stopWatch.Start();
            while (stopWatch.ElapsedTicks < ticks)
            { }
            stopWatch.Stop();
        }
    }
}
