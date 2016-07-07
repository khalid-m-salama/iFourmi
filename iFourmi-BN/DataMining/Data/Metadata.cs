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
        private Dictionary<string, int> _indexes;
        private Attribute _target;
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

        public Attribute Target
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

        #endregion

        #region Constructors

        public Metadata(string name,Attribute[] attributes, Attribute target)
        {
            this._name = name;
            this._attributes = attributes;
            this._target = target;
            this._indexes = new Dictionary<string, int>();

            if (target is HierarchicalAttribute)
                this._isHierarchical = true;


            foreach (Attribute attribute in attributes)
            {
                this._indexes.Add(attribute.Name, attribute.Index);
                
            }
        }

        #endregion

        #region Methods

        public int GetAttributeIndex(string attributeName)
        {
            if (this._indexes.ContainsKey(attributeName))
                return this._indexes[attributeName];
            else
                return -1;

        }

        public Metadata Clone()
        {
            DataMining.Data.Attribute target=_target.Clone();

            DataMining.Data.Attribute [] attClone=new Attribute[this._attributes.Length];
            for(int attributeIndex=0; attributeIndex< this._attributes.Length; attributeIndex++)
                attClone[attributeIndex ]=this._attributes[attributeIndex].Clone();
           

            Metadata clone = new Metadata(_name, attClone, target);
            return clone;
        }

        #endregion

      
    }
}
