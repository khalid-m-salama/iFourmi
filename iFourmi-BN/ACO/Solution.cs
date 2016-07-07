using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO
{
    public class Solution<T>
    {
        #region Data Members

        private List<DecisionComponent<T>> _components;

        private double _quality;

        #endregion

        #region Properties

        public List<DecisionComponent<T>> Components
        {
            get { return this._components; }
        }

        public double Quality
        {
            get { return this._quality; }
            set { this._quality = value; }
        }

        
        #endregion

        #region Constructors

        public Solution()
        {
            this._components = new List<DecisionComponent<T>>();
        }

        #endregion

        #region Methods

        public List<T> ToList()
        {
            List<T> list = new List<T>();
            foreach (DecisionComponent<T> component in this._components)
                list.Add(component.Element);
            return list;
        }

        #endregion
    }
}
