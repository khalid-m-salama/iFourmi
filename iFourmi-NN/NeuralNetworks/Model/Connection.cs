using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.NeuralNetworks.Model;

namespace iFourmi.NeuralNetworks.Model
{
    public struct Connection
    {
        public int From;     

        public LayerType FromLayerType;
        
        public int To;
     
        public LayerType ToLayerType;

        public double Weight;
   

        public Connection(int from, int to, LayerType fromLayerType, LayerType toLayerType, double weight)            
        {
            this.From = from;
            this.To = to;
            this.FromLayerType = fromLayerType;
            this.ToLayerType = toLayerType;
            this.Weight = weight;
        }

        public Connection(int from, int to, LayerType fromLayerType, LayerType toLayerType)
            :this(from,to,fromLayerType,toLayerType,DataMining.Utilities.RandomUtility.GetNextDouble(-1, 1)){}
        
        public override string ToString()
        {
            return FromLayerType.ToString() + ":" + From.ToString() + "--(" + Math.Round(Weight, 3).ToString()+ ")-->" + ToLayerType.ToString() + ":" + To.ToString();
        }

    }
}
