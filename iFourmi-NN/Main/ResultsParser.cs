using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.Main
{
    public  class ResultsParser
    {
        string _root="";
        string output;
        string output2;
       
        public ResultsParser(string root)
        {
            this._root = root;
            this.output = root + "\\0 - Result_quality.txt";
            this.output2 = root + "\\0 - Result_size.txt";
            
        }
        

        public void ParseResults()
        {
            string dataset;


            List<string> toDump_quality = new List<string>();
            List<string> toDump_size = new List<string>();

               

            
            foreach(string file in System.IO.Directory.GetFiles(this._root))
            {
                

                dataset = System.IO.Path.GetFileNameWithoutExtension(file);
                if (dataset.Contains("0 - "))
                    continue;
                dataset=dataset.Substring(0, dataset.IndexOf("_res"));

                string[] lines = System.IO.File.ReadLines(file).ToArray();

                string[] QualityArray = new string[6];
                string[] SizeArray = new string[6];

                QualityArray[0] = dataset;
                SizeArray[0] = dataset;

            
              
                for (int i = 0; i < lines.Length; i++)               
                {
                    string[] parts = lines[i].Split(',');
                    string algorithm = parts[0];
                    string quality = parts[1];
                    string size = parts[2];

                    QualityArray[i + 1] = quality;
                    SizeArray[i + 1] = size;


                }

                string resultLine_quality = string.Join(",", QualityArray);
                string resultLine_size = string.Join(",", SizeArray);

                toDump_quality.Add(resultLine_quality);
                toDump_size.Add(resultLine_size);
                
            }


            System.IO.File.WriteAllLines(output, toDump_quality.ToArray());

            System.IO.File.WriteAllLines(output2, toDump_size.ToArray());

            
        }


    }
}
