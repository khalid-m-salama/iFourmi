using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.BayesianNetworks.Model;

namespace iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators
{
    public class MICalculator : IHeuristicValueCalculator<Edge>
    {
        #region Data Members

        private Dataset _dataset;

        #endregion

        #region Properties

        public DataMining.Data.Dataset Dataset
        {
            get { return this._dataset; }
            set { this._dataset = value; }
        }

        #endregion

     

        #region Mehods

        public void CalculateHieristicValue(ACO.DecisionComponent<Edge> Element)
        {
            iFourmi.DataMining.Data.Attribute xAttribute = this._dataset.Metadata.Attributes[Element.Element.ParentIndex];
            iFourmi.DataMining.Data.Attribute yAttribute = this._dataset.Metadata.Attributes[Element.Element.ChildIndex];
            //iFourmi.DataMining.Data.Attribute zAttribute = this._dataset.Metadata.Target;


            

            double MI = 0;

            double size = this._dataset.Size;

            double xy = 0;
            double x = 0;
            double y = 0;




            List<int> attributeIndexes2 = new List<int>();
            attributeIndexes2.Add(xAttribute.Index);
            attributeIndexes2.Add(yAttribute.Index);

            List<int> valueIndexes2 = new List<int>();
            valueIndexes2.Add(0);
            valueIndexes2.Add(0);
            
            

            for (int xi = 0; xi < xAttribute.Values.Length; xi++)
            {
                x = xAttribute.ValueCounts[xi];
                x = x / size;

                for (int yi = 0; yi < yAttribute.Values.Length; yi++)
                {
                    y = yAttribute.ValueCounts[yi];
                    y = y / size;

                    valueIndexes2[0] = xi;
                    valueIndexes2[1] = yi;
                    

                    xy = this._dataset.Filter(attributeIndexes2, valueIndexes2).Count;
                    xy = xy / size;

                    double value = xy * Math.Log(xy / (x * y));

                    if (!double.IsNaN(value) && !double.IsInfinity(value))
                        MI += value;

                }
            }




            if (MI <= 0)
                MI = 0;

            Element.Heuristic = MI;
        }

        #endregion
    }
}
