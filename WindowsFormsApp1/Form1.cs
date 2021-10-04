using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            string OutputFilePath = "E:\\";
            string inputFilePath = "E:\\03-10-2021.pu";
            ParseData(‪inputFilePath, OutputFilePath);
        }

        public static bool ValidateInputFile(string inputFilePath)
        {
            //get file info
            FileInfo fileName = new FileInfo(inputFilePath);

            //check if file exist
            if (!fileName.Exists)
                return false;
            //check if file name is valid
            if (fileName.Name != string.Format("{0}.pu", DateTime.Now.ToString("dd-MM-yyyy")))
                return false;
            return true;
        }

        public static void ParseData(string inputFilePath, string OutputFilePath)
        {
            //validate input file
            if (!ValidateInputFile(inputFilePath))
                throw new Exception("Invalid file");
            // ReadAllLines
            var fileLines = File.ReadAllLines(inputFilePath);

            // Validate The First Line
            string date_file_name = inputFilePath.Substring(inputFilePath.Length - 13, 10);  //dd-MM-yyyy
            var first_line = fileLines[0].Split(',').ToList();
            string date_first_line = first_line[0].ToString();
            if (date_first_line != "date:" + date_file_name)
            {
                throw new Exception(" Invalid File");
            }

            //check that the number of burglaries on the first line is correct;
            if (first_line[1].ToString() != "count:" + (fileLines.Count() - 1).ToString())
            {
                throw new Exception(" Invalid File ");
            }

            // validate the rest of lines

            //In this line, specify the path to save the entire file with the date;
            string Path = OutputFilePath + date_file_name + ".ur";
            using (StreamWriter sw = new StreamWriter(Path))
            {
                //Write the first line in the processing file;
                sw.WriteLine(first_line[0].ToString() + "," + first_line[1].ToString() + "\n" + "{");

                for (int i = 1; i <= fileLines.Length; i++)
                {
                    string[] All = fileLines[i].ToString().Split(new string[] { "[", "]" }, StringSplitOptions.None);
                    string[] firstHalf = All[0].Split(new char[] { ',' }); //put{Number , string}in array;
                    if (firstHalf.Count() != 4)  //Check the number of columns in all lines;
                    {
                        throw new Exception("The number of columns per line is incorrect !");
                    }

                    Regex reg2 = new Regex("^(\\d+)$");   //Validate that the first element in the line is a number
                    if (!reg2.IsMatch(firstHalf[0]))
                    {
                        throw new Exception("The first element is not a number!");
                    }

                    sw.WriteLine("    [");
                    //Writing the first half {Number , string};
                    sw.WriteLine("        age:" + firstHalf[0].ToString() + "," + "\n" + "        country:" + firstHalf[1].ToString() + "," + "\n" + "        Name:" + firstHalf[2].ToString());
                    if (All[1].Contains(","))   //Find out if there is between [] more than one element {array of string};
                    {   //Write more than one element;
                        string[] last_half = All[1].Split(new char[] { ',' }); //put{array of string}in array;
                        sw.WriteLine("        Date:[");
                        for (int x = 0; x < last_half.Length; x++)
                        {
                            sw.WriteLine("             " + last_half[x] + ",");
                        }
                        sw.WriteLine("        ]");
                        sw.WriteLine("    ]");
                    }
                    else
                    {   //If it is one element;
                        sw.WriteLine("        Date:[");
                        sw.WriteLine("            " + All[1]);
                        sw.WriteLine("        ]");
                        sw.WriteLine("    ],");
                    }
                }
            }
        }
    }
}