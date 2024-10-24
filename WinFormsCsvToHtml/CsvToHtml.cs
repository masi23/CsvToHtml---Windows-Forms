using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace WinFormsCsvToHtml
{

    public partial class CsvToHtml : Form
    {

        string srcPath = "";                    //source file (.CSV) directory
        string destPath = "";                   //destination file (.HTML) directory
        string lastOpenedDirectory = @"C:\";    //holds recently opened folder directory
        public CsvToHtml()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Choose source file with OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog();

            openFileDialog.Title = "Wybierz plik";
            openFileDialog.Filter = "Pliki CSV (*.csv)|*.csv";
            openFileDialog.InitialDirectory = lastOpenedDirectory;

            if(openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Read selected CSV file and display its content with textBox
                srcPath = openFileDialog.FileName;
                lastOpenedDirectory = Path.GetDirectoryName(srcPath);
                label3.Text = "Plik źródłowy: " + Path.GetFileName(srcPath);
                textBox1.Text = srcPath;
                string csvText = File.ReadAllText(srcPath);
                textBox3.Text = csvText;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //choose destination file with SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog();

            saveFileDialog.Title = "Wybierz plik";
            saveFileDialog.Filter = "Pliki HTML (*.html)|*.html";
            saveFileDialog.InitialDirectory = lastOpenedDirectory;

            //show info dialog
            if(saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                destPath = saveFileDialog.FileName;
                textBox2.Text = destPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if(srcPath != "" && destPath != "")
            {
                //use HTML format to create table and fill it with data
                bool firstLine = true;
                File.WriteAllText(destPath, "<html><head><style>" +
                    "table, tr, td{ border: 1px solid lightgray; border-collapse: collapse;} " +
                    "tr:nth-child(even) {background: lightgray;}</style></head><body><table>");

                
                string[] lines = File.ReadAllLines(srcPath);
                foreach(var line in lines)
                {
                    File.AppendAllText(destPath, "<tr>");
                    string[] values = line.Split(",");
                    foreach (var value in values)
                    {
                        //display first line of data in special (header) table row
                        if (firstLine)
                        {
                            File.AppendAllText(destPath, "<th>" + value + "</th>");
                        }
                        else
                        {
                            File.AppendAllText(destPath, "<td>" + value + "</td>");
                        }
                    }
                    if (firstLine)
                    {
                        firstLine = false;
                    }
                    File.AppendAllText(destPath, "</tr>");
                }
                //finish file in proper HTML style
                File.AppendAllText(destPath, "</table></body></html>");

                //open newly created/altered HTML file in default web browser
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = destPath,
                        UseShellExecute = true
                    });
                } catch(Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd: " + ex.Message);
                }
            } else
            {
                MessageBox.Show("Nie wybrano ścieżki dla pliku.", "Uwaga", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void textBox3Changed(object sender, EventArgs e)
        {
            saveChangesButton.Visible = true;
        }

        //save changes made in CSV-displaying textBox with SaveFileDialog
        private void saveChangesButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Wybierz plik";
            sfd.Filter = "Pliki CSV (*.csv)|*.csv";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(srcPath, textBox3.Text);
                MessageBox.Show("Plik został zapisany pomyślnie", "Sukces", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
