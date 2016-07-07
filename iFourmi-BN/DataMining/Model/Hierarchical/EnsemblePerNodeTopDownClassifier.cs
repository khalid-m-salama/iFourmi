using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.Model.Hierarchical
{
    public class EnsemblePerNodeTopDownClassifier:EnsembleTopDownClassifier
    {
        public EnsemblePerNodeTopDownClassifier(ClassHierarchy hierarchy)
            :base(hierarchy) {}

        
        public override int [] Classify(Data.Example example)
        {
            
            List<int> prediceted = new List<int>();

            Queue<Node> candidates = new Queue<Node>();
            foreach(string child in this._hierarchy.Root.Children)
                candidates.Enqueue(this._hierarchy[child]);
            
            while (candidates.Count != 0)
            {
                Node current = candidates.Dequeue();
                List<Tuple<string,IClassifier>> classifiers=this._modelMapping[current.Name];                               
                
                int [] values=new int[classifiers.Count];
                int index = 0;
                foreach (Tuple<string, IClassifier> tuple in classifiers)
                {
                    IClassifier classifier = tuple.Item2;
                    int localLabel = classifier.Classify(example.Clone() as Example);
                    values[index] = localLabel;
                                      
                }

                IClassifier metaClassifier = this._metaModel[current.Name];
                Example metaExample = new Example(new Dataset(metaClassifier.MetaData), 0, values, -1);

                int label = metaClassifier.Classify(metaExample);
                
                
                    if (label == 0)
                    {
                        prediceted.Add(current.ValueIndex);
                        if (current.Children != null)
                            foreach (string child in current.Children)
                                candidates.Enqueue(this._hierarchy[child]);                        
                    }
                

            }

            return prediceted.Distinct().ToArray();
        }

        public int[] Classify(List<Data.Example> examples)
        {

            List<int> prediceted = new List<int>();

            Queue<Node> candidates = new Queue<Node>();
            foreach (string child in this._hierarchy.Root.Children)
                candidates.Enqueue(this._hierarchy[child]);


            while (candidates.Count != 0)
            {
                Node current = candidates.Dequeue();
                List<Tuple<string, IClassifier>> classifiers = this._modelMapping[current.Name];

                int[] values = new int[classifiers.Count];
                int index = 0;
                foreach (Tuple<string, IClassifier> tuple in classifiers)
                {
                    string classifierName = tuple.Item1;
                    IClassifier classifier = tuple.Item2;
                    Example example = examples.Find(e => e.Dataset.Metadata.DatasetName == classifierName);
                    int localLabel = classifier.Classify(example.Clone() as Example);
                    values[index] = localLabel;

                }

                IClassifier metaClassifier = this._metaModel[current.Name];
                Example metaExample = new Example(new Dataset(metaClassifier.MetaData), 0, values, -1);

                int label = metaClassifier.Classify(metaExample);


                if (label == 0)
                {
                    prediceted.Add(current.ValueIndex);
                    if (current.Children != null)
                        foreach (string child in current.Children)
                            candidates.Enqueue(this._hierarchy[child]);
                }


            }

            return prediceted.Distinct().ToArray();
        }

        public static EnsemblePerNodeTopDownClassifier CreateEnsembleDSPerNodeTopDownClassifier(List<Data.Dataset> trainingSets, IClassificationAlgorithm algorithm)
        {
            ClassHierarchy hierarchy = ((Data.HierarchicalAttribute)trainingSets[0].Metadata.Target).Hierarchy;
            EnsemblePerNodeTopDownClassifier topDownClassifier = new EnsemblePerNodeTopDownClassifier(hierarchy);


            Data.Attribute[] metaAttributes = new Data.Attribute[2];

            string[] values = new string[trainingSets.Count];

            for(int i =0; i< metaAttributes.Length; i++)            
                values[i]=trainingSets[i].Metadata.DatasetName;

            metaAttributes[0] = new Data.Attribute("Dataset", 0, values);
            metaAttributes[1] = new Data.Attribute("Prediction", 1,  new string[] { "yes", "no" });
            Data.Attribute target=new Data.Attribute("class",metaAttributes.Length,new string[] { "yes", "no" });

            Metadata meta = new Metadata("metaModel", metaAttributes, target);

            Node current = hierarchy.Root;
            Queue<Node> queue = new Queue<Node>();
            foreach (Node node in hierarchy.GetNodes(current.Children))
                queue.Enqueue(node);

            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                Node[] siblings = hierarchy.GetSiblings(current);
                List<string> negativeClassValues = new List<string>();
                foreach (Node sibling in siblings)
                    negativeClassValues.Add(sibling.Name);


                Dataset dsLocal=new Dataset(meta.Clone());                

                int i=0;
                foreach (Dataset trainingSet in trainingSets)
                {                   

                    Dataset local = trainingSet.GetBinaryFlatLabelSubDataset(current.Name, negativeClassValues.ToArray());
                    algorithm.TrainingSet=local;
                    IClassifier classifier = algorithm.CreateClassifier();
                    topDownClassifier.AddClassifier(current.Name, local.Metadata.DatasetName, classifier);
                    //Example metaExample=new Example(dsLocal,i,


                }

                //algorithm.TrainingSet = local;
                //IClassifier classifier = algorithm.CreateClassifier();
                //topDownClassifier.PutClassifier(current.Name, trainingSet.Metadata.DatasetName, classifier);
                //if (current.Children != null)
                //    foreach (string child in current.Children)
                //        queue.Enqueue(hierarchy[child]);


            }


            return topDownClassifier;
        }
        
    }
}
