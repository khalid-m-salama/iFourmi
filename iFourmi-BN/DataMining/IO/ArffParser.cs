using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining;



namespace iFourmi.DataMining.IO
{
    public static class ArffHelper
    {
        public static Data.Dataset LoadDatasetFromArff(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);

            string datasetName = null;
            Data.Metadata metadata = null;
            Data.Dataset dataset = null;

            List<Data.Attribute> attributes = new List<Data.Attribute>();
            List<Data.Example> examples = new List<Data.Example>();
            int attributeIndex = 0;
            int exampleIndex=0;

            List<string> exampleLines = new List<string>();

            while (!reader.EndOfStream)
            {
                string line=reader.ReadLine();
                if(!string.IsNullOrEmpty(line))
                {
                    if(line.Contains("%"))
                        continue;
                    if(line.Contains("@relation"))
                    {
                        datasetName = line.Substring(line.IndexOf(' ')).Trim();
                        datasetName = datasetName.Contains("-weka") ? datasetName.Substring(0, datasetName.IndexOf("-weka")) : datasetName;
                    }

                    else if(line.Contains("@attribute"))
                    {

                        Data.Attribute attribute = ParseAttributeLine(line, attributeIndex);

                        if (attribute != null)
                        {
                            attributeIndex++;
                            attributes.Add(attribute);
                        }
                        
                    }

                    else if (line.Contains("@data"))
                    {
                        int targetIndex = attributes.FindIndex(m => string.Compare(m.Name, "Class", true) == 0);
                        Data.Attribute target = attributes[targetIndex];
                        attributes.RemoveAt(target.Index);
                        metadata = new Data.Metadata(datasetName, attributes.ToArray(),target);
                        dataset = new Data.Dataset(metadata);
                        

                    }
                    else
                    {
                        if (exampleLines.Contains(line))
                            continue;
                        
                        Data.Example example = ParseExampleLine(exampleIndex,line,dataset );
                        exampleIndex++;
                        examples.Add(example);
                        exampleLines.Add(line);
 
                    }
                }
           
            }

            dataset.SetExamples(examples.ToArray());

            return dataset;
        }

        public static Data.Dataset LoadHierarchicalDatasetFromTxt(string filePath,bool skipfirstAttribute)
        {
            StreamReader reader = new StreamReader(filePath);
            

            string datasetName = null;
            Data.Metadata metadata = null;
            Data.Dataset dataset = null;
            
            List<Data.Attribute> attributes = new List<Data.Attribute>();
            List<Data.Example> examples = new List<Data.Example>();
            List<Data.Node> nodes = new List<Data.Node>();

            int attributeIndex = 0;
            int exampleIndex = 0;

            string mode = "start";

            


            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                if (!string.IsNullOrEmpty(line))
                {
                    if (line.Contains("%"))
                        continue;
                    if (line.Contains("@relation"))
                    {
                        datasetName = line.Substring(line.IndexOf(' ')).Trim();
                        datasetName = datasetName.Contains("-weka") ? datasetName.Substring(0, datasetName.IndexOf("-weka")) : datasetName;
                    }

                    else if (line.Contains("@attribute"))
                    {

                        Data.Attribute attribute = ParseAttributeLine(line, attributeIndex);

                        if (attribute != null)
                        {
                            attributeIndex++;
                            attributes.Add(attribute);
                        }
                       

                    }

                    else if(line.Contains("@ontology"))
                    {
                        mode = "ontolog";
                        
                    }

                    else if (line.Contains("@data"))
                    {
                        List<string> classValues=new List<string>();

                        int counter = 0;
                        foreach (Data.Node node in nodes)
                        {
                            node.ValueIndex = counter;
                            classValues.Add(node.Name);
                            counter++;
                        }

                        Data.ClassHierarchy classHierarchy = new Data.ClassHierarchy(nodes.ToArray());

                        Data.HierarchicalAttribute target = new Data.HierarchicalAttribute("class", attributes.Count,classValues.ToArray(),classHierarchy);                      
                        metadata = new Data.Metadata(datasetName, attributes.ToArray(), target);
                        dataset = new Data.Dataset(metadata);

                        mode = "data";

                    }
                    else
                    {
                        if (mode == "ontolog")
                        {
                            Data.Node node = ParseOntologyLine(line);
                            if(!nodes.Exists(n=>n.Name==node.Name))
                                nodes.Add(node);

                        }
                        else
                        {

                            Data.Example example = ParseHierarchicalExampleLine(exampleIndex, line, dataset, skipfirstAttribute);
                            exampleIndex++;
                            examples.Add(example);
                            
                        }

                    }
                }

            }

            dataset.SetExamples(examples.ToArray());

            return dataset;
        }

        private static Data.Example ParseHierarchicalExampleLine(int exampleIndex, string line, Data.Dataset dataset, bool skipFirstAttribute)
        {
            string[] parts = line.Split(',');
            List<int> values = new List<int>();

            List<int> label = new List<int>();
            string[] labelParts = parts[parts.Length - 1].Split(';');
            foreach (string target in labelParts)
                if(!string.IsNullOrWhiteSpace(target))
                    label.Add(dataset.Metadata.Target.GetIndex(target));
              


            int skips = 0;
            int step = skipFirstAttribute ? 1 : 0;
            int index = skipFirstAttribute ? 1 : 0;

            for (; index < parts.Length - 1; index++)
            {
                string value = parts[index];
                if (!value.Contains("All"))
                    values.Add(dataset.Metadata.Attributes[index-step- skips].GetIndex(value));
                else
                    skips++;
            }


            Data.Example example = new Data.Example(dataset.Metadata, exampleIndex, values.ToArray(), label);
            return example;
        }


        private static Data.Node ParseOntologyLine(string line)
        {
            string[] parts = line.Split('\\');

            string name = parts[0];
            

            Data.Node node = new Data.Node(parts[0]);
            if(parts.Length>1)
                node.AddChildren(parts[1].Split(',').ToList());
            return node;
        }

        public static void SaveDatasetToArff(Data.Dataset dataset, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("@relation "+dataset.Metadata.DatasetName);
                writer.WriteLine();

                foreach (Data.Attribute attributre in dataset.Metadata.Attributes)
                    writer.WriteLine("@attribute "+attributre.ToString());
                writer.WriteLine("@attribute " + dataset.Metadata.Target.ToString());
                writer.WriteLine();

                writer.WriteLine("@data");
                foreach (Data.Example example in dataset)
                    writer.WriteLine(example.ToString());
                
                writer.Flush();
                writer.Close();
            }
            
        }


        private static Data.Attribute ParseAttributeLine(string line, int attributeIndex)
        {
            
            List<string> values=new List<string>();
            
            string name="";
            string[] parts = line.Split(' ');
            if (parts.Length == 3)
                name = parts[1];
            else
                name = parts[1] + parts[2];


            string[] vals = parts[parts.Length-1].Split(',');
            foreach (string val in vals)
                values.Add(val.Trim('{', '}',' '));



            foreach(string val in values)
                if(val.Contains("All"))
                    return null;

            Data.Attribute attribute = new Data.Attribute(name, attributeIndex, values.ToArray());
            return attribute;
        }


        private static Data.Example ParseExampleLine(int exampleIndex, string line, Data.Dataset dataset)
        {
            string [] parts =line.Split(',');
            List<int> values = new List<int>();
            int label = dataset.Metadata.Target.GetIndex(parts[parts.Length-1]);

            int skips = 0;
            for (int index = 0; index < parts.Length -1 ; index++)
            {
                string value = parts[index];
                if (!value.Contains("All"))
                    values.Add(dataset.Metadata.Attributes[index - skips].GetIndex(value));
                else
                    skips++;
            }


            Data.Example example = new Data.Example(dataset.Metadata,exampleIndex, values.ToArray(), label);
            return example;
            
        }

        
    }
}
