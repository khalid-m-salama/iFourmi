using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.Algorithms;


namespace iFourmi.DataMining.Model.Hierarchical
{
    public class LocalPerNodeClassifier : LocalClassifier
    {

        #region Constructors

        public LocalPerNodeClassifier(ClassHierarchy hierarchy)
            : base(hierarchy) { }

        #endregion

        #region Methods

        public override Prediction Classify(Data.Instance instance)
        {
            List<int> prediceted = new List<int>();

            Queue<Node> candidates = new Queue<Node>();
            foreach (string child in this._hierarchy.Root.Children)
                candidates.Enqueue(this._hierarchy[child]);

            while (candidates.Count != 0)
            {
                Node current = candidates.Dequeue();
                IClassifier classifier = this[current.Name];
                Prediction prediction = classifier.Classify(instance);


                if (prediction.Label == 0)
                {
                    prediceted.Add(current.ValueIndex);
                    if (current.Children != null)
                        foreach (string child in current.Children)
                            candidates.Enqueue(this._hierarchy[child]);


                }


            }

            double[] probabilities = new double[instance.Metadata.Target.Values.Length];
            foreach (int index in prediceted)
                probabilities[index] = 1;

            Prediction final = new Prediction(-1, probabilities);

            return final;
        }


        #endregion




    }
}
