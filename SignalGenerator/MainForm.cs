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
        private static bool isRun = false;
        private static int PortAddress = 0;

        public MainForm()
        {
            InitializeComponent();
            _deviceName.SelectedIndex = 0;
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
            if (_dataContainer != null)
            {
                PortAddress = ParallelPort.PortAddress[_deviceName.SelectedIndex];
                isRun = true;
            }
        }

        private void _stop_Click(object sender, EventArgs e)
        {
            if (_dataContainer != null)
            {
                isRun = false;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private int GetDelay()
        {
            return (int)((1000L * 1000L * 60L) / (((long)_rpm.Value / 2) * _dataContainer.dataSet.Count));
        }

        private static Stopwatch stopWatch = new Stopwatch();
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            uint w,r;
            w = r = 0;
            
            do
            {
                if (_dataContainer != null)
                {
                    while (isRun)
                    {
                        int microseconds = GetDelay();
                        long ticks = ((10L * 1000L * 1000L) / Stopwatch.Frequency) * microseconds;

                        for (int i = 0; i < _dataContainer.dataSet.Count; i++)
                        {
                            stopWatch.Start();

                            ParallelPort.Output(PortAddress, _dataContainer.dataSet[i]);

                            while (stopWatch.ElapsedTicks < ticks)
                            { }
                            stopWatch.Stop();
                            stopWatch.Reset();
                        }
                    }
                }
                Thread.Sleep(300);
            } while (true);
        }
    }
}
