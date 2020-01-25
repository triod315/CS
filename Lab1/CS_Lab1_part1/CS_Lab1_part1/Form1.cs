using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Noemax.Compression;

namespace CS_Lab1_part1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OpenFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string fileName = openFileDialog1.FileName;
                toolStripStatusLabel1.Text = fileName;

                richTextBox1.Text=System.IO.File.ReadAllText(fileName);
                
            }
        }

        private void RunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SortedDictionary<char, int> analText = new SortedDictionary<char, int>();

            var text = richTextBox1.Text;
            int letterCount = 0;
            int unicletterCount = 0;

            for (int i = 0; i < text.Length; i++)
            {
                if (!analText.ContainsKey(Char.ToLower(text[i])) && Char.IsLetter(text[i]))
                {
                    analText.Add(Char.ToLower(text[i]), 1);
                    letterCount++;
                }
                else if (analText.ContainsKey(Char.ToLower(text[i])))
                {
                    analText[Char.ToLower(text[i])]++;
                    letterCount++;
                }
            }

            unicletterCount = analText.Count();
            Dictionary<char, double> freqText = new Dictionary<char, double>();
            
            //calculate frequency
            chart1.Series["Frequency"].Points.Clear();
            chart1.ChartAreas[0].AxisX.Interval = 1;
            
            foreach (var c in analText)
            {
                freqText.Add(c.Key, (double)c.Value / letterCount);
                chart1.Series["Frequency"].Points.AddXY(c.Key.ToString(), (float)c.Value / letterCount);

                dataGridView1.Rows.Add(c.Key, c.Value,Math.Round(freqText[c.Key],4));
            }

            label1.Text = $"Letter count: {letterCount}\nUnic letter count: {unicletterCount}\n";
            label1.Text += $"Entropy: {calcAvgEntropy(freqText):0.000}\nInformation count: {calcAvgEntropy(freqText)*letterCount/8:0.000} bytes";
        }

        double calcAvgEntropy(Dictionary<char, double> freqText)
        {
            double result = 0;
            foreach (var elem in freqText)
            {
                if (1/elem.Value!=0)
                    result += elem.Value * Math.Log(1 / elem.Value);
            }

            return result;
        }

        private void ToZlibToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writeTextToFile(writeZLib);
        }

        delegate void CompressMethod(string path, string text);

        void writeTextToFile(CompressMethod compressMethod)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                compressMethod(saveFileDialog.FileName, richTextBox1.Text);
            }
        }
        /// <summary>
        /// Compress file using algorithm ZLib
        /// </summary>
        /// <param name="path">Path to file</param>
        /// <param name="text">Text to write</param>
        void writeZLib(string path, string text)
        {
            CompressionFactory.Zlib.Compress(Encoding.UTF8.GetBytes(richTextBox1.Text), 3);

            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);

            using (var output = CompressionFactory.Zlib.CreateOutputStream(File.Create(path+".zlib"), 3, false))
            {
                output.Write(data, 0, data.Length);
            }

            FileInfo fileInfo = new FileInfo(path+".zlib");

            label1.Text += $"\nZlib file: {fileInfo.Name}\nsize={fileInfo.Length}";
        }

        /// <summary>
        /// Compress file using gZIP
        /// </summary>
        /// <param name="path">Path to compressed file</param>
        /// <param name="text">text to compress and write</param>
        void writeGZIP(string path, string text)
        {
            path += ".gzip";
            CompressionFactory.GZip.Compress(Encoding.UTF8.GetBytes(richTextBox1.Text), 3);

            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);

            using (var output = CompressionFactory.GZip.CreateOutputStream(File.Create(path), 3, false))
            {
                output.Write(data, 0, data.Length);

            }
            FileInfo fileInfo = new FileInfo(path);

            label1.Text += $"\nGlib file: {fileInfo.Name}\nsize={fileInfo.Length}";

        }

        void writeBZIP2(string path, string text)
        {
            
            CompressionFactory.BZip2.Compress(Encoding.UTF8.GetBytes(richTextBox1.Text), 3);

            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);

            using (var output = CompressionFactory.BZip2.CreateOutputStream(File.Create(path + ".bzip2"), 3, false))
            {
                output.Write(data, 0, data.Length);
                path += ".bzip2";
            }
            FileInfo fileInfo = new FileInfo(path); 

            label1.Text += $"\nBZIP2 file: {fileInfo.Name}\nsize={fileInfo.Length}";
            
        }

        void writeLZF4(string path, string text)
        {

            CompressionFactory.Lzf4.Compress(Encoding.UTF8.GetBytes(richTextBox1.Text), 3);

            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);

            using (var output = CompressionFactory.Lzf4.CreateOutputStream(File.Create(path + ".lzf4"), 3, false))
            {
                output.Write(data, 0, data.Length);
                path += ".lzf4";
            }
            FileInfo fileInfo = new FileInfo(path);

            label1.Text += $"\nlzf4 file: {fileInfo.Name}\nsize={fileInfo.Length}";

        }

        void writeLZMA(string path, string text)
        {

            CompressionFactory.Lzma.Compress(Encoding.UTF8.GetBytes(richTextBox1.Text), 3);

            byte[] data = Encoding.UTF8.GetBytes(richTextBox1.Text);

            using (var output = CompressionFactory.Lzma.CreateOutputStream(File.Create(path + ".lzma"), 3, false))
            {
                output.Write(data, 0, data.Length);
                path += ".lzma";
            }
            FileInfo fileInfo = new FileInfo(path);

            label1.Text += $"\nlzma file: {fileInfo.Name}\nsize={fileInfo.Length}";

        }

        void writeText(string path, string text)
        {
            path += ".txt";
            File.WriteAllText(path, text);
            toolStripStatusLabel1.Text = path;
            FileInfo fileInfo = new FileInfo(path);

            label1.Text += $"\nTXT file: {fileInfo.Name}\nsize={fileInfo.Length}";
        }
        private void ToBzib2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writeTextToFile(writeBZIP2);
        }
        
        private void ToGzipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writeTextToFile(writeGZIP);
        }

        private void ToZipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            writeTextToFile(writeLZF4);
        }

        private void ToAllAboveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                writeZLib(saveFileDialog.FileName, richTextBox1.Text);
                writeBZIP2(saveFileDialog.FileName, richTextBox1.Text);
                writeGZIP(saveFileDialog.FileName, richTextBox1.Text);
                writeLZMA(saveFileDialog.FileName, richTextBox1.Text);
                writeLZF4(saveFileDialog.FileName, richTextBox1.Text);
                writeText(saveFileDialog.FileName, richTextBox1.Text);
            }

            CompressionReport compressionReport = new CompressionReport(saveFileDialog.FileName);
            compressionReport.Show();
           
        }

        private void SaveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                writeText(saveFileDialog.FileName, richTextBox1.Text);
            }

        }

        private void ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                chart1.SaveImage(saveFileDialog.FileName+".png",System.Windows.Forms.DataVisualization.Charting.ChartImageFormat.Png);
            }
        }
    }
}
