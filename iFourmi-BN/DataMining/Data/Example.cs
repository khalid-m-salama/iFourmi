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
        private int[] _values;
        private List<int> _label;
        private Metadata _metadata;

        #endregion

        #region Properties

        public int Index
        {
            get { return this._index; }
        }

        public int[] Values
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

        public int[] HierarchicalLabel
        {
            get { return this._label.ToArray(); }
            set { this._label = value.ToList(); }
        }

        public int this[int attributeIndex]
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


        public Example(Metadata Metadata, int index, int[] values, int label)
        {
            this._metadata = Metadata;
            this._index = index;
            this._values = values;
            //this._label = label;
            this._label = new List<int>();
            this._label.Add(label);
        }

        public Example(Metadata Metadata, int index, int[] values, List<int> label)
        {
            this._metadata = Metadata;
            this._index = index;
            this._values = values;
            //this._label = label;
            this._label = label;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            //builder.Append("{");

            for (int index = 0; index < this._values.Length ; index++)
            {
                int valueIndex=this._values[index];
                builder.Append( this._metadata.Attributes[index].Values[valueIndex]);
                builder.Append(", "); 
            }

            builder=builder.Remove(builder.Length - 1,1);
            //builder.Append("}");
            //builder.Append("{");

            if (_label[0] == -1)
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
            //builder.Append("}");
            return builder.ToString();

        }

        public object Clone()
        {
            List<int> clone = new List<int>();
            foreach (int value in _label)
                clone.Add(value);

            return new Example(this._metadata, this._index, (int[])this._values.Clone(), clone);
        }

        #endregion

      
    }
}
