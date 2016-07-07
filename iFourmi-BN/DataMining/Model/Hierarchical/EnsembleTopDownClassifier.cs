using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.Model.Hierarchical
{
    public abstract class EnsembleTopDownClassifier : IHierarchicalClassifier
    {
        #region Data Members

        protected ClassHierarchy _hierarchy;
        protected Dictionary<string, List<Tuple<string,IClassifier>>> _modelMapping;
        protected Dictionary<string, IClassifier> _metaModel;        
        protected List<string> _classifierNames;

        #endregion

        #region Properties


        protected Dictionary<string, List<Tuple<string, IClassifier>>> ModelMapping
        {
            get { return this._modelMapping; }
        }

        public Dictionary<string, IClassifier> MetaModel
        {
            get { return this._metaModel; }
        }
               

        #endregion


        #region Constructor

        public EnsembleTopDownClassifier(ClassHierarchy hierarchy)
        {
            this._hierarchy = hierarchy;            
            this._modelMapping = new Dictionary<string, List<Tuple<string, IClassifier>>>();
            this._metaModel = new Dictionary<string, IClassifier>();
            this._classifierNames = new List<string>();
        }

        #endregion

        #region Method

        public void AddClassifier(string label, string description,IClassifier classifier)
        {
            Tuple<string, IClassifier> tuple = new Tuple<string, IClassifier>(description, classifier);
            List<Tuple<string,IClassifier>> classifiers = this._modelMapping[label];
            if (classifiers == null)
                classifiers = new List<Tuple<string, IClassifier>>();
            classifiers.Add(tuple);
            this._classifierNames.Add(description);
        }

        public abstract int[] Classify(Data.Example example);
        #endregion
    }
 	

}
