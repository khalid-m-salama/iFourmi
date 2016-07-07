using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.ClassificationMeasures;


namespace iFourmi.DataMining.Model.Hierarchical
{
    public class LocalPerNodeClassifier:LocalClassifier
    {
               
        #region Constructors

        public LocalPerNodeClassifier(ClassHierarchy hierarchy,EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy)
            : base(hierarchy, ensembleStrategy) { }

        #endregion

        #region Methods

        public override int[] Classify(Data.Example example)
        {
            List<int> prediceted = new List<int>();

            Queue<Node> candidates = new Queue<Node>();
            foreach(string child in this._hierarchy.Root.Children)
                candidates.Enqueue(this._hierarchy[child]);

            while (candidates.Count != 0)
            {
                Node current = candidates.Dequeue();
                
                List<ClassifierInfo> classifiersInfo = this[current.Name];
                if (classifiersInfo != null)
                {
                    int label = _ensembleStrategy.Classify(classifiersInfo, example);
                    if (label == 0)
                    {
                        prediceted.Add(current.ValueIndex);
                        if (current.Children != null)
                            foreach (string child in current.Children)
                                candidates.Enqueue(this._hierarchy[child]);

                        
                    }
                }

            }

            return prediceted.Distinct().ToArray();
        }

        public override int[] Classify(List<Data.Example> examples)
        {

            List<int> prediceted = new List<int>();

            Queue<Node> candidates = new Queue<Node>();
            foreach (string child in this._hierarchy.Root.Children)
                candidates.Enqueue(this._hierarchy[child]);

            while (candidates.Count != 0)
            {
                Node current = candidates.Dequeue();
                
                List<ClassifierInfo> classifiersInfo = this[current.Name];
                if (classifiersInfo != null)
                {                 
                    
                    int label = _ensembleStrategy.Classify(classifiersInfo, examples);
                    if (label == 0)
                    {
                        prediceted.Add(current.ValueIndex);
                        if (current.Children != null)
                            foreach (string child in current.Children)
                                if(!candidates.Contains(this._hierarchy[child]))
                                    candidates.Enqueue(this._hierarchy[child]);


                    }
                }

            }

            return prediceted.Distinct().ToArray();
        }

        #endregion



       
    }
}
