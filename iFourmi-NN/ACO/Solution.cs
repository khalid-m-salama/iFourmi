using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.ACO
{
    public class Solution<T>
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

        public virtual List<T> ToList()
        {
            List<T> list = new List<T>();

            for(int i=0;i<this._components.Count;i++)
                list.Add(this._components[i].Element);
            return list;
        }

        #endregion
    }
}
