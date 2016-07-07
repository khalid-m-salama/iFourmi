using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.DataMining.EnsembleStrategy
{
    public class EnsembleStackClassificationStrategy:IEnsembleClassificationStrategy
    {
        public int Classify(List<ClassifierInfo> classifiersInfo, Data.Example example)
        {
            IClassifier metaClassifier = classifiersInfo.Last().Classifier;
            List<int> predictionValues = new List<int>();

            for (int i = 0; i < classifiersInfo.Count - 1; i++)
            {
                IClassifier classifier = classifiersInfo[i].Classifier;
                int label = classifier.Classify(example).Label;
                predictionValues.Add(label);
            }

            Data.Example metaExample = new Data.Example(metaClassifier.Metadata, 0, predictionValues.ToArray(), -1);

            int finalPredction = metaClassifier.Classify(metaExample).Label;

            return finalPredction;


        }
        
        public int Classify(List<ClassifierInfo> classifiersInfo, List<Data.Example> examples)
        {
            IClassifier metaClassifier = classifiersInfo.Last().Classifier;
            List<int> predictionValues = new List<int>();

            for (int i = 0; i < classifiersInfo.Count - 1; i++)
            {
                ClassifierInfo info = classifiersInfo[i];
                Data.Example example= examples.Find(e => info.Desc.Contains(e.Metadata.DatasetName));

                int prediction = info.Classifier.Classify(example).Label;
                predictionValues.Add(prediction);
            }

            Data.Example metaExample = new Data.Example(null, -1, predictionValues.ToArray(), -1);

            int finalPredction = metaClassifier.Classify(metaExample).Label;

            return finalPredction;
          
        }

        public static IClassifier CreateMetaClassifier(IClassificationAlgorithm algorithm, Dataset trainingset, List<ClassifierInfo> ensemble)
        {            
            List<Data.Attribute> metaAttributes = new List<Data.Attribute>();

            for (int i = 0; i < ensemble.Count; i++)            
                metaAttributes.Add(new Data.Attribute(ensemble[i].Desc, i, new string[]{"Yes","No"}));
                
            Data.Attribute metaClass=new Data.Attribute("Class",ensemble.Count,new string[]{"Yes","No"});

            Metadata metadata = new Metadata("metaClassifier", metaAttributes.ToArray(), metaClass);

            List<Example> metaExamples = new List<Example>();

            foreach (Example example in trainingset)
            {
                List<int> predictions = new List<int>();

                foreach (ClassifierInfo info in ensemble)
                {
                    int predictoin = info.Classifier.Classify(example).Label;
                    predictions.Add(predictoin);
                }

                Example metaExample = new Example(metadata, example.Index, predictions.ToArray(), example.Label);
                metaExamples.Add(metaExample);

            }

            Dataset metaDataset = new Dataset(metadata);
            metaDataset.SetExamples(metaExamples.ToArray());

            algorithm.Dataset=metaDataset;
            IClassifier classifier = algorithm.CreateClassifier();
            return classifier;

        }

        public static IClassifier CreateMetaClassifier(IClassificationAlgorithm algorithm, List<Dataset> trainingsets, List<ClassifierInfo> ensemble)
        {
            List<Data.Attribute> metaAttributes = new List<Data.Attribute>();

            for (int i = 0; i < ensemble.Count; i++)
                metaAttributes.Add(new Data.Attribute(ensemble[i].Desc, i, new string[] { "Yes", "No" }));

            Data.Attribute metaClass = new Data.Attribute("Class", ensemble.Count, new string[] { "Yes", "No" });

            Metadata metadata = new Metadata("metaClassifier", metaAttributes.ToArray(), metaClass);

            List<Example> metaExamples = new List<Example>();

            int counter = 0;
            foreach (Example example in trainingsets[0])
            {
                List<int> predictions = new List<int>();

                for(int infoIndex=0; infoIndex<ensemble.Count; infoIndex++)
                {
                    ClassifierInfo info = ensemble[infoIndex];
                    int predictoin = info.Classifier.Classify(trainingsets[infoIndex][example.Index]).Label;
                    predictions.Add(predictoin);
                }


                Example metaExample = new Example(metadata, counter++, predictions.ToArray(), trainingsets[0][example.Index].Label);
                metaExamples.Add(metaExample);
            }

         

            Dataset metaDataset = new Dataset(metadata);
            metaDataset.SetExamples(metaExamples.ToArray());

            algorithm.Dataset = metaDataset;
            IClassifier classifier = algorithm.CreateClassifier();
            return classifier;

        }
    }
}
