using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class NumericAttribute:Data.Attribute
    {
       #region Constructor

        public NumericAttribute(string name, int index)         
        {
            this._index = index;
            this._name = name;       
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return this._name + " Numeric";
        }


        public override Attribute Clone()
        {
            Data.NumericAttribute clone = new NumericAttribute(this._name, this._index);
            return clone;
        }

        #endregion
    }
}
