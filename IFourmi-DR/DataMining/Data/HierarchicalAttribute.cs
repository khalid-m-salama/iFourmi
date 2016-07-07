using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class HierarchicalAttribute:Data.NominalAttribute
    {
        #region Data Members
        private ClassHierarchy _hierarchy;
        #endregion

        #region Properties

        public ClassHierarchy Hierarchy
        {
            get { return this._hierarchy; }
        }

        #endregion


        public HierarchicalAttribute(string name, int index, string[] values, ClassHierarchy hierarchy)
            : base(name, index, values)
        {
            this._hierarchy = hierarchy ;
 
        }

        public override string ToString()
        {
            return base.ToString();
            
        }

    }

    
}
