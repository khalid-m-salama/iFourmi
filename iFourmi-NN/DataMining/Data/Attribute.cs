using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    [Serializable()]
    public class Attribute
    {

        #region Data Members

        private string _name;
        private int _index;

        

        #endregion

        #region Properties

        public string Name
        {
            get { return this._name; }
        }

        public int Index
        {
            get { return this._index; }
        }


        #endregion

        #region Constructor

        public Attribute(string name, int index)
        {
            this._index = index;
            this._name = name;
 
        }

        #endregion

        #region Methods


        public override string ToString()
        {
            return this._name+" Numeric";
        }
        #endregion


        internal Attribute Clone()
        {
             Data.Attribute clone = new Attribute(this._name, this._index);
             return clone;
            
        }
    }
}
