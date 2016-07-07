using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace iFourmi.ACO
{
    public class ConstructionGraph<T>
    {
        #region Data Members

        private DecisionComponent<T> [] _components;
        

        #endregion

        #region Properties

        public DecisionComponent<T>[] Components
        {
            get { return this._components; }
        }
        
        #endregion

        #region Constructor

        public ConstructionGraph(DecisionComponent<T>[] elements)
        {
            this._components = elements;
        }

        #endregion

        #region Methods

        public void InitializePheromone()
        {
            for (int index = 0; index < this._components.Length; index++)
                this._components[index].Pheromone = 1 / (double)this._components.Length;

        }

        public void InitializePheromone(double value)
        {
            for (int index = 0; index < this._components.Length; index++)
                this._components[index].Pheromone = value;

        }

        public void SetHeuristicValues(ProblemSpecifics.IHeuristicsCalculator<T> calculator, bool normalize)
        {
            double sum = 0;
            for(int i=0; i<this._components.Length;i++)            
            {
                calculator.CalculateHeuristics(this._components[i]);
                sum += this._components[i].Heuristic;
            }


            if (normalize)
            {
                for (int i = 0; i < this._components.Length; i++)      
                {
                    this._components[i].Heuristic /= sum;
                }
            }

        }    

        public List<DecisionComponent<T>> CalculateProbabilities(double alpha, double beta)
        {
            List<DecisionComponent<T>> potentialComponents = new List<DecisionComponent<T>>();

            double sum = 0;
            List<DecisionComponent<T>> validComponents=this.GetValidComponents();
            
            for(int i=0; i<validComponents.Count;i++)
            {
               
                    validComponents[i].Probability = Math.Pow(validComponents[i].Pheromone, alpha) * Math.Pow(validComponents[i].Heuristic, beta);
                    if (!double.IsNaN(validComponents[i].Probability))
                    {
                        potentialComponents.Add(validComponents[i]);
                        sum += validComponents[i].Probability;
                    }

            }

            for (int i = 0; i < potentialComponents.Count; i++)
                potentialComponents[i].Probability = potentialComponents[i].Probability / sum;

            return potentialComponents;
 
        }

        public List<DecisionComponent<T>> CalculateProbabilities(double alpha, double beta, int startIndex,int length)
        {
            List<DecisionComponent<T>> potentialComponents = new List<DecisionComponent<T>>();

            double sum = 0;
            List<DecisionComponent<T>> validComponents = this.GetValidComponents(startIndex, length);

            for (int i = 0; i < validComponents.Count; i++)
            {

                validComponents[i].Probability = Math.Pow(validComponents[i].Pheromone, alpha) * Math.Pow(validComponents[i].Heuristic, beta);
                if (!double.IsNaN(validComponents[i].Probability))
                {
                    potentialComponents.Add(validComponents[i]);
                    sum += validComponents[i].Probability;
                }

            }

            for (int i = 0; i < potentialComponents.Count; i++)
                potentialComponents[i].Probability = potentialComponents[i].Probability / sum;

            return potentialComponents;
        }
        
        public void ResetValidation()
        {
            for (int i = 0; i < this._components.Length; i++)
                this._components[i].IsValid = true;
        }

        public void ResetValidation(bool isValid)
        {
            for (int i = 0; i < this._components.Length; i++)
                this._components[i].IsValid = isValid;
        }
         
        public  void EvaporatePheromone()
        {
            this.NormalizePheromone();
            //this.SetMaxMinLevels();
        }

        public void EvaporatePheromone(double value)
        {
            for (int i = 0; i < this._components.Length; i++)
            {
                this._components[i].Pheromone = this._components[i].Pheromone * (1 - value);
                if (double.IsNaN(this._components[i].Pheromone))
                    this._components[i].Pheromone = 0.001;
            }

        }

        private void NormalizePheromone()
        {
            double sum = 0;
            for (int i = 0; i < this._components.Length; i++)
                sum += this._components[i].Pheromone;


            for (int i = 0; i < this._components.Length; i++)
                this._components[i].Pheromone = this._components[i].Pheromone / sum;


        }  

        public List<DecisionComponent<T>> GetValidComponents()
        {
            List<DecisionComponent<T>> validElements = new List<DecisionComponent<T>>();
            for (int i = 0; i < this._components.Length; i++)
                if (this._components[i].IsValid)
                    validElements.Add(this._components[i]);
            return validElements;
        }

        public List<DecisionComponent<T>> GetValidComponents(int startIndex,int length)
        {
            
            List<DecisionComponent<T>> validElements = new List<DecisionComponent<T>>();

            for(int index=startIndex;index<startIndex+length;index++)
            {
                DecisionComponent<T> component = this._components[index];
                if (component.IsValid)                
                    validElements.Add(component);
                    
            }
            return validElements;
        }

       
        #endregion


        
    }
}
