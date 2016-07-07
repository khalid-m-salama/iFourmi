using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class Dataset : IEnumerable<Example>
    {
        #region Data Members

        private Metadata _metadata;
        private Example[] _examples;
        private int _size;

        #endregion

        #region Properties

        public Metadata Metadata
        {
            get { return this._metadata; }
        }

        public Example this[int index]
        {
            get
            {

                return this._examples[index];
            }
        }

        public int Size
        {
            get { return this._size; }
        }

        #endregion

        #region Constructors

        public Dataset(Metadata metadata)
        {
            this._metadata = metadata;
        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<Example> GetEnumerator()
        {
            foreach (Example example in this._examples)
                yield return example;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._examples.GetEnumerator();
        }

        #endregion

        #region Methods

        internal void SetExamples(Example[] examples)
        {
            this._examples = examples;

            this._size = examples.Length;
            this._metadata.Size = this._size;
            this.Normalize();

        }


        private void Standardize()
        {
            int attributeCount = this.Metadata.Attributes.Length;

            double[] max = new double[attributeCount];
            double[] min = new double[attributeCount];
            double[] sums = new double[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                max[i] = double.MinValue;
                min[i] = double.MaxValue;
            }


            foreach (Example example in this)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    if (!Double.IsNaN(example[attributeIndex]))
                    {
                        if (example[attributeIndex] > max[attributeIndex])
                            max[attributeIndex] = example[attributeIndex];
                        else if (example[attributeIndex] < min[attributeIndex])
                            min[attributeIndex] = example[attributeIndex];
                        sums[attributeIndex] += example[attributeIndex];
                    }

                }
            }

            for (int exampleIndex = 0; exampleIndex < this._size; exampleIndex++)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    double v1 = (max[attributeIndex] + min[attributeIndex]) / 2;
                    double v2 = (max[attributeIndex] - min[attributeIndex]) / 2;
                    double mean = sums[attributeIndex] / this._size;
                    if (Double.IsNaN(this._examples[exampleIndex][attributeIndex]))
                        this._examples[exampleIndex][attributeIndex] = v2 == 0 ? 0 : (mean - v1) / v2;
                    else
                        this._examples[exampleIndex][attributeIndex] = v2 == 0 ? 0 : (this[exampleIndex][attributeIndex] - v1) / v2;
                }
            }

        }

        private void Normalize()
        {
            int attributeCount = this.Metadata.Attributes.Length;

            double[] max = new double[attributeCount];
            double[] min = new double[attributeCount];
            double[] sums = new double[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                max[i] = double.MinValue;
                min[i] = double.MaxValue;
            }


            foreach (Example example in this)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    if (!Double.IsNaN(example[attributeIndex]))
                    {
                        if (example[attributeIndex] > max[attributeIndex])
                            max[attributeIndex] = example[attributeIndex];
                        else if (example[attributeIndex] < min[attributeIndex])
                            min[attributeIndex] = example[attributeIndex];
                        sums[attributeIndex] += example[attributeIndex];
                    }

                }
            }

            for (int exampleIndex = 0; exampleIndex < this._size; exampleIndex++)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    double minValue = min[attributeIndex];
                    double maxValue = max[attributeIndex];
                    double currentValue = this._examples[exampleIndex][attributeIndex];

                    double mean = sums[attributeIndex] / this._size;
                    if (Double.IsNaN(currentValue))
                        this._examples[exampleIndex][attributeIndex] = mean;
                    else
                        this._examples[exampleIndex][attributeIndex] = (currentValue - minValue) / (maxValue - minValue);
                }
            }

        }
        
        public List<int> Filter(int target)
        {
            List<int> result = new List<int>();
            foreach (Example example in this)
            {
                if (example.Label == target)
                    result.Add(example.Index);
            }

            return result;
        }

        public Dataset[] SplitByClassValues()
        {
            Dataset[] datasets = new Dataset[this._metadata.Target.Length];

            for (int classIndex = 0; classIndex < this._metadata.Target.Length; classIndex++)
            {
                datasets[classIndex] = new Dataset(this._metadata.Clone());
                Example[] examples = this.GetExamples(this.Filter(classIndex));
                datasets[classIndex].SetExamples(examples);
            }

            return datasets;
        }

        private Example[] GetExamples(List<int> indexes)
        {
            List<Example> examples = new List<Example>();
            foreach (int index in indexes)
                examples.Add(this[index]);
            return examples.ToArray();

        }

        private Example[] RemoveExamples(List<int> indexestoRemove)
        {
            List<Example> examples = new List<Example>();
            foreach (Example example in this._examples)
                if (!indexestoRemove.Contains(example.Index))
                    examples.Add(example);
            return examples.ToArray();

        }

        public Dataset[] Startify(int folds)
        {
            Dataset[] results = new Dataset[folds];
            List<Tuple<int, string>> classes = new List<Tuple<int, string>>();


            {
                int index = 0;
                foreach (string classValue in this.Metadata.Target)
                {
                    classes.Add(new Tuple<int, string>(index, classValue));
                    index++;
                }

            }


            for (int index = 0; index < results.Length; index++)
                results[index] = new Dataset(this.Metadata.Clone());

            List<Example>[] exampleLists = new List<Example>[folds];
            for (int index = 0; index < folds; index++)
                exampleLists[index] = new List<Example>();


            foreach (Tuple<int, string> classTuple in classes)
            {
                int foldPointer = 0;

                foreach (Example example in this.GetExamples(this.Filter(classTuple.Item1)))
                {
                    exampleLists[foldPointer].Add((Example)example.Clone());
                    foldPointer++;
                    if (foldPointer == folds)
                        foldPointer = 0;
                }

            }

            for (int index = 0; index < folds; index++)
                results[index].SetExamples(exampleLists[index].ToArray());

            return results;




        }

        public Dataset[] SplitRandomly(double ratio)
        {
            Random r = new Random();

            Dataset[] datasets = new Dataset[2];

            datasets[0] = new Dataset(this._metadata.Clone());
            datasets[1] = new Dataset(this._metadata.Clone());

            List<Example> list1 = new List<Example>();
            List<Example> list2 = new List<Example>();

            for (int i = 0; i < this._metadata.Size; i++)
            {
                //double p = r.NextDouble();
                //if (p < ratio)
                //    list1.Add(this[i].Clone() as Example);
                //else
                //    list2.Add(this[i].Clone() as Example);

                double p = r.NextDouble();
                if (p < ratio)
                    list1.Add(this[i]);
                else
                    list2.Add(this[i]);

            }

            datasets[0].SetExamples(list1.ToArray());
            datasets[1].SetExamples(list2.ToArray());

            return datasets;
        }

        public override string ToString()
        {
            return this._metadata.DatasetName;
        }

        #endregion


    }
}
