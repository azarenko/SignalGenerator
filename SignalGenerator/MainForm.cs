﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SignalGenerator
{
    public partial class MainForm : Form
    {
        private DataContainer _dataContainer = null;
        private MediaPlayer player = null;

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
            if (_dataContainer != null)
            {
                byte[] buffer = _dataContainer.GetSoundBuffer((int)_rpm.Value);
                File.WriteAllBytes("out.wav", buffer);
                player = new MediaPlayer(buffer);
                player.Play(buffer);
            }
        }

        private void _stop_Click(object sender, EventArgs e)
        {
            if (_dataContainer != null)
            {
                player.Stop();
            }
        }

        private void _rpm_ValueChanged(object sender, EventArgs e)
        {
            if (_dataContainer != null)
            {
                byte[] buffer = _dataContainer.GetSoundBuffer((int)_rpm.Value);
                player.Play(buffer);
            }
        }
    }
}
