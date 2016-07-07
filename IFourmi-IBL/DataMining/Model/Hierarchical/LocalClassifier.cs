using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Algorithms;

namespace iFourmi.DataMining.Model.Hierarchical
{
    public abstract class LocalClassifier : IClassifier
    {
        #region Data Members
                
        protected Dictionary<string, IClassifier> _modelMapping;
        protected ClassHierarchy _hierarchy;
                
        #endregion

        #region Properties

        public IClassifier this[string label]
        {
            get { return this._modelMapping[label]; }

        }

        #endregion

        #region Constructor

        public LocalClassifier(ClassHierarchy hierarchy)
        {
            this._hierarchy = hierarchy;
            this._modelMapping = new Dictionary<string, IClassifier>();
            

        }


        #endregion

        #region Methods

        public void PutClassifier(string label, IClassifier classifier)
        {
            if (!this._modelMapping.Keys.Contains(label))
                this._modelMapping.Add(label, classifier);

        }

     

        public IClassifier GetClassifierWithDescription(string label)
        {
            return this._modelMapping[label];
        }

        public bool Contains(string label)
        {
            return this._modelMapping.Keys.Contains(label);
        }

        public abstract Prediction Classify(Example example);

        #endregion

        public Metadata Metadata
        {
            get
            {
                return null;
            }
            set
            {
 
            }
        }

      
        public string Desc
        {
            get;
        
            set;
       
        }
    }
}
