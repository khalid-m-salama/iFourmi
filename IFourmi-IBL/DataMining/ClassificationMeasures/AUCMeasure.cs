using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Statistics;
using iFourmi.DataMining.Data;
using iFourmi.DataMining.Model;

namespace iFourmi.DataMining.ClassificationMeasures
{
    public class AUCMeasure : IClassificationMeasure
    {
        private CurveBuilder _curveBuilder;
        private AUCMode _mode;

        public AUCMeasure(CurveBuilder builder, AUCMode mode)
        {
            this._curveBuilder = builder;
            this._mode = mode;
        }

        public double CalculateMeasure(ConfusionMatrix[] list)
        {
            throw new NotImplementedException();
        }

        public double CalculateMeasure(Model.IClassifier classifier, Data.Dataset dataset)
        {
            double measure = 0;

            switch (this._mode)
            {
                case AUCMode.Pooled:
                    {
                        measure = this.CalculatePooledAUC(classifier, dataset);
                        break;
                    }

                case AUCMode.Averaged:
                    {
                        measure = this.CalculateAverageAUC(classifier, dataset);
                        break;
                    }

                case AUCMode.Wegithed:
                    {
                        measure = this.CalculateWeightedAUC(classifier, dataset);
                        break;
                    }
            }

            return measure;


        }

        private double CalculatePooledAUC(IClassifier classifier, Dataset dataset)
        {
             double final = 0;

            int labelSize = dataset.Metadata.Target.Values.Length;
            double[][] values = new double[labelSize][];
            int index = 0;

            for (int exampleIndex = 0; exampleIndex < dataset.Size; exampleIndex++)
            {
                values[index] = new double[2];

                Example example = dataset[exampleIndex];
                bool[] actualFlags = example.LabelFlags;
                Prediction prediction = classifier.Classify(example);


                for (int classIndex = 1; classIndex < labelSize; classIndex++)
                {
                    values[index][0] = prediction.Probabilities[classIndex];
                    values[index][1] = actualFlags[classIndex] ? 1 : 0;
					index++;
				}
			}

            return final;
		
        }

        private double CalculateWeightedAUC(IClassifier classifier, Dataset dataset)
        {
            double final = 0;

            int labelSize = dataset.Metadata.Target.Values.Length;
            double[][] values = new double[dataset.Size][];
            double[] areas = new double[labelSize];
            double[] frequency = new double[labelSize];
            double weight = 1.0 / (double)labelSize;
            double total = 0;

            for (int classIndex = 0; classIndex < labelSize; classIndex++)
            {
                int index = 0;
                double positives = 0;

                for (int exampleIndex = 0; exampleIndex < dataset.Size; exampleIndex++)
                {
                    values[index] = new double[2];

                    Example example = dataset[exampleIndex];

                    bool[] actualFlags = example.LabelFlags;
                    Prediction prediction = classifier.Classify(example);

                    values[index][0] = prediction.Probabilities[classIndex];
                    values[index][1] = actualFlags[classIndex] ? 1 : 0;
                    positives += values[index][1];
                    index++;
                }

                Curve curve = this._curveBuilder.CreateCurve(values);
                areas[classIndex] = curve.CalculateArea();
                frequency[classIndex] = positives;
                total += positives;

            }


            for (int classIndex = 0; classIndex < labelSize; classIndex++)
                final += ((frequency[classIndex] / total) * areas[classIndex]);

            return final;

        }

        private double CalculateAverageAUC(IClassifier classifier, Dataset dataset)
        {
            double final = 0;

            int labelSize = dataset.Metadata.Target.Values.Length;
            double[][] values = new double[dataset.Size][];
            double area = 0;
            double weight = 1.0 / (double)labelSize - 1;

            for (int classIndex = 0; classIndex < labelSize; classIndex++)
            {
                int index = 0;


                for (int exampleIndex = 0; exampleIndex < dataset.Size; exampleIndex++)
                {
                    values[index] = new double[2];

                    Example example = dataset[exampleIndex];

                    bool[] actualFlags = example.LabelFlags;
                    Prediction prediction = classifier.Classify(example);

                    values[index][0] = prediction.Probabilities[classIndex];
                    values[index][1] = actualFlags[classIndex] ? 1 : 0;
                    index++;
                }

                Curve curve = this._curveBuilder.CreateCurve(values);
                area += weight * curve.CalculateArea();


            }

            return final;

        }
    }

    public enum AUCMode
    {
        Averaged,
        Pooled,
        Wegithed

    }
}
