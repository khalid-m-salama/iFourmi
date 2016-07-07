using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class Dataset : IEnumerable<Instance>
    {
        #region Data Members

        private Metadata _metadata;
        private Instance[] _instances;
        private int _size;

        #endregion

        #region Properties

        public Metadata Metadata
        {
            get { return this._metadata; }
        }

        public Instance this[int index]
        {
            get 
            {
              
                return this._instances[index];
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

        public IEnumerator<Instance> GetEnumerator()
        {
            foreach (Instance instance in this._instances)
                yield return instance;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._instances.GetEnumerator();
        }

        #endregion

        #region Methods

        internal void SetInstances(Instance[] instances)
        {
            this._instances = instances;

            this._size = instances.Length;
            this._metadata.Size = this._size;
            this.Normalize();
            this.UpdateMissingValues();
            this.UpdateValueCounts();
         
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


            foreach (Instance instance in this)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    if (!Double.IsNaN(instance[attributeIndex]))
                    {
                        if (instance[attributeIndex] > max[attributeIndex])
                            max[attributeIndex] = instance[attributeIndex];
                        else if (instance[attributeIndex] < min[attributeIndex])
                            min[attributeIndex] = instance[attributeIndex];
                        sums[attributeIndex] += instance[attributeIndex];
                    }

                }
            }

            for (int instanceIndex = 0; instanceIndex < this._size; instanceIndex++)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    double v1 = (max[attributeIndex] + min[attributeIndex]) / 2;
                    double v2 = (max[attributeIndex] - min[attributeIndex]) / 2;
                    double mean = sums[attributeIndex] / this._size;
                    if (Double.IsNaN(this._instances[instanceIndex][attributeIndex]))
                        this._instances[instanceIndex][attributeIndex] = v2 == 0 ? 0 : (mean - v1) / v2;
                    else
                        this._instances[instanceIndex][attributeIndex] = v2 == 0 ? 0 : (this[instanceIndex][attributeIndex] - v1) / v2;
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


            foreach (Instance instance in this)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    if (!Double.IsNaN(instance[attributeIndex]))
                    {
                        if (instance[attributeIndex] > max[attributeIndex])
                            max[attributeIndex] = instance[attributeIndex];
                        else if (instance[attributeIndex] < min[attributeIndex])
                            min[attributeIndex] = instance[attributeIndex];
                        sums[attributeIndex] += instance[attributeIndex];
                    }

                }
            }

            for (int instanceIndex = 0; instanceIndex < this._size; instanceIndex++)
            {
                for (int attributeIndex = 0; attributeIndex < attributeCount; attributeIndex++)
                {
                    double minValue = min[attributeIndex];
                    double maxValue = max[attributeIndex];
                    double currentValue = this._instances[instanceIndex][attributeIndex];

                    double mean = sums[attributeIndex] / this._size;
                    if (Double.IsNaN(currentValue))
                        this._instances[instanceIndex][attributeIndex] = mean;
                    else
                        this._instances[instanceIndex][attributeIndex] = (currentValue - minValue) / (maxValue - minValue);
                }
            }

        }
        

        private void UpdateMissingValues()
        {
            foreach (Instance examlpe in this)
            {
                for (int attributeIndex = 0; attributeIndex < this._metadata.Attributes.Length; attributeIndex++)
                {
                    if (this._metadata.Attributes[attributeIndex] is NominalAttribute)
                    {

                        if (examlpe[attributeIndex] == -1)
                        {
                            examlpe[attributeIndex] = GetMaxIndex(attributeIndex);
                        }
                    }
                }

            }
        }

        private int GetMaxIndex(int attributeIndex)
        {
            int maxIndex = 0;
            int maxCount = 0;

            for (int valueIndex = 0; valueIndex < ((NominalAttribute)this._metadata.Attributes[attributeIndex]).Values.Length; valueIndex++)
            {
                if (((NominalAttribute)this._metadata.Attributes[attributeIndex]).ValueCounts[valueIndex] > maxCount)
                {
                    maxCount = ((NominalAttribute)this._metadata.Attributes[attributeIndex]).ValueCounts[valueIndex];
                    maxIndex = valueIndex;
                }

            }

            return maxIndex;
        }

        private Instance[] RemoveExamples(List<int> indexestoRemove)
        {
            List<Instance> instances = new List<Instance>();
            foreach (Instance instance in this._instances)
                if (!indexestoRemove.Contains(instance.Index))
                    instances.Add(instance);
            return instances.ToArray();

        }

        private List<int> Filter(int attributeIndex, int valueIndex)
        {
            List<int> result = new List<int>();
            if (attributeIndex == this._metadata.Target.Index)
            {
                return this.Filter(valueIndex);
            }
            else
            {

                foreach (Instance instance in this)
                {
                    if (instance[attributeIndex] == valueIndex)
                        result.Add(instance.Index);
                }
            }

            return result;

        }

        public List<int> Filter(int attributeIndex, int valueIndex, int labelValueIndex)
        {
            List<int> result = new List<int>();
            foreach (Instance instance in this)
            {

                if (instance[attributeIndex] == valueIndex && instance.Labels.Contains(labelValueIndex))
                    result.Add(instance.Index);

            }


            return result;

        }

        private void UpdateValueCounts()
        {

            for (int attributeIndex = 0; attributeIndex < this._metadata.Attributes.Length; attributeIndex++)
            {
                if (this._metadata.Attributes[attributeIndex] is NominalAttribute)
                {

                    Data.NominalAttribute currentAttribute = this._metadata.Attributes[attributeIndex] as NominalAttribute;

                    for (int valueIndex = 0; valueIndex < currentAttribute.Values.Length; valueIndex++)
                        currentAttribute.ValueCounts[valueIndex] = this.Filter(attributeIndex, valueIndex).Count;

                    for (int valueIndex = 0; valueIndex < this._metadata.Target.Values.Length; valueIndex++)
                        this._metadata.Target.ValueCounts[valueIndex] = this.Filter(valueIndex).Count;

                }
            }

            this._metadata.Size = this._instances.Length;


        }


        public List<int> Filter(int target)
        {
            List<int> result = new List<int>();
            foreach (Instance instance in this)
            {
                    if (instance.Label==target)
                        result.Add(instance.Index);                
            }

            return result;
        }

        public Dataset[] SplitByClassValues()
        {
            Dataset[] datasets = new Dataset[this._metadata.Target.Values.Length];

            for (int classIndex = 0; classIndex < this._metadata.Target.Values.Length; classIndex++)
            {
                datasets[classIndex] = new Dataset(this._metadata.Clone());
                Instance[] instances = this.Getinstances(this.Filter(classIndex));
                datasets[classIndex].SetInstances(instances);
            }

            return datasets;
        }
        
        private Instance[] Getinstances(List<int> indexes)
        {
            List<Instance> instances = new List<Instance>();
            foreach (int index in indexes)
                instances.Add(this[index]);
            return instances.ToArray();

        }

        public Dataset[] Startify(int folds)
        {
            Dataset[] results = new Dataset[folds];
            List<Tuple<int, string>> classes = new List<Tuple<int, string>>();

    
            {
                int index = 0;
                foreach (string classValue in this.Metadata.Target.Values)
                {
                    classes.Add(new Tuple<int, string>(index, classValue)); 
                    index++;
                }
 
            }


            for (int index = 0; index < results.Length; index++)
                results[index] = new Dataset(this.Metadata.Clone());

            List<Instance> [] instanceLists = new List<Instance>[folds];
            for (int index = 0; index < folds; index++)
                instanceLists[index] = new List<Instance>();


            foreach (Tuple<int,string> classTuple in classes)
            {
                int foldPointer = 0;

                foreach(Instance instance in this.Getinstances(this.Filter(classTuple.Item1)))
                {
                    instanceLists[foldPointer].Add((Instance)instance.Clone());
                    foldPointer++;
                    if (foldPointer == folds)
                        foldPointer = 0;
                }
 
            }

            for (int index = 0; index < folds; index++)
                results[index].SetInstances(instanceLists[index].ToArray());

            return results;
            
            
            
 
        }

        public Dataset[] SplitRandomly(double ratio)
        {
            Random r = new Random();

            Dataset[] datasets = new Dataset[2];

            datasets[0] = new Dataset(this._metadata.Clone());
            datasets[1] = new Dataset(this._metadata.Clone());

            List<Instance> list1 = new List<Instance>();
            List<Instance> list2 = new List<Instance>();

            for (int i = 0; i < this._metadata.Size; i++)
            {
                //double p = r.NextDouble();
                //if (p < ratio)
                //    list1.Add(this[i].Clone() as instance);
                //else
                //    list2.Add(this[i].Clone() as instance);

                double p = r.NextDouble();
                if (p < ratio)
                    list1.Add(this[i]);
                else
                    list2.Add(this[i]);

            }

            datasets[0].SetInstances(list1.ToArray());
            datasets[1].SetInstances(list2.ToArray());

            return datasets;
        }

        public Dataset GetBinaryFlatLabelSubDataset(string positiveClassValue, string[] negativeClassValues)
        {
            string name = this._metadata.DatasetName;
            int classIndex = this._metadata.Target.GetIndex(positiveClassValue);


            DataMining.Data.Attribute[] attClone = new Attribute[this.Metadata.Attributes.Length];
            for (int attributeIndex = 0; attributeIndex < this.Metadata.Attributes.Length; attributeIndex++)
                attClone[attributeIndex] = this.Metadata.Attributes[attributeIndex].Clone();

            Data.NominalAttribute target = new NominalAttribute("class", this._metadata.Target.Index, new string[] { "Yes", "No" });
            DataMining.Data.Metadata metadata = new Data.Metadata(name, attClone, target,false);

            Dataset dsResult = new Dataset(metadata);


            int positiveClassIndex = this._metadata.Target.GetIndex(positiveClassValue);
            List<Instance> positive = new List<Instance>();

            int pcounter = 0;
            foreach (int instanceIndex in this.Filter(classIndex))
                positive.Add(new Instance(dsResult.Metadata, pcounter++, this[instanceIndex].Values, 0));

            List<Instance> negative = new List<Instance>();

            int ncounter = 0;
            foreach (string negativeClassValue in negativeClassValues)
            {
                int negativeClassIndex = this._metadata.Target.GetIndex(negativeClassValue);
                foreach (int instanceIndex in this.Filter(negativeClassIndex))
                {
                    if (!negative.Exists(e => e.Index == instanceIndex))
                        negative.Add(new Instance(dsResult.Metadata, ncounter++, this[instanceIndex].Values, 1));

                }
            }


            List<Instance> instances = new List<Instance>();
            instances.AddRange(positive);
            instances.AddRange(negative);
            dsResult.SetInstances(instances.ToArray());

            return dsResult;


        }

        public static Dataset Merge(Dataset trainingSet, Dataset testingSet)
        {
            List<Instance> instances = trainingSet._instances.ToList();
            instances.AddRange(testingSet._instances.ToList());


            Dataset merge = new Dataset(trainingSet.Metadata);
            merge.SetInstances(instances.ToArray());

            return merge;

        }

        public override string ToString()
        {
            return this._metadata.DatasetName;
        }


        public Dataset ReduceInstances(int[] instancesToRemove)
        {
            Metadata newMetadata = this._metadata.Clone();
            Dataset reducedDataset = new Dataset(newMetadata);
            List<Instance> reducedInstances = new List<Instance>();
            for (int instanceIndex = 0; instanceIndex < this._size; instanceIndex++)
                if (!instancesToRemove.Contains(instanceIndex))
                    reducedInstances.Add(this[instanceIndex]);

            reducedDataset.SetInstances(reducedInstances.ToArray());

            return reducedDataset;
        }

        public Dataset ReduceAttributes(int[] attributesToRemove)
        {
            List<Data.Attribute> reducedAttributes = new List<Attribute>();
            for (int attributeIndex = 0; attributeIndex < this._metadata.Attributes.Length; attributeIndex++)
                if (!attributesToRemove.Contains(attributeIndex))
                    reducedAttributes.Add(this._metadata.Attributes[attributeIndex]);


            Metadata newMetadata = new Metadata(this._metadata.DatasetName, reducedAttributes.ToArray(), this._metadata.Target, this._metadata.IsHierarchical);
            Dataset reducedDataset = new Dataset(newMetadata);
            List<Instance> newInstances = new List<Instance>();

            for (int instanceIndex = 0; instanceIndex < this._size; instanceIndex++)
            {
                Instance current = this[instanceIndex];

                List<double> values=new List<double>();
                for(int attributeIndex=0; attributeIndex<this._metadata.Attributes.Length; attributeIndex++)
                    if(!attributesToRemove.Contains(attributeIndex))
                        values.Add(current[attributeIndex]);

                Instance newInstance = new Instance(newMetadata, instanceIndex, values.ToArray(), current.Label);
                newInstances.Add(newInstance);
            }

            reducedDataset.SetInstances(newInstances.ToArray());
            return reducedDataset;

        }

        #endregion



      
    }
}
