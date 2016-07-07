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

        public void SetHeuristicValues(ProblemSpecifics.IHeuristicValueCalculator<T> calculator, bool normalize)
        {
            double sum = 0;
            foreach (DecisionComponent<T> element in this._components)
            {
                calculator.CalculateHieristicValue(element);
                sum += element.Heuristic;
            }


            if (normalize)
            {
                foreach (DecisionComponent<T> element in this._components)
                {
                    element.Heuristic /= sum;
                }
            }

        }    

        public void CalculateProbabilities(double alpha, double beta)
        {
            List<DecisionComponent<T>> validComponents = new List<DecisionComponent<T>>();

            double sum = 0;
            foreach (DecisionComponent<T> component in this._components)
            {
                if (component.IsValid)
                {
                    component.Probability = Math.Pow(component.Pheromone, alpha) * Math.Pow(component.Heuristic, beta);
                    if (!double.IsNaN(component.Probability))
                    {
                        validComponents.Add(component);
                        sum += component.Probability;
                    }

                }
            }

            foreach (DecisionComponent<T> component in validComponents)
                component.Probability = component.Probability / sum;
 
        }
        
        public void ResetValidation()
        {
            foreach (DecisionComponent<T> element in this._components)
                element.IsValid = true;
        }

        public void ResetValidation(bool isValid)
        {
            foreach (DecisionComponent<T> element in this._components)
                element.IsValid = isValid;
        }
         
        public  void EvaporatePheromone()
        {
            this.NormalizePheromone();
            
        }

        public void EvaporatePheromone(double value)
        {
            foreach (DecisionComponent<T> element in this._components)
            {
                element.Pheromone = element.Pheromone * (1 - value);
                if (double.IsNaN(element.Pheromone))
                    element.Pheromone = 0.001;
            }
        }

        private void SetMaxMinLevels()
        {
            

            double sum = 0;
            foreach (DecisionComponent<T> element in this._components)
                sum += element.Pheromone;
          

            foreach (DecisionComponent<T> element in this._components)
                element.Pheromone = element.Pheromone / sum;

            sum = 0;
            double minLevel = 1.0 / (this._components.Length * 10.0);

            foreach (DecisionComponent<T> element in this._components)
            {
                if (element.Pheromone < minLevel)
                    element.Pheromone += minLevel;
                sum += element.Pheromone;
            }


            foreach (DecisionComponent<T> element in this._components)
                element.Pheromone = element.Pheromone / sum;
        }

        private void NormalizePheromone()
        {
            double sum = 0;
            foreach (DecisionComponent<T> element in this._components)
                sum += element.Pheromone;


            foreach (DecisionComponent<T> element in this._components)
                element.Pheromone = element.Pheromone / sum;


        }  

        public List<DecisionComponent<T>> GetValidComponents()
        {
            List<DecisionComponent<T>> validElements = new List<DecisionComponent<T>>();
            foreach (DecisionComponent<T> element in this._components)
                if (element.IsValid)
                    validElements.Add(element);
            return validElements;
        }

       
        #endregion


        
    }
}
