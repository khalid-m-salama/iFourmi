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
            List<Data.Instance> instances = new List<Data.Instance>();
            int attributeIndex = 0;
            int instanceIndex = 0;

            List<string> instanceLines = new List<string>();

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

                    else if (line.Contains("@data"))
                    {
                        int targetIndex = attributes.FindIndex(m => string.Compare(m.Name, "Class", true) == 0);
                        Data.NominalAttribute target = attributes[targetIndex] as Data.NominalAttribute;
                        attributes.RemoveAt(target.Index);
                        metadata = new Data.Metadata(datasetName, attributes.ToArray(), target,false);
                        dataset = new Data.Dataset(metadata);


                    }
                    else
                    {
                        if (instanceLines.Contains(line))
                            continue;

                        Data.Instance instance = ParseinstanceLine(instanceIndex, line, dataset);
                        instanceIndex++;
                        instances.Add(instance);
                        instanceLines.Add(line);

                    }
                }

            }

            dataset.SetInstances(instances.ToArray());

            return dataset;
        }
        
        private static Data.Attribute ParseAttributeLine(string line, int attributeIndex)
        {
             Data.Attribute attribute=null;

            string[] parts = line.Split(' ');

            string name=parts[1];
            for (int i = 2; i < parts.Length - 1; i++)
                name += parts[i];

            if (string.Compare(parts[parts.Length - 1].Trim(), "numeric", true) == 0)
            {

                attribute = new Data.NumericAttribute(name, attributeIndex);
            }
            else
            {
                string[] values = line.Split(' ');
                values = values[values.Length - 1].Trim('{', '}').Split(',');

                attribute = new Data.NominalAttribute(name, attributeIndex, values);

            }

            return attribute;

        
        }
        
        private static Data.Instance ParseinstanceLine(int instanceIndex, string line, Data.Dataset dataset)
        {
            string[] parts = line.Split(',');
            List<double> values = new List<double>();
            int label = dataset.Metadata.Target.GetIndex(parts[parts.Length - 1]);

            for (int index = 0; index < parts.Length - 1; index++)
            {
                if (dataset.Metadata.Attributes[index] is Data.NominalAttribute)
                {
                    Data.NominalAttribute attribute = dataset.Metadata.Attributes[index] as Data.NominalAttribute;
                    string value = parts[index];
                    values.Add(attribute.GetIndex(value));
                }
                else
                {
                    double value = double.NaN;
                    if (parts[index] != "?")
                        value = double.Parse(parts[index]);
                    values.Add(value);
                    
                }
               
            }


            Data.Instance instance = new Data.Instance(dataset.Metadata, instanceIndex, values.ToArray(), label);
            return instance;

        }

        public static Data.Dataset LoadHierarchicalDatasetFromTxt(string filePath, bool skipfirstAttribute)
        {
            StreamReader reader = new StreamReader(filePath);
            
            string datasetName = null;
            Data.Metadata metadata = null;
            Data.Dataset dataset = null;

            List<Data.Attribute> attributes = new List<Data.Attribute>();
            List<Data.Instance> instances = new List<Data.Instance>();
            List<Data.Node> nodes = new List<Data.Node>();

            int attributeIndex = 0;
            int instanceIndex = 0;

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

                    else if (line.Contains("@ontology"))
                    {
                        mode = "ontolog";

                    }

                    else if (line.Contains("@data"))
                    {
                        List<string> classValues = new List<string>();

                        int counter = 0;
                        for(int i=1; i<nodes.Count;i++)                        
                        {
                            Data.Node node = nodes[i];
                            node.ValueIndex = counter;
                            classValues.Add(node.Name);
                            counter++;
                        }

                        Data.ClassHierarchy classHierarchy = new Data.ClassHierarchy(nodes.ToArray());

                        Data.HierarchicalAttribute target = new Data.HierarchicalAttribute("class", attributes.Count, classValues.ToArray(), classHierarchy);
                        metadata = new Data.Metadata(datasetName, attributes.ToArray(), target,true);
                        dataset = new Data.Dataset(metadata);

                        mode = "data";

                    }
                    else
                    {
                        if (mode == "ontolog")
                        {
                            Data.Node node = ParseOntologyLine(line);
                            if (!nodes.Exists(n => n.Name == node.Name))
                                nodes.Add(node);

                        }
                        else
                        {

                            Data.Instance instance = ParseHierarchicalinstanceLine(instanceIndex, line, dataset, skipfirstAttribute);
                            instanceIndex++;
                            instances.Add(instance);

                        }

                    }
                }

            }

            dataset.SetInstances(instances.ToArray());

            return dataset;
        }

        private static Data.Instance ParseHierarchicalinstanceLine(int instanceIndex, string line, Data.Dataset dataset, bool skipFirstAttribute)
        {
            string[] parts = line.Split(',');
            List<double> values = new List<double>();

            List<int> label = new List<int>();
            string[] labelParts = parts[parts.Length - 1].Split(';');
            foreach (string target in labelParts)
                if (!string.IsNullOrWhiteSpace(target))
                    label.Add(dataset.Metadata.Target.GetIndex(target));



            for (int index = 0; index < parts.Length - 1; index++)
            {
                if (dataset.Metadata.Attributes[index] is Data.NominalAttribute)
                {
                    Data.NominalAttribute attribute = dataset.Metadata.Attributes[index] as Data.NominalAttribute;
                    string value = parts[index];
                    values.Add(attribute.GetIndex(value));
                }
                else
                {
                    double value = double.NaN;
                    if (parts[index] != "?")
                        value = double.Parse(parts[index]);
                    values.Add(value);

                }

            }


            Data.Instance instance = new Data.Instance(dataset.Metadata, instanceIndex, values.ToArray(), label);
            return instance;
        }

        private static Data.Node ParseOntologyLine(string line)
        {
            string[] parts = line.Split('\\');

            string name = parts[0];


            Data.Node node = new Data.Node(parts[0]);
            if (parts.Length > 1)
                node.AddChildren(parts[1].Split(',').ToList());
            return node;
        }


        public static void SaveDatasetToArff(Data.Dataset dataset, string filePath)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                writer.WriteLine("@relation "+dataset.Metadata.DatasetName);
                writer.WriteLine();

                foreach (Data.Attribute attribute in dataset.Metadata.Attributes)
                    writer.WriteLine("@attribute "+attribute.ToString());
                writer.WriteLine("@attribute " + dataset.Metadata.Target.ToString());
                writer.WriteLine();

                writer.WriteLine("@data");
                foreach (Data.Instance instance in dataset)
                    writer.WriteLine(instance.ToString());
                
                writer.Flush();
                writer.Close();
            }
            
        }


    

        
    }
}
