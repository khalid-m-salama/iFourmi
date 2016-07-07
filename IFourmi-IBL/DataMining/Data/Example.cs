using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Data
{
    public class Example: ICloneable
    {
        #region Data Members

        private int _index;
        private double[] _values;
        private List<int> _label;
        private Metadata _metadata;

        #endregion

        #region Properties

        public int Index
        {
            get { return this._index; }
        }

        public double[] Values
        {
            get { return this._values; }
        }

        public int Label
        {
            get { return this._label[0]; }
            set
            {
                this._label = new List<int>();
                this._label.Add(value);

            }
        }

        public int[] Labels
        {
            get { return this._label.ToArray(); }
            set { this._label = value.ToList(); }
        }

        public bool[] LabelFlags
        {
            get
            {
                bool[] flags = new bool[this.Metadata.Target.Values.Length];
                foreach (int label in this._label)
                    flags[label] = true;

                return flags;
            }
        }

   

        public double this[int attributeIndex]
        {
            get { return this._values[attributeIndex]; }
            internal set { this._values[attributeIndex] = value; }
        }

        public Metadata Metadata
        {
            get { return this._metadata; }
            set { this._metadata = value; }
            
        }

        #endregion

        #region Constructor


        public Example(Metadata Metadata, int index, double[] values, int label)
        {
            this._metadata = Metadata;
            this._index = index;
            this._values = values;
            this._label = new List<int>();
            this._label.Add(label);
        }

        public Example(Metadata Metadata, int index, double[] values, List<int> label)
        {
            this._metadata = Metadata;
            this._index = index;
            this._values = values;            
            this._label = label;
        }

    
        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            
            for (int index = 0; index < this._values.Length; index++)
            {
                if (this._metadata.Attributes[index] is NumericAttribute)
                {
                    builder.Append(this._values[index].ToString());
                    builder.Append(", ");
                }
                else
                {

                    int valueIndex = (int)this._values[index] ;
                    builder.Append(((NominalAttribute)this._metadata.Attributes[index]).Values[valueIndex]);
                    builder.Append(", ");
                }
            }

            builder = builder.Remove(builder.Length - 1, 1);
            
            if (this._label[0] == -1)
            {
                foreach (int label in this._label)
                {
                    builder.Append("?");
                    builder.Append(',');
                }

            }
            else
            {

                foreach (int label in this._label)
                {
                    builder.Append(this._metadata.Target.Values[label]);
                    builder.Append(',');
                }
            }

            builder.Remove(builder.Length - 1, 1);            
            return builder.ToString();

        }


        #endregion

        public object Clone()
        {
            List<int> cloneLabel = new List<int>();
            foreach (int value in this._label)
                cloneLabel.Add(value);

            return new Example(this._metadata, this._index, (double[])this._values.Clone(), cloneLabel);
        }
    }
}
