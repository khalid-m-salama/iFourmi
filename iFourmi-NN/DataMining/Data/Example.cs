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
        private int _label;
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
            get { return this._label; }
            set 
            {
                this._label = value;
               
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
                double value=this._values[index];
                builder.Append(Math.Round(value,3).ToString());
                builder.Append(", "); 
            }

            builder.Append(this._metadata.Target[_label]);
            
            //builder.Append("}");
            return builder.ToString();

        }

        #endregion

        public object Clone()
        {
            
            return new Example(this._metadata, this._index, (double [])this._values.Clone(),this._label);
        }
    }
}
