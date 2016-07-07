using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iFourmi.ACO
{
    public class Ant<T>:ICloneable 
    {
        #region Data Members

        private int _index;
        private double _alpha;
        private double _beta;
        private Solution<T> _solution;
        private List<int> _trail;
        private AntColony<T> _colony;
        


        #endregion

        #region Properties

        public int Index
        {
            get { return this._index; }
        }

        public double Aplha
        {
            get { return this._alpha; }
        }
        
        public double Beta
        {
            get { return this._beta; }
        }

        public Solution<T> Solution
        {
            get { return this._solution; }
            set { this._solution = value; }
        }

        public List<int> Trail
        {
            get { return this._trail; }
            set { this._trail = value; }
        }
                

        #endregion

        #region Constructor

        public Ant(int index, AntColony<T> colony)
        {
            this._index = index;


            //this._alpha = Utilities.RandomUtility.GetNextDoubleFromGaussianFunction(2, 1);
            //if (this._alpha < 1)
            //    this._alpha = 1;

            //this._beta = 3 - this._alpha;
            //if (this._beta < 1)
            //    this._beta = 1;

            //this._alpha = 2;
            //this._beta = 2;

            //this._alpha = DataMining.Utilities.RandomUtility.GetNextDouble(1, 3);
            //this._beta = DataMining.Utilities.RandomUtility.GetNextDouble(1, 3);

            this._alpha = this._beta = 1;

            this._colony = colony;
            this._solution = new Solution<T>();
            this._trail = new List<int>();
        
        }

        #endregion

        #region Methods

        public void CreateSoltion()
        {
            while (true)
            {
                this._colony.ConstructionGraph.CalculateProbabilities(this._alpha, this._beta);
                List<DecisionComponent<T>> validElements = this._colony.ConstructionGraph.GetValidComponents();
                if (validElements.Count == 0)
                    break;
                DecisionComponent<T> element = this.SelectElementProbablistically(validElements);
                if (element == null)
                    break;
                this._solution.Components.Add(element);
                this._trail.Add(element.Index);                
                this._colony.Problem.ComponentInvalidator.Invalidate(element, this.Solution, this._colony.ConstructionGraph);
            }

        }

        private DecisionComponent<T> SelectElementProbablistically(List<DecisionComponent<T>> validElements)
        {
            DecisionComponent<T> chosen = null;
            
            double randomNumber = Utilities.RandomUtility.GetNextDouble(0, 1);
            double sum = 0;

            foreach (DecisionComponent<T> element in validElements)
            {
                sum += element.Probability;
                if (sum >= randomNumber)
                {
                    chosen = (DecisionComponent<T>)element.Clone();
                    break;
                }
            }

            return chosen;
        }

        public void DepositePheromone(double ratio)
        {
            foreach (int index in this._trail)
            {
                if (index != -1)
                {
                    double currentPhromoneLevel = this._colony.ConstructionGraph.Components[index].Pheromone;
                    this._colony.ConstructionGraph.Components[index].Pheromone += currentPhromoneLevel * this.Solution.Quality * ratio;

                    //this._colony.ConstructionGraph.Components[index].Pheromone  += this.Solution.Quality;

                    if (this._colony.ConstructionGraph.Components[index].Pheromone < 0)
                        this._colony.ConstructionGraph.Components[index].Pheromone = 0.001;
                }
            }

        }

        public object Clone()
        {
            Ant<T> clone = new Ant<T>(this._index, this._colony);
            clone._solution = this._solution;
            clone._trail = this._trail;
            return clone;
        }
        
        #endregion
       
    }
}
