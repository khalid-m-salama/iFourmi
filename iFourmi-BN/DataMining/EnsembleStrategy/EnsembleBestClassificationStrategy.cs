using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.EnsembleStrategy
{
    public class EnsembleBestClassificationStrategy:IEnsembleClassificationStrategy
    {
        public int Classify(List<ClassifierInfo> classifiersInfo,Data.Example example)
        {
            ClassifierInfo best = classifiersInfo[0];
            foreach (ClassifierInfo current in classifiersInfo)
            {
                if (current.Quality > best.Quality)
                    best = current;
            }

            return best.Classifier.Classify(example).Label;
        }


        public int Classify(List<ClassifierInfo> classifiersInfo, List<Data.Example> examples)
        {
            ClassifierInfo best = classifiersInfo[0];
            foreach (ClassifierInfo current in classifiersInfo)
            {
                if (current.Quality > best.Quality)
                    best = current;
            }

            Data.Example bestExample = examples.Find(e => best.Desc.Contains(e.Metadata.DatasetName));
            return best.Classifier.Classify(bestExample).Label;
        }
    }
}
