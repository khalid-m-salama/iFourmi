using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.NeuralNetworks.LearningMethods;
using iFourmi.NeuralNetworks.ActivationFunctions;

namespace iFourmi.NeuralNetworks.Model
{
    public class NeuralNetwork:IClassifier
    {
        
        #region Data Members

        private string _desc;

        private Metadata _metadata;

        private Neuron[] _inputLayer;

        private Neuron[] _hiddenLayer;

        private Neuron[] _outputLayer;
                
        private int _size;
        
        private IActivationFunction _activationFunction;

        #endregion

        #region Properties

        public Metadata Metadata
        {
            get { return this._metadata; }
        }

        public string Desc
        {
            get
            {
                return this._desc;
            }
            set
            {
                this._desc = value;
            }
        }

        public Neuron[] InputLayer
        {
            get { return this._inputLayer; }
        }

        public Neuron[] HiddenLayer
        {
            get { return this._hiddenLayer; }
        }

        public Neuron[] OutputLayer
        {
            get { return this._outputLayer; }
        }

        public Neuron[] NetNeurons
        {
            get           
            {
                List<Neuron> neurons = new List<Neuron>();

                neurons.AddRange(this._inputLayer);
                neurons.AddRange(this._hiddenLayer);
                neurons.AddRange(this._outputLayer);

                return neurons.ToArray();
            }
        }
       
        public IActivationFunction ActivationFunction
        {
            get { return this._activationFunction; }
        }

        public int Size
        {
            get { return this._size; }
        }

        #endregion

        #region Constructors

        public NeuralNetwork(Metadata metadata, int hiddenUnitCount, IActivationFunction activationFuntcion,  Connection[] connections)
        {
            this._metadata = metadata;
            
            this._inputLayer = new Neuron[this.Metadata.Attributes.Length];
            this._hiddenLayer = new Neuron[hiddenUnitCount];
            this._outputLayer = new Neuron[this.Metadata.Target.Length];
            this._activationFunction = activationFuntcion;                              
            this.CreateNetworkTopology(connections);


        }
               
        //public NeuralNetwork(Metadata metadata, int hiddenUnitCount,IActivationFunction activationFunction)
        //{
        //    this._metadata = metadata;

        //    this._inputLayer = new Neuron[this.Metadata.Attributes.Length];
        //    this._hiddenLayer = new Neuron[hiddenUnitCount];
        //    this._outputLayer = new Neuron[this.Metadata.Target.Length];
        //    this._activationFunction = activationFunction;

        //    Connection [] connections = this.Create3LayerConnectedConnections();
        //    this.CreateNetworkTopology(connections);

        //}

        #endregion

        #region Methods

        public static Connection[] Create3LayerConnectedConnections(Metadata metadata, int hiddenUnitCount)
        {
            int inputCount = metadata.Attributes.Length;
            int hiddenCount = hiddenUnitCount;
            int outputCunt = metadata.Target.Length;

            List<Connection> connections = new List<Connection>();

            for (int from = 0; from < inputCount; from++)
                for (int to = 0; to < hiddenCount; to++)
                    connections.Add(new Connection(from, to, LayerType.Input, LayerType.Hidden));

            for (int from = 0; from < hiddenCount; from++)
                for (int to = 0; to < outputCunt; to++)
                    connections.Add(new Connection(from, to, LayerType.Hidden, LayerType.Output));

            return connections.ToArray();
        }

        private void CreateNetworkTopology(Connection[] connections)
        {
            this._size = connections.Length;

            for (int i = 0; i < this._inputLayer.Length; i++)
                this._inputLayer[i] = new Neuron(i, LayerType.Input);

            for (int i = 0; i < this._hiddenLayer.Length; i++)
                this._hiddenLayer[i] = new Neuron(i, LayerType.Hidden);

            for (int i = 0; i < this._outputLayer.Length; i++)
                this._outputLayer[i] = new Neuron(i, LayerType.Output);

            
            Connection connection;
            for(int i=0; i<connections.Length;i++)
            {
                connection=connections[i];
                if (connection.FromLayerType == LayerType.Input)
                {
                    if (connection.ToLayerType == LayerType.Hidden)                    
                        this._inputLayer[connection.From].ConnectTo(this._hiddenLayer[connection.To],connection.Weight);
                    
                    else if (connection.ToLayerType == LayerType.Output)                    
                        this._inputLayer[connection.From].ConnectTo(this._outputLayer[connection.To],connection.Weight);
    
                }
                else if (connection.FromLayerType == LayerType.Hidden)
                {
                    if (connection.ToLayerType == LayerType.Hidden)                    
                        this._hiddenLayer[connection.From].ConnectTo(this._hiddenLayer[connection.To],connection.Weight);
                        
                    
                    else if (connection.ToLayerType == LayerType.Output)                    
                        this._hiddenLayer[connection.From].ConnectTo(this._outputLayer[connection.To],connection.Weight );
         
                }

            }

        }

        public void TrainNetwork(Dataset trainingSet, ILearningMethod LearningMethod)
        {
            LearningMethod.TrainNetwork(this,trainingSet);
        }

        public Prediction Classify(DataMining.Data.Example example)
        {
      

            for (int i = 0; i < this._inputLayer.Length; i++)            
                this._inputLayer[i].Output = example[i];                
                      
            for (int i = 0; i < this._hiddenLayer.Length; i++)              
                SetOutput(this._hiddenLayer[i]);
            
            for (int i = 0; i < this._outputLayer.Length; i++)         
                SetOutput(this._outputLayer[i]); ;


            int maxLabel=0;
   
            double [] scores=new double[_outputLayer.Length];

            for (int i = 0; i < this._outputLayer.Length; i++)
            {
                scores[i] = this._outputLayer[i].Output;
                
                if (scores[i] > scores[maxLabel])
                    maxLabel = i;
            }


            Prediction prediction = new Prediction(maxLabel, scores);
            return prediction;

        }
             
        private void SetOutput(Neuron neuron)
        {
            double sum = neuron.Bias;
            double value = 0;

            for (int i = 0; i < neuron.From.Count; i++)            
            {
        
                value=neuron.From[i].Output * neuron.Weights[i];

                if (!double.IsNaN(value))
                    sum += value;
                else
                    throw new Exception("neuron.From[i].Output * neuron.Weights[i]=NaN!!!");
                
                
            }

            neuron.Output = this._activationFunction.Activate(sum);
            
 
        }


        #endregion


        
    }
}
