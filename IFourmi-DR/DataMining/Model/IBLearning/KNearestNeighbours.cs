using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Model.IBLearning
{
    public class KNearestNeighbours:AbstractLazyClassifier
    {
        private int _k;

        private bool _useWeightedVote;

        public int KNeighbours
        {
            get
            { return this._k; }
            set
            {
                this._k = value;
            }
        }

         public KNearestNeighbours(IDistanceMeasure distanceMeasure, Dataset database, double[][] weights, bool useWeightedVote)
            : base(distanceMeasure, database, weights)
        {
            
            this._useWeightedVote = useWeightedVote;
        }

        public KNearestNeighbours(IDistanceMeasure distanceMeasure, Dataset database, double[] weights, bool useWeightedVote)
            :base(distanceMeasure,database,weights)
        {
            
            this._useWeightedVote = useWeightedVote;
        }

        public KNearestNeighbours(IDistanceMeasure distanceMeasure, Dataset database, bool useWeightedVote)
            : base(distanceMeasure, database)
        {

            this._useWeightedVote = useWeightedVote;

        }

        public KNearestNeighbours(int k, IDistanceMeasure distanceMeasure, Dataset database, double[][] weights, bool useWeightedVote)
            : base(distanceMeasure, database, weights)
        {
            this._k = k;
            this._useWeightedVote = useWeightedVote;
        }

        public KNearestNeighbours(int k, IDistanceMeasure distanceMeasure, Dataset database, double[] weights, bool useWeightedVote)
            :base(distanceMeasure,database,weights)
        {
            this._k=k;
            this._useWeightedVote = useWeightedVote;
        }

        public KNearestNeighbours(int k, IDistanceMeasure distanceMeasure, Dataset database, bool useWeightedVote)
            : base(distanceMeasure, database)
        {
            this._k = k;
            this._useWeightedVote = useWeightedVote;
       
        }

        public override Model.Prediction Classify(Data.Instance instance)
        {
            double[] classCounts = new double[this._database.Metadata.Target.Values.Length];

            double[] distances = new double[this._database.Size];
            int[] instanceIndexes = new int[this._database.Size];

            for (int instanceIndex = 0; instanceIndex < this._database.Size; instanceIndex++)
            {
                if (this._database[instanceIndex] == instance)
                    continue;

                instanceIndexes[instanceIndex] = instanceIndex;
                double distance=this._distanceMeasure.CalculateDistance(instance, this._database[instanceIndex],this._classBasedWeights);
                if(distance==0)
                    distances[instanceIndex] = distance;
                distances[instanceIndex] = distance; 
            }
            Array.Sort(distances, instanceIndexes);

            int max=0;

            for (int counter = 0; counter < this._k; counter++)
            {
                int instanceIndex = instanceIndexes[counter];
                int predicted = this._database[instanceIndex].Label;

                if (this._useWeightedVote)
                    classCounts[predicted] += (1.0/1.0+distances[instanceIndex]);
                else
                    classCounts[predicted] += 1;

                if(classCounts[predicted]>classCounts[max])
                    max=predicted;               

            }

            double sum=classCounts.Sum();
            for(int i=0; i<classCounts.Length;i++)
                classCounts[i]/=sum;


            Prediction prediction = new Prediction(max, classCounts);
            return prediction;


                       
        }
    }
}
