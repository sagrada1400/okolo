using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using Word = Microsoft.Office.Interop.Word;
using System.IO;
using System.Diagnostics;

namespace okolo
{
    public partial class otchetopen : Form
    {
        public otchetopen()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OpenLastAddedDocxFile(string folderPath)
        {

            string[] docxFiles = Directory.GetFiles(folderPath, "*.docx");

            if (docxFiles.Length > 0)
            {

                Array.Sort(docxFiles, new Comparison<string>((f1, f2) =>
                    DateTime.Compare(File.GetLastWriteTime(f2), File.GetLastWriteTime(f1))));


                Process.Start(docxFiles[0]);
            }
            else
            {
                MessageBox.Show("В выбранной папке нет файлов формата docx.");
            }
        }
        private void button9_Click(object sender, EventArgs e)
        {
            string folderPath = @"C:\Users\User\Desktop\okolo\okolo\okolo\bin\Debug";
            OpenLastAddedDocxFile(folderPath);
            this.Close();
        }
    }
}
