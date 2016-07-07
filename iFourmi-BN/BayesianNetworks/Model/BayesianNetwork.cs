using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public class BayesianNetwork
    {

        #region Data Members

        protected Variable[] _variables;
        protected bool[,] _edges;
        public static Dictionary<string, string> cache = new Dictionary<string, string>();
        protected DataMining.Data.Metadata _metadata;

        public static double default_value = 0.0001;


        #endregion

        #region Properties

        public Variable[] Variables
        {
            get { return this._variables; }
        }



        public DataMining.Data.Metadata Metadata
        {
            get { return this._metadata; }
        }

        public Edge[] Edges
        {
            get
            {
                List<Edge> edges = new List<Edge>();
                for (int parentIndex = 0; parentIndex < this._edges.GetLength(0); parentIndex++)
                    for (int childIndex = 0; childIndex < this._edges.GetLength(0); childIndex++)
                        if (this._edges[parentIndex, childIndex])
                            edges.Add(new Edge(parentIndex, childIndex));
                return edges.ToArray();

            }
        }


        #endregion

        #region constructors

        public BayesianNetwork()
        { }

        public BayesianNetwork(DataMining.Data.Metadata metasata)
        {
            this._metadata = metasata;
            List<DataMining.Data.Attribute> attributes = new List<DataMining.Data.Attribute>(this._metadata.Attributes);
            this.SetVariables(attributes);
            this._edges = new bool[this._variables.Length, this._variables.Length];
        }

        public BayesianNetwork(DataMining.Data.Metadata metadata, List<Edge> edges)
            : this(metadata)
        {


            foreach (Edge edge in edges)
                this.Connect(edge.ParentIndex, edge.ChildIndex);

        }

        protected void SetVariables(List<DataMining.Data.Attribute> attributes)
        {
            this._variables = new Variable[attributes.Count];

            for (int index = 0; index < attributes.Count; index++)
                _variables[index] = new Variable(attributes[index]);

        }

        #endregion

        #region Methods

        public void Connect(int parentIndex, int childIndex)
        {
            if (this._edges == null)
                this._edges = new bool[this._variables.Length, this._variables.Length];

            this._variables[childIndex].AddParent(this._variables[parentIndex]);
            this._edges[parentIndex,childIndex] = true;
        }

        public void Disconnect(int parentIndex, int childIndex)
        {
            if (this._edges != null)
            {

                this._variables[childIndex].RemoveParent(this._variables[parentIndex]);
                this._edges[parentIndex,parentIndex] = false;
            }
        }

        public void LearnParameters(DataMining.Data.Dataset dataset)
        {
            foreach (Variable variable in this._variables)
            {

                string desc = variable.ToString();
                variable.CPT = new ConditionalProbabilityTable(desc, variable.GetCTPSize());
                //if (cache.ContainsKey(desc))
                //    variable.CPT.LoadFormString(cache[desc]);
                //else
                //{

                    if (variable.Parents == null)
                    {
                        int rowIndex = 0;
                        DataMining.Data.Attribute attribute = dataset.Metadata.Target.Index == variable.Index ? dataset.Metadata.Target : dataset.Metadata.Attributes[variable.Index];

                        for (int valueIndex = 0; valueIndex < variable.Attribute.Values.Length; valueIndex++)
                        {
                            int indexExpression = valueIndex + 1;
                            int count = attribute.ValueCounts[valueIndex];                
                            variable.CPT.Set(rowIndex, indexExpression, count);
                            rowIndex++;

                        }
                    }

                    else
                    {
                        List<int> attributeIndexes = new List<int>();
                        DataMining.Data.Attribute[] attributes = new DataMining.Data.Attribute[variable.Parents.Count + 1];
                        attributes[0] = variable.Attribute;
                        attributeIndexes.Add(variable.Attribute.Index);

                        if (variable.Parents != null)
                        {
                            int i = 1;
                            foreach (Variable parent in variable.Parents)
                            {
                                attributes[i] = parent.Attribute;
                                attributeIndexes.Add(parent.Attribute.Index);
                                i++;
                            }
                        }

                        int[] valuesPointers = new int[attributes.Length];
                        Stack<int> pointerStack = new Stack<int>();

                        int rowIndex = 0;

                        bool stop = false;

                        while (true)
                        {
                            int indexExpression = variable.GetIndexExpression(valuesPointers.ToArray());
                            int count = dataset.Filter(attributeIndexes, valuesPointers.ToList<int>()).Count;
                            variable.CPT.Set(rowIndex, indexExpression, count);
                            rowIndex++;

                            int attributePointer = valuesPointers.Length - 1;

                            while (true)
                            {
                                if (valuesPointers[attributePointer] == attributes[attributePointer].Values.Length - 1)
                                {
                                    if (attributePointer == 0)
                                    {
                                        stop = true;
                                        break;
                                    }

                                    valuesPointers[attributePointer] = 0;
                                    attributePointer--;
                                }
                                else
                                {
                                    valuesPointers[attributePointer]++;
                                    break;

                                }
                            }

                            if (stop)
                                break;


                        }

                    //}

                    //cache[desc] = variable.CPT.ToString();

                }

            }

        }

        public double GetProbability(DataMining.Data.Example example)
        {
            double probability = 1;
            double termProbability = 0;

            for (int attributeIndex = 0; attributeIndex < example.Values.Length; attributeIndex++)
            {
                Variable variable = this.Variables[attributeIndex];
                int valueIndex = example[attributeIndex];

                if (variable.Parents == null)
                {
                    termProbability = variable.Attribute.ValueCounts[valueIndex] / (double)this._metadata.Size;
                    if (termProbability == 0)
                        termProbability = BayesianNetwork.default_value;
                }

                else
                {

                    List<Term> parentValues = new List<Term>();
                    foreach (Variable parent in variable.Parents)
                    {
                        if (parent.Index != this._metadata.Target.Index)
                            parentValues.Add(new Term(parent.Index, example[parent.Index]));
                    }

                    termProbability = this.GetConditionalProbability(new Term(attributeIndex, example[attributeIndex]), parentValues);

                }

                probability *= termProbability;
            }

            return probability;

        }

        public double GetConditionalProbability(Term variableTerm, List<Term> parentTerms)
        {
            double probability = 0.0;
            Variable variable = this.Variables[variableTerm.AttributeIndex];

            int[] parentValueIndexes = new int[variable.Parents.Count];
            for (int index = 0; index < parentTerms.Count; index++)
                parentValueIndexes[index] = parentTerms[index].ValueIndex;

            int indexExpression = variable.GetIndexExpression(variableTerm.ValueIndex, parentValueIndexes);

            probability = variable.CPT.GetConditonalProbability(indexExpression);
            if (probability == 0)
                probability = BayesianNetwork.default_value;

            return probability;
        }

        #endregion
    }
}
