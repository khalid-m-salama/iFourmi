using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Data ;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public class Variable
    {
        #region Data Member

        private iFourmi.DataMining.Data.Attribute _attribute;
        private List<Variable> _parents;        
        private ConditionalProbabilityTable _cpt;

        #endregion;

        #region Properties

        public iFourmi.DataMining.Data.Attribute Attribute
        {
            get { return this._attribute; }
        }

        public int Index
        {
            get { return this._attribute.Index; }
        }

        public List<Variable> Parents
        {
            get { return this._parents; }
        }

        //public List<Variable> Children
        //{
        //    get { return this._children; }
        //}

        public ConditionalProbabilityTable CPT
        {
            get { return this._cpt; }
            set { this._cpt = value; }
        }

     

        #endregion

        #region Constructors

        public Variable(iFourmi.DataMining.Data.Attribute attribute)
        {
            this._attribute = attribute;
        }

        #endregion

        #region Method

        public void AddParent(Variable parent)
        {
            if (this._parents == null)
                this._parents = new List<Variable>();

            if(!this.Parents.Contains(parent))
                this._parents.Add(parent);
            //parent.Children.Add(this);
        }

        public void RemoveParent(Variable parent)
        {
            if (this._parents != null)
            {
                if (this.Parents.Contains(parent))
                    this._parents.Remove(parent);
            }
            //parent.Children.Remove(this);
        }


        public int GetIndexExpression(params int[] valueIndexes)
        {
            int indexExpression = 0;
            for (int i = 0; i < valueIndexes.Length; i++)
            {
                indexExpression += (valueIndexes[i]+1) * (int)Math.Pow(10, valueIndexes.Length - 1 -i );
            }
            
            return indexExpression;

        }


        public   int GetIndexExpression(int variableValueIndex, params int[] parentValueIndexes)
        {
            int indexExpression=(variableValueIndex+1) * (int)Math.Pow(10,parentValueIndexes.Length);

            if (parentValueIndexes != null)
            {
                for (int i = 0; i < parentValueIndexes.Length; i++)
                {
                    indexExpression += (parentValueIndexes[i]+1) * (int)Math.Pow(10, parentValueIndexes.Length -1 - i);

                }
            }

            return indexExpression;

        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{" + this._attribute.Name + "}");
           

            if (this._parents != null)
            {
                builder.Append("<--");
                builder.Append("{");
                foreach (Variable parent in this._parents)
                {
                    builder.Append(parent.Attribute.Name);
                    builder.Append(",");
                }

                builder.Remove(builder.Length - 1, 1);
                builder.Append("}");
            }

            return builder.ToString();
        }

        public int GetCTPSize()
        {
            int result = this.Attribute.Values.Length;
            if (this._parents != null)
            {
                foreach (Variable parent in this._parents)
                    result *= parent.Attribute.Values.Length;
            }
            return result;
        }

        #endregion


        
    }
}
