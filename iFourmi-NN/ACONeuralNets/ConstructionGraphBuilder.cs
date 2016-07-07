using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;


namespace iFourmi.ACONeuralNets
{
    public static class ConstructionGraphBuilder
    {
        public static ConstructionGraph<ConnectionDC> BuildNNConstructionGraph(Metadata metadata,int hiddenUnits)
        {
            int counter = 0;

            int inputUnitCount = metadata.Attributes.Length;
            int outputUnitCount = metadata.Target.Length;
            
            List<DecisionComponent<ConnectionDC>> components = new List<DecisionComponent<ConnectionDC>>();

            //available connections between input units and hidden units.
            for (int from = 0; from < inputUnitCount; from++)
            {
                for (int to = 0; to < hiddenUnits; to++)
                {
                    ConnectionDC connector1 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Input, NeuralNetworks.Model.LayerType.Hidden, true);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector1));

                    ConnectionDC connector2 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Input, NeuralNetworks.Model.LayerType.Hidden, false);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector2));
                }
            }

            //available connections between input units and output units.
            for (int from = 0; from < inputUnitCount; from++)
            {
                for (int to = 0; to < outputUnitCount; to++)
                {
                    ConnectionDC connector1 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Input, NeuralNetworks.Model.LayerType.Output, true);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector1));

                    ConnectionDC connector2 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Input, NeuralNetworks.Model.LayerType.Output, false);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector2));
                }
            }

            //available connections between hidden units and output units.
            for (int from = 0; from < hiddenUnits; from++)
            {
                for (int to = 0; to < outputUnitCount; to++)
                {
                    ConnectionDC connector1 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Hidden, NeuralNetworks.Model.LayerType.Output, true);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector1));

                    ConnectionDC connector2 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Hidden, NeuralNetworks.Model.LayerType.Output, false);
                    components.Add(new DecisionComponent<ConnectionDC>(counter++, connector2));
                }
            }

            //available connections between hidden units each other.
            for (int from = 0; from < hiddenUnits; from++)
            {
                for (int to = from + 1; to < hiddenUnits; to++)
                {  
                        ConnectionDC connector = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Hidden, NeuralNetworks.Model.LayerType.Hidden, true);
                        components.Add(new DecisionComponent<ConnectionDC>(counter++, connector));

                        ConnectionDC connector2 = new ConnectionDC(from, to, NeuralNetworks.Model.LayerType.Hidden, NeuralNetworks.Model.LayerType.Hidden, false);
                        components.Add(new DecisionComponent<ConnectionDC>(counter++, connector2));                    
                }
            }

            return new ConstructionGraph<ConnectionDC>(components.ToArray());

        }
    }
}
