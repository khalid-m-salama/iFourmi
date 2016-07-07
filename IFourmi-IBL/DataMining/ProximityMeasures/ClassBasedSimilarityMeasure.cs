using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.ProximityMeasures
{
    public class ClassBasedSimilarityMeasure:ISimilarityMeasure
    {
        private Data.Dataset _dataset;
        private List<double> _similarities;

        public ClassBasedSimilarityMeasure(Data.Dataset dataset)
        {
            this._dataset = dataset;
            this.CalculateBaseSimilarities();
        }

        private void CalculateBaseSimilarities()
        {
            this._similarities = new List<double>();
            double inputMin=double.MaxValue;
            double inputMax = 1;


            for (int attributeIndex = 0; attributeIndex < this._dataset.Metadata.Attributes.Length; attributeIndex++)
            {
                for (int value1Index = 0; value1Index < this._dataset.Metadata.Attributes[attributeIndex].Values.Length-1; value1Index++)
                {
                    for (int value2Index = value1Index + 1; value2Index < this._dataset.Metadata.Attributes[attributeIndex].Values.Length; value2Index++)
                    {

                        double value1Freq = this._dataset.Metadata.Attributes[attributeIndex].ValueCounts[value1Index];
                        double value2Freq = this._dataset.Metadata.Attributes[attributeIndex].ValueCounts[value2Index];

                        double value1ClassFreq = 0;
                        double value2ClassFreq = 0;

                        double similarity = 0;

                        for (int ClassValueIndex = 0; ClassValueIndex < this._dataset.Metadata.Target.Values.Length; ClassValueIndex++)
                        {
                            value1ClassFreq = this._dataset.Filter(attributeIndex, value1Index, ClassValueIndex).Count;
                            value1ClassFreq /= value1Freq;

                            value2ClassFreq = this._dataset.Filter(attributeIndex, value2Index, ClassValueIndex).Count;
                            value2ClassFreq /= value2Freq;

                            double value = 0 - Math.Abs(value1ClassFreq - value2ClassFreq);
                            similarity += value;
                        }
                        
                        similarity/= this._dataset.Metadata.Target.Values.Length;

                        if(similarity<inputMin)
                            inputMin=similarity;
                        else if (similarity > inputMax)
                            inputMax = similarity;
                        this._similarities.Add(similarity);
                    }

                }
            }

            for (int i = 0; i < this._similarities.Count; i++)
                this._similarities[i] = Stretch(this._similarities[i], inputMin, inputMax , 0, 1);

        }
        

        public double CalculateSimilarity(Data.Example example1, Data.Example example2)
        {
            double similarity = 0;
            int seek = 0;

            for (int attributeIndex = 0; attributeIndex < this._dataset.Metadata.Attributes.Length; attributeIndex++)
            {
                int value1Index = example1[attributeIndex];
                int value2Index = example2[attributeIndex];

                if (value1Index == value2Index)
                    similarity += 1;
                else
                {
                    if (value1Index > value2Index)
                    {
                        int temp = value1Index;
                        value1Index = value2Index;
                        value2Index = temp;
                    }

                    int index = seek + GetAttributeValueSeek(attributeIndex, value1Index) + value2Index - value1Index-1;
                    similarity += this._similarities[index];
                }


                seek += GetAttributeSeek(attributeIndex);
            }

            similarity /= this._dataset.Metadata.Attributes.Length;
            return similarity;
        }

        public int GetLength(int value)
        {
            int lenght=0;
            for (int i = 1; i < value; i++)
                lenght += i;

            return lenght;
        }

        public int GetAttributeSeek(int attributeIndex)
        {
            int seek = 0;
            for (int i = 1; i < this._dataset.Metadata.Attributes[attributeIndex].Values.Length; i++)
                seek += i;

            return seek;
        }

        public int GetAttributeValueSeek(int attributeIndex, int valueIndex)
        {
            int seek = 0;
            int counter = 0;
            for (int i = this._dataset.Metadata.Attributes[attributeIndex].Values.Length - 1;  i > 0; i--)
            {
                if (counter == valueIndex)
                    break;

                seek += i;
                counter++;
            }

            return seek;
        }

        public double Stretch(double value, double inputMin, double inputMax, double newMin, double newMax)
        {
            return (value - inputMin) * ((newMax - newMin) / (inputMax - inputMin)) + newMin;
                    
        }
    }
}
