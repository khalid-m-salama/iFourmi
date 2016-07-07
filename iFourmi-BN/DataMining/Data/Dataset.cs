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
        private Dictionary<int,Example> _examples;
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

        public Example[] Examples
        {
            get { return this._examples.Values.ToArray<Example>(); }
        }





        #endregion

        #region Constructors

        public Dataset(Metadata metadata)
        {
            this._examples = new Dictionary<int, Example>();
            this._metadata = metadata;

        }

        #endregion

        #region IEnumerable Implementation

        public IEnumerator<Example> GetEnumerator()
        {
            foreach (Example example in this.Examples)
                yield return example;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this._examples.GetEnumerator();
        }

        #endregion

        #region Methods

        public void SetExamples(Example[] examples)
        {
            this._examples.Clear();
            foreach (Example example in examples)
                this._examples.Add(example.Index, example);

            this._size = examples.Length;
            this.UpdateValueCounts();
            this.UpdateMissingValues();
        }

        private void UpdateMissingValues()
        {
            foreach (Example examlpe in this)
            {
                for (int attributeIndex = 0; attributeIndex < this._metadata.Attributes.Length; attributeIndex++)
                {
                    if (examlpe[attributeIndex] == -1)
                    {
                        examlpe[attributeIndex] = GetMaxIndex(attributeIndex);
                    }
                }

            }
        }

        private int GetMaxIndex(int attributeIndex)
        {
            int maxIndex = 0;
            int maxCount = 0;

            for (int valueIndex = 0; valueIndex < this._metadata.Attributes[attributeIndex].Values.Length; valueIndex++)
            {
                if (this._metadata.Attributes[attributeIndex].ValueCounts[valueIndex] > maxCount)
                {
                    maxCount = this._metadata.Attributes[attributeIndex].ValueCounts[valueIndex];
                    maxIndex = valueIndex;
                }

            }

            return maxIndex;
        }

        private void UpdateValueCounts()
        {

            for (int attributeIndex = 0; attributeIndex < this._metadata.Attributes.Length; attributeIndex++)
            {
                Data.Attribute currentAttribute = this._metadata.Attributes[attributeIndex];

                for (int valueIndex = 0; valueIndex < currentAttribute.Values.Length; valueIndex++)
                    currentAttribute.ValueCounts[valueIndex] = this.Filter(attributeIndex, valueIndex).Count;

                for (int valueIndex = 0; valueIndex < this._metadata.Target.Values.Length; valueIndex++)
                    this._metadata.Target.ValueCounts[valueIndex] = this.Filter(valueIndex).Count;

            }

            this._metadata.Size = this._examples.Count;


        }

        public List<int> Filter(int labelValueIndex)
        {
            List<int> result = new List<int>();
            foreach (Example example in this)
            {

                    if (example.HierarchicalLabel.Contains(labelValueIndex))
                        result.Add(example.Index);
                
            }

            return result;
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

                foreach (Example example in this)
                {
                    if (example[attributeIndex] == valueIndex)
                        result.Add(example.Index);
                }
            }

            return result;

        }

        public List<int> Filter(int attributeIndex, int valueIndex, int labelValueIndex)
        {
            List<int> result = new List<int>();
            foreach (Example example in this)
            {
               
                    if (example[attributeIndex] == valueIndex && example.HierarchicalLabel.Contains(labelValueIndex))
                        result.Add(example.Index);
                
            }


            return result;

        }

        public List<int> Filter(List<int> paretnAttributeIndexes, List<int> parentValueIndexes, int attributeIndex, int valueIndex)
        {
            List<int> attributeIndexes = new List<int>();
            foreach (int val in paretnAttributeIndexes)
                attributeIndexes.Add(val);
            attributeIndexes.Add(attributeIndex);

            List<int> valueIndexes = new List<int>();
            foreach (int val in parentValueIndexes)
                valueIndexes.Add(val);
            valueIndexes.Add(valueIndex);


            return Filter(attributeIndexes, valueIndexes);
        }

        public List<int> Filter(List<int> attributeIndexes, List<int> valueIndexes)
        {
            if (attributeIndexes.Contains(this._metadata.Target.Index))
            {
                int indexOfTarget = attributeIndexes.IndexOf(this._metadata.Target.Index);
                int targetValueIndex = valueIndexes[indexOfTarget];

                List<int> att = new List<int>(attributeIndexes);
                List<int> val = new List<int>(valueIndexes);

                att.RemoveAt(indexOfTarget);
                val.RemoveAt(indexOfTarget);

                return this.Filter(att, val, targetValueIndex);

            }


            List<int> result = new List<int>();
            foreach (Example example in this)
            {
                bool IsMatch = true;

                for (int current = 0; current < attributeIndexes.Count; current++)
                {
                    int attributeIndex = attributeIndexes[current];
                    int valueIndex = valueIndexes[current];

                    if (example[attributeIndex] != valueIndex)
                    {
                        IsMatch = false;
                        break;
                    }
                }

                if (IsMatch)
                    result.Add(example.Index);
            }

            return result;

        }

        private List<int> Filter(List<int> attributeIndexes, List<int> valueIndexes, int labelValueIndex)
        {
            List<int> result = new List<int>();

            foreach (Example example in this)
            {
                if (example.HierarchicalLabel.Contains(labelValueIndex))
                {
                    bool IsMatch = true;

                    for (int current = 0; current < attributeIndexes.Count; current++)
                    {
                        int attributeIndex = attributeIndexes[current];
                        int valueIndex = valueIndexes[current];

                        if (example[attributeIndex] != valueIndex)
                        {
                            IsMatch = false;
                            break;
                        }
                    }

                    if (IsMatch)
                        result.Add(example.Index);
                }
            }

            return result;

        }
        
        public Dataset[] Split()
        {
            Dataset[] datasets = new Dataset[this._metadata.Target.Values.Length];

            for (int classIndex = 0; classIndex < this._metadata.Target.Values.Length; classIndex++)
            {
                datasets[classIndex] = new Dataset(this._metadata.Clone());
                Example[] examples = this.GetExamples(this.Filter(classIndex));
                datasets[classIndex].SetExamples(examples);
            }

            return datasets;
        }

        public Dataset[] SplitRandomly(double ratio)
        {
            Random r=new Random();

            Dataset[] datasets = new Dataset[2];

            datasets[0]=new Dataset(this._metadata.Clone());
            datasets[1]=new Dataset(this._metadata.Clone());

            List<Example> list1 = new List<Example>();
            List<Example> list2 = new List<Example>();

            for(int i=0; i<this._metadata.Size; i++)
            {
                double p= r.NextDouble();
                if(p < ratio)                
                    list1.Add(this[i].Clone() as Example);                
                else                
                    list2.Add(this[i].Clone() as Example);

                datasets[0].SetExamples(list1.ToArray());
                datasets[1].SetExamples(list2.ToArray());
               
            }

            return datasets;
        }
        
        public Dataset[] Split(string [] classValues)
        {
            Dataset[] datasets = new Dataset[classValues.Length];

            DataMining.Data.Attribute[] attClone = new Attribute[this.Metadata.Attributes.Length];
            for (int attributeIndex = 0; attributeIndex < this.Metadata.Attributes.Length; attributeIndex++)
                attClone[attributeIndex] = this.Metadata.Attributes[attributeIndex].Clone();
            string name = this._metadata.DatasetName;
            Data.Attribute target = new Attribute("class", this._metadata.Target.Index, classValues);

            DataMining.Data.Metadata metadata = new Data.Metadata(name, attClone, target);

            int newClassIndex=0;
            
            foreach(string classValue in classValues)
            {
                int counter=0;
                int classIndex = this._metadata.Target.GetIndex(classValue);
                              

                datasets[classIndex] = new Dataset(metadata);

                List<Example> examples = new List<Example>();

                foreach (int exampleIndex in this.Filter(classIndex))                
                    examples.Add(new Example(datasets[newClassIndex].Metadata, counter++, this[exampleIndex].Values, newClassIndex));
                                                
                datasets[newClassIndex].SetExamples(examples.ToArray());
                classIndex++;

                
            }

            return datasets;
 
        }

        public Dataset GetBinaryFlatLabelSubDataset(string positiveClassValue, string[] negativeClassValues)
        {            
            string name = this._metadata.DatasetName;
            int classIndex = this._metadata.Target.GetIndex(positiveClassValue);


            DataMining.Data.Attribute[] attClone = new Attribute[this.Metadata.Attributes.Length];
            for (int attributeIndex = 0; attributeIndex < this.Metadata.Attributes.Length; attributeIndex++)
                attClone[attributeIndex] = this.Metadata.Attributes[attributeIndex].Clone();

            Data.Attribute target = new Attribute("class", this._metadata.Target.Index, new string[] {"Yes","No" });
            DataMining.Data.Metadata metadata = new Data.Metadata(name, attClone, target);

            Dataset dsResult = new Dataset(metadata);


            int positiveClassIndex = this._metadata.Target.GetIndex(positiveClassValue);
            List<Example> positive = new List<Example>();

            int pcounter=0;
            foreach (int exampleIndex in this.Filter(classIndex))            
                positive.Add(new Example(dsResult.Metadata, pcounter++, this[exampleIndex].Values, 0));

            List<Example> negative=new List<Example>();

            int ncounter = 0;
            foreach (string negativeClassValue in negativeClassValues)
            {
                int negativeClassIndex = this._metadata.Target.GetIndex(negativeClassValue);
                foreach(int exampleIndex in this.Filter(negativeClassIndex))
                {
                    if (!negative.Exists(e => e.Index == exampleIndex))        
                        negative.Add(new Example(dsResult.Metadata, ncounter++, this[exampleIndex].Values, 1));
                   
                }
            }

  
            List<Example> examples = new List<Example>();
            examples.AddRange(positive);
            examples.AddRange(negative);
            dsResult.SetExamples(examples.ToArray());

            return dsResult;
                                   

        }

        private Example[] GetExamples(List<int> indexes)
        {
            List<Example> examples = new List<Example>();
            foreach (int index in indexes)
                examples.Add(this[index]);
            return examples.ToArray();

        }

        public Dataset[] Startify(int folds)
        {
            Dataset[] results = new Dataset[folds];
            List<Tuple<int, string>> classes = new List<Tuple<int, string>>();

            if (this.Metadata.IsHierarchical)
            {
                Node[] leaves = ((HierarchicalAttribute)this.Metadata.Target).Hierarchy.GetLeaves();
                foreach (Node leaf in leaves)
                    classes.Add(new Tuple<int, string>(leaf.ValueIndex, leaf.Name));
            }
            else
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

            List<Example> [] exampleLists = new List<Example>[folds];
            for (int index = 0; index < folds; index++)
                exampleLists[index] = new List<Example>();


            foreach (Tuple<int,string> classTuple in classes)
            {
                int foldPointer = 0;

                foreach(Example example in this.GetExamples(this.Filter(classTuple.Item1)))
                {
                    exampleLists[foldPointer].Add(example.Clone() as Example);
                    foldPointer++;
                    if (foldPointer == folds)
                        foldPointer = 0;
                }
 
            }

            for (int index = 0; index < folds; index++)
                results[index].SetExamples(exampleLists[index].ToArray());

            return results;
            
            
            
 
        }

        public override string ToString()
        {
            return this._metadata.DatasetName;
        }

        #endregion


    }
}
