using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.Model.Hierarchical;
using iFourmi.DataMining.ClassificationMeasures;

namespace iFourmi.DataMining.Algorithms.Hierarchical
{
    public class LocalPerNodeClassificationAlgorithm:IHierarchicalClassificationAlgorithm
    {

        #region Data Members

        private IClassificationAlgorithm _algorithm;
        private List<Dataset> _trainingSets;
        private EnsembleStrategy.IEnsembleClassificationStrategy _ensembleStrategy;
        private IClassificationQualityMeasure _evaluator;
        private bool _serialize;
        private bool _fireEvents;
        #endregion

        #region Events

        public event EventHandler onPostClassifierConstruction;
        public event EventHandler onPostNodeProcessing;

        #endregion

        #region Constructors
        public LocalPerNodeClassificationAlgorithm(List<Dataset> trainingSets, IClassificationAlgorithm algorithm, EnsembleStrategy.IEnsembleClassificationStrategy ensembleStrategy, IClassificationQualityMeasure evaluator, bool serialize, bool fireEvents)
        {
            this._algorithm = algorithm;
            this._trainingSets = trainingSets;
            this._ensembleStrategy = ensembleStrategy;
            this._evaluator = evaluator;
            this._serialize = serialize;
            this._fireEvents = fireEvents;
        }

        #endregion

        #region Properties
        public Data.Dataset TrainingSet
        {
            get
            {
                return null;
            }
            set
            {
                
            }
        }

        #endregion

        #region Methods

        public Model.Hierarchical.IHierarchicalClassifier CreateClassifier()
        {
            ClassHierarchy hierarchy = ((Data.HierarchicalAttribute)this._trainingSets[0].Metadata.Target).Hierarchy;
            LocalPerNodeClassifier localClassifer = new LocalPerNodeClassifier(hierarchy,_ensembleStrategy);

            Node current = hierarchy.Root;
            Queue<Node> queue = new Queue<Node>();
            foreach (Node node in hierarchy.GetNodes(current.Children))
                queue.Enqueue(node);

            int counter = 1;

            while (queue.Count != 0)
            {
                current = queue.Dequeue();
                if (localClassifer.Contains(current.Name))
                    continue;


                List<ClassifierInfo> ensemble = CreateEnsemble(current);

                if (_ensembleStrategy is EnsembleStrategy.EnsembleStackClassificationStrategy)
                {  
                    List<Dataset> localDatasets = new List<Dataset>();
                    foreach(Dataset trainingSet in this._trainingSets)
                        localDatasets.Add(GetLocalFlatDataset(current, trainingSet));

                    IClassifier metaClassifier = EnsembleStrategy.EnsembleStackClassificationStrategy.CreateMetaClassifier(this._algorithm, localDatasets, ensemble);
                    ClassifierInfo metaInfo = new ClassifierInfo() { Desc = "metaClassifier", Classifier = metaClassifier, Quality = -1 };
                    ensemble.Add(metaInfo);
                }
               
                if (_serialize)
                {
                    IO.Serializer.SerializeEnsmeble(ensemble, current.Name);
                    localClassifer.PutEnsemble(current.Name, null);
                }
                else                
                    localClassifer.PutEnsemble(current.Name, ensemble); 
                                               

                if (_fireEvents)
                    this.onPostNodeProcessing(current, new NodeCounterEventArgs() { Counter = counter });

                counter++;
                
                if (current.Children != null)
                    foreach (string child in current.Children)
                        if(!queue.Contains(hierarchy[child]))
                            queue.Enqueue(hierarchy[child]);


            }

            if (_serialize)            
                foreach(Node node in hierarchy.GetDescendants(hierarchy.Root))
                    localClassifer.PutEnsemble(node.Name,IO.Serializer.DeserializeEnsemble(node.Name));
 

            return localClassifer;
        }

        private List<ClassifierInfo> CreateEnsemble(Node current)
        {
           List<ClassifierInfo> ensemble = new List<ClassifierInfo>();

            foreach (Dataset currentTrainingSet in this._trainingSets)
            {               
                Dataset local = GetLocalFlatDataset(current, currentTrainingSet);

                _algorithm.Dataset = local;
                IClassifier currentClassifier = _algorithm.CreateClassifier();
                double currentQyuality = _evaluator.CalculateMeasure(ConfusionMatrix.GetConfusionMatrixes(currentClassifier, local));
                ClassifierInfo info = new ClassifierInfo() { Classifier = currentClassifier, Desc = current.Name + ":" + currentTrainingSet.Metadata.DatasetName, Quality = currentQyuality };

                if (_fireEvents)
                {
                    ClassifierInfoEventArgs args = new ClassifierInfoEventArgs() { Info = info };
                    this.onPostClassifierConstruction(current, args);
                }

                ensemble.Add(info);
            }

            return ensemble;
 
        }

        private Dataset GetLocalFlatDataset(Node current,Dataset dataset)
        {
            ClassHierarchy hierarchy = ((Data.HierarchicalAttribute)dataset.Metadata.Target).Hierarchy;
            Node[] siblings = hierarchy.GetSiblings(current);
            List<string> negativeClassValues = new List<string>();
            foreach (Node sibling in siblings)
                negativeClassValues.Add(sibling.Name);


            Dataset local = dataset.GetBinaryFlatLabelSubDataset(current.Name, negativeClassValues.ToArray());
            return local;
        }


        #endregion
    }

    public class ClassifierInfoEventArgs : EventArgs
    {
        public ClassifierInfo Info
        {
            get;
            set;
        }
    }

    public class NodeCounterEventArgs : EventArgs
    {
        public int Counter
        {
            get;
            set;
        }
    }
}
