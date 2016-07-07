using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    [Serializable()]
    public class Metadata
    {

        #region Data Members

        private string _name;
        private bool _isHierarchical;
        private Attribute[] _attributes;
        private NominalAttribute  _target;
        private int _size;
        
        
        
        #endregion
        
        #region Properties


        public string DatasetName
        {
            get { return this._name; }
 
        }

        public Attribute[] Attributes
        {
            get { return this._attributes; }

        }

        public NominalAttribute Target
        {
            get { return this._target; }
        }


        public bool IsHierarchical
        {
            get { return this._isHierarchical; }
        }

        public int Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        public int ClassIndex
        {
            get
            {
                return this.Attributes.Length;
            }
        }

        #endregion

        #region Constructors

        public Metadata(string name,Attribute[] attributes, NominalAttribute target,bool isHierarchical)
        {
            this._name = name;
            this._attributes = attributes;
            this._target = target;
            this._isHierarchical = isHierarchical;
        }

        #endregion

        #region Methods


        public Metadata Clone()
        {
            NominalAttribute target = this._target.Clone() as NominalAttribute;

            DataMining.Data.Attribute [] attClone=new Attribute[this._attributes.Length];
            for(int attributeIndex=0; attributeIndex< this._attributes.Length; attributeIndex++)
                attClone[attributeIndex ]=this._attributes[attributeIndex].Clone();
           

            Metadata clone = new Metadata(_name, attClone, target,_isHierarchical);
            return clone;
        }

        #endregion

      
    }
}
