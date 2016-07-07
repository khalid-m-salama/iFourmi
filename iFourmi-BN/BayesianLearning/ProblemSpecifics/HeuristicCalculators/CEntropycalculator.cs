using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.ACO.ProblemSpecifics;
using iFourmi.DataMining.Data;
using iFourmi.BayesianNetworks.Model;


namespace iFourmi.BayesianLearning.ProblemSpecifics.HeuristicCalculators
{
    public class CEntropyCalculator : IHeuristicValueCalculator<Edge>
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

        #region Constructor

        public CEntropyCalculator(Dataset dataset)
        {
            this._dataset = dataset;
        }

        #endregion

        #region Mehods

        public void CalculateHieristicValue(ACO.DecisionComponent<Edge> Element)
        {
            iFourmi.DataMining.Data.Attribute xAttribute = this._dataset.Metadata.Attributes[Element.Element.ParentIndex];
            iFourmi.DataMining.Data.Attribute yAttribute = this._dataset.Metadata.Attributes[Element.Element.ChildIndex];
            iFourmi.DataMining.Data.Attribute zAttribute = this._dataset.Metadata.Target;

            double InformationGain = 0;
            
            double size = this._dataset.Size;

            double xy_z = 0;
            double x_z = 0;
            double y_z = 0;

            double xyz = 0;
            double xz = 0;
            double yz = 0;
            double z = 0;


            List<int> attributeIndexes3 = new List<int>();
            attributeIndexes3.Add(xAttribute.Index);
            attributeIndexes3.Add(yAttribute.Index);
            attributeIndexes3.Add(zAttribute.Index);

            List<int> valueIndexes3 = new List<int>();
            valueIndexes3.Add(0);
            valueIndexes3.Add(0);
            valueIndexes3.Add(0);



            List<int> attributeIndexes2 = new List<int>();
            attributeIndexes2.Add(xAttribute.Index);
            attributeIndexes2.Add(zAttribute.Index);

            List<int> valueIndexes2 = new List<int>();
            valueIndexes2.Add(0);
            valueIndexes2.Add(0);


            for (int zi = 0; zi < zAttribute.Values.Length; zi++)
            {
                double baseEntropy = 0;
                double entropy = 0;
                double _information = 0;
                z = zAttribute.ValueCounts[zi];
                
                                
                for (int yi = 0; yi < yAttribute.Values.Length; yi++)
                {
                    attributeIndexes2[0] = yAttribute.Index;
                    attributeIndexes2[1] = zAttribute.Index;

                    valueIndexes2[0] = yi;
                    valueIndexes2[1] = zi;

                    yz = this._dataset.Filter(attributeIndexes2, valueIndexes2).Count;
                    y_z = yz/z;
                    

                    //baseEntropy -= (y_z) * Math.Log(y_z);

                    baseEntropy -= (yz/size) * Math.Log(yz/size);

                    if (double.IsNaN(baseEntropy) || double.IsInfinity(baseEntropy))
                        baseEntropy = 1;
                }


                for (int xi = 0; xi < xAttribute.Values.Length; xi++)
                {
                    double x = xAttribute.ValueCounts[xi];

                    for (int yi = 0; yi < yAttribute.Values.Length; yi++)
                    {
                        attributeIndexes2[0] = xAttribute.Index;
                        attributeIndexes2[1] = zAttribute.Index;

                        valueIndexes2[0] = xi;
                        valueIndexes2[1] = zi;

                        xz = this._dataset.Filter(attributeIndexes2, valueIndexes2).Count;
                        x_z = xz / z;

                        valueIndexes3[0] = xi;
                        valueIndexes3[1] = yi;
                        valueIndexes3[2] = zi;

                        xyz = this._dataset.Filter(attributeIndexes3, valueIndexes3).Count;
                        xy_z = xyz / z;

                        //double value = (xy_z / x_z) * Math.Log((xy_z / x_z));

                        double value = -(xyz / x) * Math.Log((xyz / x));

                        if (!double.IsNaN(value) && !double.IsInfinity(value))
                            entropy += (xyz / z) * value;

                    }
                }

                _information = baseEntropy - entropy;

                InformationGain += _information * (z/size);

            }


            Element.Heuristic = InformationGain;
        }

        #endregion
    }
}
