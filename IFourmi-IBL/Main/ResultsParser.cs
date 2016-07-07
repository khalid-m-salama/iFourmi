using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace iFourmi.Main
{
    public static  class  ResultsParser
    {
        static private void ParseConvResults()
        {
            string root = @"D:\Academics\0 - Academic Papers\2014.04 - ACO-IBL\Con Results";

            StringBuilder strBuilder = new StringBuilder();

            foreach (string file in System.IO.Directory.GetFiles(root))
            {
                string[] parts = new string[4];

                string dataset = System.IO.Path.GetFileNameWithoutExtension(file);
                dataset = dataset.Substring(0, dataset.IndexOf("_res"));

                parts[0] = dataset;

                string[] lines = System.IO.File.ReadAllLines(file);
                parts[1] = lines[0].Split(',')[2];
                parts[2] = lines[1].Split(',')[2];
                parts[3] = lines[2].Split(',')[1];

                string newline = string.Join(",", parts);

                strBuilder.Append(newline);
                strBuilder.Append(Environment.NewLine);

            }


            string result = strBuilder.ToString();

        }

        public static DataTable PrepareDataTable()
        {
            DataTable tbData = new DataTable();
            tbData.Columns.Add(new DataColumn("Dataset"));

            //tbData.Columns.Add(new DataColumn("ACO-KNN-CB"));
            //tbData.Columns.Add(new DataColumn("ACO-KNN-CB-ens-WV"));
            //tbData.Columns.Add(new DataColumn("ACO-NNC-CB"));
            //tbData.Columns.Add(new DataColumn("ACO-NNC-CB-ens-WV"));
            //tbData.Columns.Add(new DataColumn("ACO-GKC-CB"));
            //tbData.Columns.Add(new DataColumn("ACO-GKC-CB-ens-WV"));

            //tbData.Columns.Add(new DataColumn("PSO-KNN-CB"));
            //tbData.Columns.Add(new DataColumn("PSO-KNN-CB-ens-WV"));
            //tbData.Columns.Add(new DataColumn("PSO-NNC-CB"));
            //tbData.Columns.Add(new DataColumn("PSO-NNC-CB-ens-WV"));
            //tbData.Columns.Add(new DataColumn("PSO-GKC-CB"));
            //tbData.Columns.Add(new DataColumn("PSO-GKC-CB-ens-WV"));

            return tbData;
        }

        public static DataTable PrepareDataTable2()
        {
            DataTable tbData = new DataTable();
            tbData.Columns.Add(new DataColumn("Dataset"));
            tbData.Columns.Add(new DataColumn("1NN"));
            tbData.Columns.Add(new DataColumn("11NN"));
            tbData.Columns.Add(new DataColumn("21NN"));

            tbData.Columns.Add(new DataColumn("NCC-0"));
            tbData.Columns.Add(new DataColumn("NCC-0.5"));
            tbData.Columns.Add(new DataColumn("NCC-1"));

            tbData.Columns.Add(new DataColumn("GKE-0"));
            tbData.Columns.Add(new DataColumn("GKE-0.25"));
            tbData.Columns.Add(new DataColumn("GKE-0.5"));


            return tbData;
        }

        public static DataTable PopulateDataTable()
        {
            DataTable tbData = PrepareDataTable2();

            string root = @"D:\Academics\0 - Academic Papers\2014.06 - ACO-IBL\Results\Con Results\";
            //string root = @"D:\Academics\0 - Academic Papers\2014.04 - ACO-IBL\Results\ACO-Results";
            //string root = @"D:\Academics\0 - Academic Papers\2014.04 - ACO-IBL\Results\PSO-Results";

            foreach (string file in System.IO.Directory.GetFiles(root))
            {
                DataRow dr = tbData.NewRow();

                string dataset = System.IO.Path.GetFileNameWithoutExtension(file);
                dataset = dataset.Substring(0, dataset.IndexOf("_res"));
                dr[0] = dataset;


                string[] lines = System.IO.File.ReadAllLines(file);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(',');
                    string alg = parts[0];
                    string res = parts[parts.Length - 2];
                    dr[alg] = res;
                }


                tbData.Rows.Add(dr);

            }

            return tbData;
        }

        
    }
}
