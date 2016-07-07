using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.NeuralNetworks.Model;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;
using iFourmi.DataMining.ClassificationMeasures;

namespace iFourmi.NeuralNetworks.LearningMethods
{
    public class BackPropagation : ILearningMethod
    {
        #region Data Members

        private double _learningRate;

        private int _epochCount;

        private double _positiveClassValue;

        private bool _earlyStop;

        private IClassificationMeasure _MSError;

        #endregion

        #region Properties

        public bool EarlyStop
        {
            get { return this._earlyStop; }
            set { this._earlyStop = value; }
        }

        #endregion
        
        #region Events

        public event EventHandler OnPostEpoch;

        #endregion
        
        #region Constructors

        public BackPropagation(double learningRate, int epochCount, double positiveClassValue, bool earlyStop)
        {   
            this._learningRate = learningRate;
            this._epochCount = epochCount;
            this._positiveClassValue = positiveClassValue;
            this._earlyStop = earlyStop;
            this._MSError = new MSError();
        }

        #endregion

        #region Methods

        public void TrainNetwork(NeuralNetwork network, Dataset trainingSet)
        {

            if (!_earlyStop)
            {
                for (int i = 0; i < _epochCount; i++)
                {
                    this.PerformEpoch(network, trainingSet);

                    if (OnPostEpoch != null)
                        OnPostEpoch(this, null);

                }
            }
            else
                this.TrainNetworkEarlyStop(network, trainingSet);
        }

        private void TrainNetworkEarlyStop(NeuralNetwork network, Dataset trainingSet)
        {
            double bestError = double.MaxValue;
            double currentError = 0;
            Dataset[] datasets = trainingSet.SplitRandomly(0.8);
            Dataset learningSet = datasets[0];
            Dataset validationSet = datasets[1];
            
            int convergence=0;

            for (int i = 0; i < _epochCount; i++)
            {
                this.PerformEpoch(network, learningSet);
                currentError = this._MSError.CalculateMeasure(network, validationSet);

                if (currentError < bestError)
                {
                    bestError = currentError;
                    convergence = 0;
                    
                }
                else
                {   
                    convergence++;
                    if (convergence == 1)
                        break;

                }
               

                if (OnPostEpoch != null)
                    OnPostEpoch(this, new BPEventArgs() {Quality=currentError });                
            }
        }

        private void UpdateLearningRate(bool improved)
        {
            if (improved)
                this._learningRate *= 1.05;
            else
                this._learningRate *= 0.5;
            
        }

        private void PerformEpoch(NeuralNetwork network, Dataset trainingSet)
        {
            double [] error = null;
            
            for(int i=0; i<trainingSet.Size; i++)
            {   
                error = CalculateError(network,trainingSet[i]);
                this.Propegate(network, error);
            }
                
        }

        private double [] CalculateError(NeuralNetwork network, Example pattern)
        {   
            Prediction prediction = network.Classify(pattern);
            double[] error = new double[prediction.Scores.Length];

            for (int i = 0; i < prediction.Scores.Length; i++) 
            {
                if (i == pattern.Label)
                    error[i] = this._positiveClassValue - prediction.Scores[i];
                else
                    error[i] = (1-this._positiveClassValue) - prediction.Scores[i];
            }
            return error;
        }

        private void Propegate(NeuralNetwork network, double[] error)
        {
  
            for (int i = 0; i < network.OutputLayer.Length; i++)            
                network.OutputLayer[i].Error = error[i];              
           
            for (int i = network.HiddenLayer.Length - 1; i >= 0; i-- )
                SetError(network.HiddenLayer[i], network.ActivationFunction);
           
            for (int i = network.InputLayer.Length - 1; i >= 0; i--)           
                SetError(network.InputLayer[i], network.ActivationFunction); ;
                         
        }


        private void SetError(Neuron neuron, ActivationFunctions.IActivationFunction activationFunction)
        {
            double error = 0;
            double dw=0;
            double eta = this._learningRate;
            int weightIndex = -1;

            for (int i = 0; i < neuron.To.Count; i++)
            { 
                weightIndex = neuron.GetWeightIndex(i);
                dw = eta * neuron.To[i].Error * activationFunction.Dervative(neuron.To[i].Output) * neuron.Output;
                double a, b, c;
                a = neuron.To[i].Error;
                b = activationFunction.Dervative(neuron.To[i].Output);
                c = neuron.To[i].Weights[weightIndex];
                error += a * b * c;
                neuron.To[i].Weights[weightIndex] += dw;

            }

            neuron.Error = error;
            neuron.Bias += eta * neuron.Error * activationFunction.Dervative(neuron.Output);
            
        }

        #endregion

        
    }

    public class BPEventArgs : EventArgs
    {
        public double Quality
        {
            get;
            set;
        }
    }
}
