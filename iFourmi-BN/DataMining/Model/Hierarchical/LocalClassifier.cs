using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;


namespace iFourmi.DataMining.Model.Hierarchical
{
    public abstract class LocalClassifier:IHierarchicalClassifier
    {
        #region Data Members

        protected Dictionary<string, List<ClassifierInfo>> _modelMapping;
        protected ClassHierarchy _hierarchy;
        protected EnsembleStrategy.IEnsembleClassificationStrategy _ensembleStrategy;

        #endregion

        #region Properties

        public List<ClassifierInfo> this[string label]
        {
            get { return this._modelMapping[label]; }
            
        }

        #endregion

        #region Constructor

        public LocalClassifier(ClassHierarchy hierarchy, EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy)
        {
            this._hierarchy = hierarchy;
            this._modelMapping = new Dictionary<string, List<ClassifierInfo>>();
            this._ensembleStrategy = ensembleStrategy;
           
        }

       
        #endregion
              
        #region Methods

        public void PutClassifier(string label, ClassifierInfo info)
        {
            if (!this._modelMapping.Keys.Contains(label))
                this._modelMapping.Add(label, new List<ClassifierInfo>());

            this._modelMapping[label].Add(info);
        }

        public void PutEnsemble(string label, List<ClassifierInfo> list)
        {
            if (!this._modelMapping.Keys.Contains(label))
                this._modelMapping.Add(label, list);

            this._modelMapping[label] = list;
        }

        public List<ClassifierInfo> GetClassifierWithDescription(string label)
        {
            return this._modelMapping[label];
        }

        public  bool Contains(string label)
        {
            return this._modelMapping.Keys.Contains(label);
        }

        public abstract int[] Classify(Example example);

        public abstract int[] Classify(List<Example> example);

        #endregion
    }
}
