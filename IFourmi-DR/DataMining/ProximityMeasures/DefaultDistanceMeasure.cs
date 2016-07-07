using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data;

namespace iFourmi.DataMining.ProximityMeasures
{
    public class DefaultDistanceMeasure:IDistanceMeasure
    {
        private int _order=1;

        public DefaultDistanceMeasure(int order)
        {
            this._order = order;
        }

        public double CalculateDistance(Data.Instance instance1, Data.Instance instance2)
        {
            double distance = 0;
            int attributeCount = instance1.Values.Length; ;

            for (int attributeIndex = 0; attributeIndex < instance1.Values.Length; attributeIndex++)
            {
                if (instance1.Metadata.Attributes[attributeIndex] is NominalAttribute)
                    distance += (instance1[attributeIndex] == instance2[attributeIndex]) ? 0 : 1;
                else
                    distance += Math.Pow(instance1[attributeIndex] - instance2[attributeIndex],this._order);
            }

            distance = Math.Pow(distance, 1.0 / (double)this._order);

            //double maxDistance = Math.Pow(attributeCount, 1.0 / (double)this._order);
            //double similarity = (maxDistance - distance) / distance;
            //return similarity;

            return distance;
        }

        public double CalculateDistance(Data.Instance instance1, Data.Instance instance2, double[] weights)
        {
            double distance = 0;
            int attributeCount = instance1.Values.Length; ;


            for (int attributeIndex = 0; attributeIndex < instance1.Values.Length; attributeIndex++)
            {
                if (instance1.Metadata.Attributes[attributeIndex] is NominalAttribute)
                    distance += (instance1[attributeIndex] == instance2[attributeIndex]) ? 0 : weights[attributeIndex];
                else
                    distance += Math.Pow((instance1[attributeIndex] - instance2[attributeIndex]), this._order) * weights[attributeIndex];
            }

            distance = Math.Pow(distance, 1.0 / (double)this._order); 
            //double maxDistance = Math.Pow(attributeCount, 1.0 / (double)this._order);
            //double similarity = (maxDistance - distance) / distance;
            //return similarity;

            return distance;
        }

        public double CalculateDistance(Data.Instance instance1, Data.Instance instance2, double[][] weights)
        {
            double distance = 0;
            int classIndex = instance2.Label;
            int attributeCount=instance1.Values.Length;;

            for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
            {
                if (instance1.Metadata.Attributes[attributeIndex] is NominalAttribute)
                    distance += (instance1[attributeIndex] == instance2[attributeIndex]) ? 0 : weights[classIndex][attributeIndex];
                else
                    distance += Math.Pow((instance1[attributeIndex] - instance2[attributeIndex]), this._order) * weights[classIndex][attributeIndex];
            }

            distance = Math.Pow(distance, 1.0 / (double)this._order);

            //double maxDistance = Math.Pow(attributeCount, 1.0 / (double)this._order);
            //double similarity = (maxDistance - distance) / maxDistance;
            //return similarity;

            //distance = Math.Pow(distance, 1.0 / (double)this._order) / attributeCount;
            //double similarity = 1.0 / (1.0 + distance);
            //return similarity;

            return distance;
        }
    }
}
