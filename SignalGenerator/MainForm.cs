using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace SignalGenerator
{
    public partial class MainForm : Form
    {
        private DataContainer _dataContainer;

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
    }
}
