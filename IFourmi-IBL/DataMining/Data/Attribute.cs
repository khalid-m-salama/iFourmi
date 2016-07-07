using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    [Serializable()]
    public abstract class Attribute
    {

        #region Data Members

        protected string _name;
        protected int _index;
        

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
  

        #region Methods


     
        public abstract Data.Attribute Clone();

        #endregion


    }
}
