using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO;
using iFourmi.DataMining.Data;




namespace iFourmi.ACOMiner
{
    class ConstructionGraphBuilder
    {
        public static ConstructionGraph<DRComponent> BuildDRConstructionGraph(Metadata metadata, bool useAttributes, bool useInstances)
        {
            int counter = 0;
            List<DecisionComponent<DRComponent>> components = new List<DecisionComponent<DRComponent>>();

            if (useAttributes)
            {
                foreach (DataMining.Data.Attribute attribute in metadata.Attributes)
                {
                    components.Add(new DecisionComponent<DRComponent>(counter++, new DRComponent(DatasetElementType.Attribute, attribute.Index, true)));
                    components.Add(new DecisionComponent<DRComponent>(counter++, new DRComponent(DatasetElementType.Attribute, attribute.Index, false)));
                }
            }


            if (useInstances)
            {
                for (int i = 0; i < metadata.Size; i++)
                {
                    components.Add(new DecisionComponent<DRComponent>(counter++, new DRComponent(DatasetElementType.Instance, i, true)));
                    components.Add(new DecisionComponent<DRComponent>(counter++, new DRComponent(DatasetElementType.Instance, i, false)));
                }
            }

            return new ConstructionGraph<DRComponent>(components.ToArray());

        }

    }
}
