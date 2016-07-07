using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.ProximityMeasures;

namespace iFourmi.DataMining.Model
{
    public class ClusteringSolution
    {

        #region Data Members

        private Dataset _dataset;
        
        private Cluster[] _clusters;        
        
        private ISimilarityMeasure _similarityMeasure;

        private int[] _belongingness;
        

        #endregion
        
        #region Properties

        public Dataset Dataset
        {
            get
            {
                return this._dataset;
            }
        }

        public int ClusterNumber
        {
            get
            {
                return this._clusters.Length;
            }
        }

        public Cluster[] Clusters
        {
            get
            {
                return this._clusters;
            }
        }

    
        public double[,] ProximityMatrix
        {
            get;
            set;
        }

        public ISimilarityMeasure SimilarityMeasure
        {
            get
            {
                return this._similarityMeasure;
            }
        }

           public Cluster this[int index]
        {
            get
            {
                return this._clusters[index];
            }
        }

        public bool IsValid
        {
            get
            {
                int sum = 0;
                foreach (Cluster cluster in this._clusters)
                    sum += cluster.Size;

                return sum == this._dataset.Size;
            }
        }

        public int[] Belongingness
        {
            get
            {
               return this._belongingness;
            }
        }

    
        #endregion

        #region Constructors

        public ClusteringSolution(Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure)
        {
            this._dataset = dataset;            
            this._similarityMeasure = similarityMeasure;
            this.CalculateProximityMatrix();
            this._belongingness = new int[this._dataset.Size];

            this._clusters=new Cluster[clustersNumber];
            for (int index = 0; index < this._clusters.Length; index++)
                this._clusters[index] = new Cluster(this, index);

        }


        public ClusteringSolution(Dataset dataset, int clustersNumber, ISimilarityMeasure similarityMeasure,double[,] proximityMatrix)
        {
            this._dataset = dataset;
            this.ProximityMatrix = proximityMatrix;
            this._similarityMeasure = similarityMeasure;
            this._clusters = new Cluster[clustersNumber];
            for (int index = 0; index < this._clusters.Length; index++)
                this._clusters[index] = new Cluster(this, index);
            this._belongingness = new int[this._dataset.Size];

        }

        #endregion
        
        #region Methods

        private void CalculateProximityMatrix()
        {
            ProximityMatrix = new double[this.Dataset.Size, this.Dataset.Size];

            for (int index1 = 0; index1 < this._dataset.Size; index1++)
                for (int index2 = 0; index2 < this._dataset.Size; index2++)
                    ProximityMatrix[index1, index2] = this._similarityMeasure.CalculateSimilarity(this.Dataset[index1], this.Dataset[index2]);
        }

        public double CalculateCohesion()
        {
            double cohesion = 0;

            foreach (Cluster cluster in this._clusters)
                cohesion += cluster.CalculateCohesion();
            cohesion /= this.ClusterNumber;

            return cohesion;
            
        }

        public double CalculateSeparation()
        {
            double totalSimilarity = 0;
            int counter=0;

            for (int index1 = 0; index1 < this.ClusterNumber - 1; index1++)
            {
                for (int index2 = index1 + 1; index2 < this.ClusterNumber; index2++)
                {
                    counter++;

                    double interSimilarity = 0;

                    foreach(Example example in this._clusters[index1].Examples)
                        interSimilarity = this._clusters[index2].CalculateBelongingness(example);
                    interSimilarity /= this._clusters[index2].Size;

                    totalSimilarity += interSimilarity;
                }

                
                
            }
                    
            totalSimilarity /= counter;

            

            return 1/totalSimilarity;
        }

        public int GetExampleNativeCluster(Example example)
        {
            double maxBelongingness = 0;
            int maxClusterIndex = -1;
            
            foreach (Cluster cluster in this.Clusters)
            {
                double currenBelongingness = cluster.CalculateBelongingness(example);

                if (currenBelongingness > maxBelongingness)
                {
                    maxBelongingness = currenBelongingness;
                    maxClusterIndex = cluster.Label;
                }
            }

            return maxClusterIndex;
        }

        public void SetClusterExampleAssignment(List<ClusterExampleAssignment> assignments)
        {
            this.Clear();

            if (assignments.Count != this._dataset.Size)
                throw new Exception("Assigment has an inconsistent size");

            foreach (ClusterExampleAssignment assignment in assignments)
                this.AssignExample(this._dataset[assignment.ExampleID], assignment.ClusterLabel);
        }

        public void SetClusterExampleAssignment(List<int> medoidEampleIdexes)
        {
            this.Clear();

            foreach (Example example in this._dataset)
            {
                double maxSimilarity = 0;
                int maxClusterLabel = 0;

                for (int clusterLabel = 0; clusterLabel < this.ClusterNumber; clusterLabel++)
                {
                    int medoidIndex = medoidEampleIdexes[clusterLabel];

                    if (medoidIndex == example.Index)
                    {
                        maxClusterLabel = clusterLabel;
                        break;
                    }

                    double currentSimilarity = this.ProximityMatrix[medoidIndex , example.Index];
                    if (currentSimilarity >= maxSimilarity)
                    {
                        maxSimilarity = currentSimilarity;
                        maxClusterLabel = clusterLabel;
                    }
                }

                this.AssignExample(example, maxClusterLabel);
            }


        }

        public void AssignExample(Example example, int clusterLabel)
        {            
            foreach (Cluster cluster in this._clusters)
                if (cluster.Contains(example))
                    throw new Exception("The example already assigned to a cluster");

            this[clusterLabel].AddExample(example.Index);
            this._belongingness[example.Index] = clusterLabel;
        }

        public void DeassignExample(Example example, int clusterLabel)
        {
            this[clusterLabel].RemoveExample(example.Index);
            this._belongingness[example.Index] = -1;
        }

        public int[] GetMedoids()
        {
            int[] medoids = new int[this._clusters.Length];
            foreach (Cluster cluster in this._clusters)
                medoids[cluster.Label] = cluster.GetMedoid();
            return medoids;
        }
                
        public void Clear()
        {
            foreach (Cluster cluster in this._clusters)
                cluster.Clear();
        }

        #endregion



      
    }
}
