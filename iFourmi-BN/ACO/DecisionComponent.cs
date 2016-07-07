using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO
{
    public class DecisionComponent<T>: ICloneable
    {
        #region Data Members

        private int _index;
        private T _element;
        private double _heuristic;
        private double _pheromone;
        private double _probability;
        private bool _isValid;

        #endregion

        #region Properties

        public int Index
        {
            get { return this._index; }
        }

        public T Element
        {
            get { return this._element; }
        }

        public double Heuristic
        {
            get { return this._heuristic; }
            set { this._heuristic = value; }
           
        }

        public double Pheromone
        {
            get { return this._pheromone; }
            set { this._pheromone = value; }

        }

        public double Probability
        {
            get { return this._probability; }
            set { this._probability = value; }
        }

        public bool IsValid
        {
            get { return this._isValid; }
            set { this._isValid = value; }
        }


        #endregion

        #region Constructors

        public DecisionComponent(int index,T element)
        {
            this._index = index;
            this._element = element;
            this._isValid = true;
        }
   
        #endregion
        
        #region Methods

        public void SetHieuristicValue(double value)
        {
            this._heuristic = value;
        }


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{" + this._element.ToString() + "}");
            builder.Append("(");
            builder.Append("He: " + Math.Round(this._heuristic,3).ToString()+", ");
            builder.Append("Ph: " + Math.Round(this.Pheromone, 3).ToString() + ", ");
            builder.Append("Pr: " + Math.Round(this.Probability, 3).ToString() + ", ");
            builder.Append("Valid: " + this._isValid.ToString());
            builder.Append(")");
            return builder.ToString();
        }

        public object Clone()
        {
            return new DecisionComponent<T>(this._index, this._element);
        }

        #endregion


       
    }
}
