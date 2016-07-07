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
            string[] target=null;

            int attributeIndex = 0;
            int exampleIndex=0;

            List<int> nominalAttributesIndexes = new List<int>();
            int aindex = 0;
            
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

                        if (line.ToUpper().Contains("CLASS"))
                        {
                            target = ParseClassAttributeLine(line);

                        }

                        else
                        {

                            Data.Attribute [] currentAttributes = ParseAttributeLine(line, attributeIndex);                            
                            attributeIndex+=currentAttributes.Length;
                            attributes.AddRange(currentAttributes);

                            if (currentAttributes.Length > 1 || !line.Contains(currentAttributes[0].Name))
                                nominalAttributesIndexes.Add(aindex);
                            aindex++;
                                                    
                        }
                        
                    }

                    else if (line.Contains("@data"))
                    {
                     
                        metadata = new Data.Metadata(datasetName, attributes.ToArray(),target);
                        dataset = new Data.Dataset(metadata);
                        

                    }
                    else
                    {
                        if (exampleLines.Contains(line))
                            continue;
                        
                        Data.Example example = ParseExampleLine(exampleIndex,line,dataset.Metadata,nominalAttributesIndexes);
                        exampleIndex++;
                        examples.Add(example);
                        exampleLines.Add(line);
 
                    }
                }
           
            }

            dataset.SetExamples(examples.ToArray());

            return dataset;
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
                foreach (Data.Example example in dataset)
                    writer.WriteLine(example.ToString());
                
                writer.Flush();
                writer.Close();
            }
            
        }

        private static string[] ParseClassAttributeLine(string line)
        {
            string[] values = line.Split(' ')[2].Trim('{', '}').Split(',');

            for (int i = 0; i < values.Length; i++)
                values[i] = values[i].Trim();

            return values;
            
        }


        private static Data.Attribute[] ParseAttributeLine(string line, int attributeIndex)
        {

            List<Data.Attribute> attributes = new List<Data.Attribute>();

            string[] parts = line.Split(' ');

            string name=parts[1];
            for (int i = 2; i < parts.Length - 1; i++)
                name += parts[i];

            if (string.Compare( parts[parts.Length-1].Trim(),"numeric",true)==0)
            {
                Data.Attribute attribute = new Data.Attribute(name, attributeIndex);
                attributes.Add(attribute);
            }
            else
            {
                string[] values = line.Split(' ');
                values = values[values.Length - 1].Trim('{', '}').Split(',');

                foreach (string value in values)
                {
                    Data.Attribute attribute = new Data.Attribute(name + "::" + value,attributeIndex);
                    attributes.Add(attribute);
                    attributeIndex++;

                }
 
            }

            return attributes.ToArray();
        }


        private static Data.Example ParseExampleLine(int exampleIndex, string line, Data.Metadata metadata, List<int> nominalAttributesIndexes)
        {
            string [] parts =line.Split(',');
            List<double> values = new List<double>();
            int label = 0;
            for (; label < metadata.Target.Length; label++)
                if (metadata.Target[label] == parts[parts.Length-1])
                    break;

            int offset=0;
            
            for (int index = 0; index < parts.Length -1 ; index++)
            {
               string stringValue = parts[index];
               
               if (!nominalAttributesIndexes.Contains(index))
               {
                   double value=double.NaN;
                   if(stringValue!="?")
                       value = double.Parse(stringValue);
                   values.Add(value);
                   offset++;
               }
               else
               {               
                   Data.Attribute attribute = metadata.Attributes[offset];
                   string attributeName = attribute.Name.Substring(0,attribute.Name.IndexOf("::"));

                   while (true)
                   {
                       attribute = metadata.Attributes[offset];

                       if (!attribute.Name.Contains("::"))
                           break;

                       string currentAttributeName = attribute.Name.Substring(0, attribute.Name.IndexOf("::"));

                       if (currentAttributeName != attributeName)
                           break;

                       string valueName = attribute.Name.Substring(attribute.Name.IndexOf("::") + 2);
                       if (valueName == stringValue)
                           values.Add(1);
                       else
                           values.Add(0);
                       offset++;

                       if (offset == metadata.Attributes.Length)
                           break;

                   }
                   
               }


                    
            }


            Data.Example example = new Data.Example(metadata,exampleIndex, values.ToArray(), label);
            return example;
            
        }

        
    }
}
