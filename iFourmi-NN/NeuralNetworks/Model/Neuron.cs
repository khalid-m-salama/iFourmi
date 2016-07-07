using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.NeuralNetworks.ActivationFunctions;
using iFourmi.DataMining.Utilities;

namespace iFourmi.NeuralNetworks.Model
{
    public class Neuron
    {
        #region Data Members

        private int index;
        private List<Neuron> _from;
        private List<Neuron> _to;
        private List<int> _myIndexInTo;
        private List<double> _weights;
        private double _output;
        private double _bias;
        private LayerType _layer;
        private double _error;
        
        #endregion
        
        #region Properies

        public int Index
        {
            get { return this.index; }
        }

        public List<Neuron> From
        {
            get { return this._from; }
        }

        public List<Neuron> To
        {
            get { return this._to; }
        }

        public List<double> Weights
        {
            get { return this._weights; }
        }
        

        internal double Bias
        {
            get { return this._bias; }
            set { this._bias = value; }
        }

        public LayerType Layer
        {
            get  { return this._layer; }
        }

        internal double Output
        {
            get { return this._output; }
            set { this._output = value; }
        }

        internal double Error
        {
            get { return this._error; }
            set { this._error = value; }
        }

        #endregion

        #region Constructor

        public Neuron(int index, LayerType layer, double bias)
        {
            this.index = index;
            this._layer = layer;
            this._bias = bias;


            if (this._layer != LayerType.Input)
            {
                this._from = new List<Neuron>();
                this._weights = new List<double>();
            }

            if (this._layer != LayerType.Output)
            {
                this._to = new List<Neuron>();
                this._myIndexInTo = new List<int>();
            }

  
        }

        public Neuron(int index, LayerType layer)
            :this(index,layer, RandomUtility.GetNextDouble(-1,1)){}
        

        #endregion


        #region Methods

        public void ConnectTo(Neuron to, double weight)
        {
            this._to.Add(to);
            to._from.Add(this);            
            to._weights.Add(weight);
            this._myIndexInTo.Add(to._from.Count - 1);
            
        }


        public int GetWeightIndex(int to)
        {
            return this._myIndexInTo[to];
        }

        #endregion
    }
}
