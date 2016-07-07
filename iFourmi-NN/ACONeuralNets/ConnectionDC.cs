using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.NeuralNetworks.Model;

namespace iFourmi.ACONeuralNets
{
    public class ConnectionDC
    {
        public bool Include;
        public Connection Connection;

        public ConnectionDC(int from, int to, LayerType fromLayerType, LayerType toLayerType, bool include)            
        {
            this.Connection = new Connection(from, to, fromLayerType, toLayerType);
            this.Include = include; 
        }
  
        public override string ToString()
        {
            return Connection.ToString() + "|" + this.Include.ToString();
        }

    }
}
