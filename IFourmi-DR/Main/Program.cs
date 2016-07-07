using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model.IBLearning;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Model.Ensembles;


namespace iFourmi.Main
{
    class Program
    {
        static void Main(string[] args)
        {
            iFourmi.Utilities.RandomUtility.Initialize(1);
            BatchTest.DatasetFolderPath = GetFilePath();
            BatchTest.DatasetNamesFile = "datasets.txt";


            BatchTest.Run_ACODR_WekaClassifier(true, false);

            BatchTest.Run_ACODR_WekaClassifier(false, true);

            BatchTest.Run_ACODR_WekaClassifier(true, true);

            BatchTest.Run_ACODR2_WekaClassifier(true);

            BatchTest.Run_ACODR2_WekaClassifier(false);

            
          

            //BatchTest.Run_Greedy_WekaClassifier(false, true);

            //BatchTest.Run_Random_WekaClassifier(false, true);

            //BatchTest.Run_ACODR2_WekaClassifier(true);

     

            //ParseResults();

            //Console.WriteLine("Batch Done");
            //Console.ReadLine();

        }

 
        private static string GetFilePath()
        {
            return System.IO.File.ReadAllLines("Config.txt")[0];
        }

        private static void ParseResults()
        {
            string root = @"D:\Academics\0 - Academic Papers\2014.11 - ADR-Miner\Results\Results - Rand";

            StringBuilder strBuilder = new StringBuilder();

            foreach (string file in System.IO.Directory.GetFiles(root))
            {
                

                string dataset = System.IO.Path.GetFileNameWithoutExtension(file);
                dataset = dataset.Substring(0, dataset.IndexOf("_res"));

                

                string[] lines = System.IO.File.ReadAllLines(file);

                string[] parts = new string[lines.Length+1];

                parts[0] = dataset;

                for (int i = 1; i <= lines.Length; i++)
                    parts[i] = lines[i - 1].Split(',')[3];

               

                string newline = string.Join(",", parts);

                strBuilder.Append(newline);
                strBuilder.Append(Environment.NewLine);

            }


            string result = strBuilder.ToString();

        }

        





    }
}
