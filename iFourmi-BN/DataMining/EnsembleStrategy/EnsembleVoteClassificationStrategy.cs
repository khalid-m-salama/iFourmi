using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.EnsembleStrategy
{
    public class EnsembleVoteClassificationStrategy:IEnsembleClassificationStrategy
    {
        public int Classify(List<ClassifierInfo> classifiersInfo,Data.Example example)
        {
            
            Dictionary<int, int> votesCount = new Dictionary<int, int>();

            foreach (ClassifierInfo current in classifiersInfo)
            {
                int label = current.Classifier.Classify(example).Label;
                if (!votesCount.Keys.Contains(label))
                    votesCount.Add(label, 1);
                else
                    votesCount[label] += 1;
                
            }

            int maxVote = -1;
            int resultLable = -1;

            foreach(KeyValuePair<int,int> entry in votesCount)
                if (entry.Value > maxVote)
                {
                    maxVote = entry.Value;
                    resultLable = entry.Key;
                }


            return resultLable;      

        }


        public int Classify(List<ClassifierInfo> classifiersInfo, List<Data.Example> examples)
        {
            

            Dictionary<int, int> votesCount = new Dictionary<int, int>();

            foreach (ClassifierInfo current in classifiersInfo)
            {
                Data.Example example = examples.Find(e => current.Desc.Contains(e.Metadata.DatasetName));

                int label = current.Classifier.Classify(example).Label;
                if (!votesCount.Keys.Contains(label))
                    votesCount.Add(label, 1);
                else
                    votesCount[label] += 1;

            }

            int maxVote = -1;
            int resultLable = -1;

            foreach (KeyValuePair<int, int> entry in votesCount)
                if (entry.Value > maxVote)
                {
                    maxVote = entry.Value;
                    resultLable = entry.Key;
                }


            return resultLable; 
        }
    }
}
