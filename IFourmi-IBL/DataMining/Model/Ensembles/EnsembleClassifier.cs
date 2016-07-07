using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
namespace iFourmi.DataMining.Model.Ensembles
{
    public class EnsembleClassifier:IClassifier
    {
        
        protected List<ClassifierInfo> _ensemble;

        public Metadata Metadata
        {
            get
            {
                return null;
            }
        }

        public IEnsembleClassificationStrategy Stratgy
        {
            get;
            set;
        }

        public string Desc
        {
            get { return "Ensemble -" + this.Stratgy.ToString(); }
            set
            {

            }
        }

        public IClassifier this[int index]
        {
            get { return this._ensemble[index].Classifier; }
        }

        public EnsembleClassifier(List<ClassifierInfo> ensemble)
        {
            this._ensemble = ensemble;
        }
        
        public Prediction Classify(Example example)
        {
            List<KeyValuePair<ClassifierInfo, Prediction>> ensemblePredictions = new List<KeyValuePair<ClassifierInfo, Prediction>>();

            foreach (ClassifierInfo classifierInfo in this._ensemble)
            {
                Prediction prediction = classifierInfo.Classifier.Classify(example);
                ensemblePredictions.Add(new KeyValuePair<ClassifierInfo,Prediction>(classifierInfo,prediction));
                
            }

            Prediction finalPrediction = this.Stratgy.AggregatePredictions(ensemblePredictions);

            return finalPrediction;

           
        }
    }
}
