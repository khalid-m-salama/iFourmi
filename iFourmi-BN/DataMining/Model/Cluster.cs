using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;

namespace iFourmi.DataMining.Model
{
    public class Cluster
    {
        #region Data Members

        private ClusteringSolution  _clusteringSolution;

        private int _label;

        private List<int> _exampleIndexes;
        
        #endregion

        #region Properties

        public int Label
        {
            get
            {
                return this._label;
            }
                       
        }
        
        public Example [] Examples
        {
            get
            {
                List<Example> examples=new List<Example>();
                foreach(int index in this._exampleIndexes)
                    examples.Add(this._clusteringSolution.Dataset[index]);

                return examples.ToArray();
            }
        }

        public int Size 
        {
            get 
            {
                return this._exampleIndexes.Count;
            }
        }
        
        #endregion

        #region Constructors

        public  Cluster(ClusteringSolution clusteringSolution, int lable)
        {
            this._clusteringSolution = clusteringSolution;
            this._exampleIndexes = new List<int>();
            this._label = lable;
        }

        #endregion

        #region Methods

        public void AddExample(int exampleIndex)
        {
            if (this._exampleIndexes.Contains(exampleIndex))
                throw new Exception("The example already belongs to the cluster");
            
            this._exampleIndexes.Add(exampleIndex);
        }

        public void RemoveExample(int exampleIndex)
        {
            if (!this._exampleIndexes.Contains(exampleIndex))
                throw new Exception("The example does not exists");

            this._exampleIndexes.Remove(exampleIndex);
        }
        
        public double CalculateBelongingness(Example example)
        {
            double belongingness = 0;
            foreach(int index in this._exampleIndexes)
                belongingness += this._clusteringSolution.ProximityMatrix[example.Index, index];

            belongingness /= this.Size;
            return belongingness;

        }

        public double CalculateCohesion()
        {
            double cohesion = 0;
            int counter = 0;
            
            for (int index1 = 0; index1 < this.Size - 1; index1++)
            {
                for (int index2 = index1 + 1; index2 < this.Size; index2++)
                {
                    int exampleIndex1 = this._exampleIndexes[index1];
                    int exampleIndex2 = this._exampleIndexes[index2];

                    cohesion += this._clusteringSolution.ProximityMatrix[exampleIndex1, exampleIndex2];
                    counter++;
                    //cohesion += this._clusteringSolution.SimilarityMeasure.CalculateSimilarity(this._clusteringSolution.Dataset[exampleIndex1], this._clusteringSolution.Dataset[exampleIndex1]);
                }
            }                             

            //cohesion /= this.Size;
            cohesion /= counter;
            return cohesion;
        }

        public bool Contains(Example example)
        {
            return this._exampleIndexes.Contains(example.Index);

        }

        public int[] GetCentroid()
        {
            int attributesCount=this._clusteringSolution.Dataset.Metadata.Attributes.Length;
            int[] centroid = new int[attributesCount];

            if (this.Examples.Length == 0)
                throw new Exception("Empty Cluster Error");
            

            int [][] attributeValuesCount = new int [attributesCount][];
            foreach (Example example in this.Examples)
            {
                for (int attributeIndex = 0; attributeIndex < attributesCount; attributeIndex++)
                {
                    if (attributeValuesCount[attributeIndex] == null)
                        attributeValuesCount[attributeIndex] = new int[this._clusteringSolution.Dataset.Metadata.Attributes[attributeIndex].Values.Length];
                    int valueIndex = example[attributeIndex];
                    attributeValuesCount[attributeIndex][valueIndex] ++;
                }

            }

            for (int attributeIndex = 0; attributeIndex < attributesCount; attributeIndex++)
            {
                int maxCount = 0;
                
                for (int valueIndex = 0; valueIndex < this._clusteringSolution.Dataset.Metadata.Attributes[attributeIndex].Values.Length; valueIndex++)
                {
                    if (attributeValuesCount[attributeIndex][valueIndex] > maxCount)
                    {
                        centroid[attributeIndex] = valueIndex;
                        maxCount = attributeValuesCount[attributeIndex][valueIndex];
                    }
 
                }
            }

            return centroid;

        }

        public int GetMedoid()
        {
            int medoidExampleIndx = -1;
            int[] centroid = this.GetCentroid();
            Example centroidExample = new Example(this._clusteringSolution.Dataset.Metadata, -1, centroid, -1);

            double maxSimilarity = 0;

            foreach (Example example in this.Examples)
            {

                double currentSimilarity = this._clusteringSolution.SimilarityMeasure.CalculateSimilarity(centroidExample,example);
                if (currentSimilarity > maxSimilarity)
                {
                    maxSimilarity = currentSimilarity;
                    medoidExampleIndx = example.Index;
                }
            }

            return medoidExampleIndx;
        }

        public void Clear()
        {
            this._exampleIndexes.Clear();
        }

        public Dataset ConvertToDataset()
        {

            Dataset dataset = new Dataset(this._clusteringSolution.Dataset.Metadata.Clone());
            dataset.SetExamples(this.Examples);
            return dataset;

            //List<Example> examples = new List<Example>();
            //Dataset dataset = new Dataset(this._clusteringSolution.Dataset.Metadata.Clone());

            //counter = 0;
            //if(dataset.Metadata.IsHierarchical)
            //    foreach(Example  example in this.Examples)
            //        examples.Add(new Example(dataset.Metadata, counter++, example.Values, example.HLabel.ToList()));
            //else
            //    foreach (Example example in this.Examples)
            //        examples.Add(new Example(dataset.Metadata, counter++, example.Values, example.Label));                  

            //dataset.SetExamples(examples.ToArray());            
            //return dataset;
        }

        #endregion

     
    }
}
