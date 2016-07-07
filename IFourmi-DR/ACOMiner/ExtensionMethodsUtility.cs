using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iFourmi.ACO;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining;
using iFourmi.DataMining.Algorithms;
using iFourmi.DataMining.ClassificationMeasures;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;


namespace iFourmi.ACOMiner
{
    public static class ExtensionMethodsUtility
    {
        public static int[] InstancesToRemove(this Solution<DRComponent> solution)
        {
            List<int> list = new List<int>();
            foreach (DecisionComponent<DRComponent> component in solution.Components)
                if (component.Element.ElementType == DatasetElementType.Instance)
                    if (!component.Element.Include)
                        list.Add(component.Element.ElemetIndex);

            return list.ToArray();

        }

        public static int[] AttributesToRemove(this Solution<DRComponent> solution)
        {
            List<int> list = new List<int>();
            foreach (DecisionComponent<DRComponent> component in solution.Components)
                if (component.Element.ElementType == DatasetElementType.Attribute)
                    if (!component.Element.Include)
                        list.Add(component.Element.ElemetIndex);

            return list.ToArray();

        }

        public static int InstanceCount(this Solution<DRComponent> solution)
        {
            int count = 0;
            foreach (DecisionComponent<DRComponent> component in solution.Components)
                if (component.Element.ElementType == DatasetElementType.Instance)
                    if (component.Element.Include)
                        count++;
                 
            return count;
        }

        public static int FeatureCount(this Solution<DRComponent> solution)
        {
            int count = 0;
            foreach (DecisionComponent<DRComponent> component in solution.Components)
                if (component.Element.ElementType == DatasetElementType.Attribute)
                    if (component.Element.Include)
                        count++;

            return count;
        }


        public static Solution<DRComponent> Merge(Solution<DRComponent> solution1, Solution<DRComponent> solution2)
        {
            Solution<DRComponent> solution = new Solution<DRComponent>();
            solution.Components.AddRange(solution1.Components);
            solution.Components.AddRange(solution2.Components);
            return solution;
            
        }

    }
}
