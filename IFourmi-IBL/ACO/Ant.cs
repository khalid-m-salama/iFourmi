using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iFourmi.ACO
{
    public class Ant<T>:ICloneable 
    {
        #region Data Members

        protected int _index;
        protected double _alpha;
        protected double _beta;
        protected Solution<T> _solution;
        protected List<int> _trail;
        protected AntColony<T> _colony;
        


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

        public virtual void CreateSoltion()
        {
            while (true)
            {
                List<DecisionComponent<T>> validComponents = this._colony.ConstructionGraph.CalculateProbabilities(this._alpha, this._beta);
                if (validComponents.Count == 0)
                    break;
                DecisionComponent<T> component = this.SelectElementProbablistically(validComponents);
                if (component == null)
                    break;
                this._solution.Components.Add(component);
                this._trail.Add(component.Index);                
                this._colony.Problem.ComponentInvalidator.Invalidate(component, this.Solution, this._colony.ConstructionGraph);
            }

        }

        protected DecisionComponent<T> SelectElementProbablistically(List<DecisionComponent<T>> validComponents)
        {
            DecisionComponent<T> chosen=null;
            
            double randomNumber = RandomUtility.Random(0, 1);
            double sum = 0;

            for(int i=0; i<validComponents.Count;i++)           
            {
                sum += validComponents[i].Probability;
                if (sum >= randomNumber)
                {
                    chosen = validComponents[i];
                    break;
                }
            }

            return chosen;
        }

        public void DepositPheromone(double ratio)
        {
            foreach (int index in this._trail)
            {
                if (index != -1)
                {
                    double currentPhromoneLevel = this._colony.ConstructionGraph.Components[index].Pheromone;
                    this._colony.ConstructionGraph.Components[index].Pheromone += currentPhromoneLevel * this.Solution.Quality * ratio;

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
