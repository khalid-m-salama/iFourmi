using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO
{
    public class Solution<T> : ICloneable
    {
        #region Data Members

        protected List<DecisionComponent<T>> _components;

        protected double _quality;

        #endregion

        #region Properties

        public List<DecisionComponent<T>> Components
        {
            get { return this._components; }
        }

        public double Quality
        {
            get { return this._quality; }
            set
            { 
                this._quality = value; 
            }
        }



        #endregion

        #region Constructors

        public Solution()
        {
            this._components = new List<DecisionComponent<T>>();
        }

        #endregion

        #region Methods

        public static Solution<T> FromList(List<T> list)
        {
            Solution<T> solution = new Solution<T>();
            for (int index = 0; index < list.Count; index++)
                solution.Components.Add(new DecisionComponent<T>(index, list[index]));

            return solution;
        }

        public virtual List<T> ToList()
        {
            List<T> list = new List<T>();

            for (int i = 0; i < this._components.Count; i++)
                list.Add(this._components[i].Element);
            return list;
        }

        public object Clone()
        {
            Solution<T> clone=new Solution<T>();
            foreach (DecisionComponent<T> component in this._components)
                clone._components.Add(component);
            clone._quality = this._quality;

            return clone;
        }

        #endregion


    }
}
