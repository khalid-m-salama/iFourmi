using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.DataMining.Model;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public class BayesianNetworkClassifier : BayesianNetwork, DataMining.Model.IClassifier
    {
        private List<VariableTypeAssignment> _variableTypeAssignments;
        

        #region properties

        public string Desc
        {
            get;
            set;
        }

        public VariableTypeAssignment[] VariableTypeAssignments
        {
            get { return this._variableTypeAssignments.ToArray(); }
        }


        #endregion

        #region Constructor

        public BayesianNetworkClassifier(DataMining.Data.Metadata metadata)
        {
            this._metadata = metadata;
            List<DataMining.Data.Attribute> attributes = new List<DataMining.Data.Attribute>(this._metadata.Attributes);
            attributes.Add(this._metadata.Target);
            this.SetVariables(attributes);
            this._edges = new bool[this._variables.Length, this._variables.Length];

            this._variableTypeAssignments = new List<VariableTypeAssignment>();

            for (int childIndex = 0; childIndex < this._edges.GetLength(1); childIndex++)
            {
                if (childIndex != this._metadata.Target.Index)
                {
                    this.Connect(this._metadata.Target.Index, childIndex);
                    this._variableTypeAssignments.Add(new VariableTypeAssignment(childIndex, VariableType.Effect));
                }
            }
        }

        public BayesianNetworkClassifier(DataMining.Data.Metadata metadata, List<Edge> edges)
            : this(metadata)
        {

            foreach (Edge edge in edges)
                this.Connect(edge.ParentIndex, edge.ChildIndex);
        }

        public BayesianNetworkClassifier(DataMining.Data.Metadata metadata, List<VariableTypeAssignment> variableTypeList)            
        {
            this._metadata = metadata;
            List<DataMining.Data.Attribute> attributes = new List<DataMining.Data.Attribute>(this._metadata.Attributes);
            attributes.Add(this._metadata.Target);
            this.SetVariables(attributes);
            this._edges = new bool[this._variables.Length, this._variables.Length];

            int classVariableIndex=this._metadata.Target.Index;

            this._variableTypeAssignments = variableTypeList;

            foreach (VariableTypeAssignment assignment in variableTypeList)
            {
                if (assignment.Type == VariableType.Cause)
                    this.Connect(assignment.VariableIndex, classVariableIndex);
                else if (assignment.Type == VariableType.Effect)
                    this.Connect(classVariableIndex, assignment.VariableIndex);
            }
            
        }

        public BayesianNetworkClassifier(DataMining.Data.Metadata metadata, List<VariableTypeAssignment> VariableTypeAssignments, List<Edge> edges)
            : this(metadata, VariableTypeAssignments)
        {
            foreach (Edge edge in edges)
                this.Connect(edge.ParentIndex, edge.ChildIndex);            
        }


        #endregion

        #region Methods

        public  double GetProbability(DataMining.Data.Example example, int classValueIndex)
        {
            //initial proir class probability
            double probability = this._metadata.Target.ValueCounts[classValueIndex] / (double)this._metadata.Size;
            double termProbability = 0;

            Variable classVariable = this.Variables[this._metadata.Target.Index];
            if (classVariable.Parents != null)
            {
                List<Term> parentValues = new List<Term>();
                foreach (Variable parent in classVariable.Parents)
                    parentValues.Add(new Term(parent.Index, example[parent.Index]));

                //proir class probability when it has parents
                probability = this.GetConditionalProbability(new Term(classVariable.Index, classValueIndex), parentValues);
            }

            //------------------------------------------------------------------------------------
            //liklihood (probability of child varibales)
   

            for (int attributeIndex = 0; attributeIndex < example.Values.Length; attributeIndex++)
            {
                Variable variable = this.Variables[attributeIndex];
                int valueIndex = example[attributeIndex];

                if (variable.Parents == null)
                    continue;

                List<Term> parentValues = new List<Term>();
                foreach (Variable parent in variable.Parents)
                {
                    if (parent.Index == this._metadata.Target.Index)
                        parentValues.Add(new Term(parent.Index, classValueIndex));
                    else
                        parentValues.Add(new Term(parent.Index, example[parent.Index]));
                }

                termProbability = this.GetConditionalProbability(new Term(attributeIndex, example[attributeIndex]), parentValues);                
                probability *= termProbability;
            }

            return probability;


        }

        public Prediction Classify(DataMining.Data.Example example)
        {
            example = example.Clone() as DataMining.Data.Example;
            example.Label = -1;

            double[] classProbabilities = new double[this._metadata.Target.Values.Length];
            int resultIndex = 0;

            for (int classIndex = 0; classIndex < classProbabilities.Length; classIndex++)
            {
                double probability = this.GetProbability(example, classIndex);

                classProbabilities[classIndex] = probability;

                if (probability > classProbabilities[resultIndex])
                    resultIndex = classIndex;

            }

            double sum = classProbabilities.Sum();
            for (int i = 0; i < classProbabilities.Length; i++)
                classProbabilities[i] /= sum;

            return new Prediction(resultIndex, classProbabilities[resultIndex]);


        }

        #endregion
      
    }
}
