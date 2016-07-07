using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ProximityMeasures
{
    public class BinaryMatchingSimilarityMeasure:ISimilarityMeasure
    {

        public double CalculateSimilarity(Data.Example example1, Data.Example example2)
        {
            double similarity = 0;

            for (int attributeIndex = 0; attributeIndex < example1.Values.Length; attributeIndex++)
                similarity += (example1[attributeIndex] == example2[attributeIndex]) ? 1 : 0;

            similarity /= example1.Metadata.Attributes.Length;

            return similarity;
        }
    }
}
