using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using iFourmi.BayesianNetworks.Model;
using iFourmi.DataMining.Data;

namespace iFourmi.BayesianNetworks.Utilities
{
    public class K2Calculator
    {
        public double Calculate(Variable variable, Dataset dataset)
        {           

            double k2 = 0.0;

            if (variable.Parents != null)
            {
                List<int> parentAttributeIndexes = new List<int>();
                DataMining.Data.Attribute[] parentAttributes = new DataMining.Data.Attribute[variable.Parents.Count];                
                
                int i = 0;
                foreach (Variable parent in variable.Parents)
                {
                    parentAttributes[i] = parent.Attribute;
                    parentAttributeIndexes.Add(parent.Attribute.Index);
                    i++;
                }


                int[] valuesPointers = new int[parentAttributes.Length];
                Stack<int> pointerStack = new Stack<int>();           
                                
                bool stop = false;

                while (true)
                {

                    int r = variable.Attribute.Values.Length;
                    int Nj = 0;
                    double part1 = 0;
                    double part2 = 0;


                    for (int k = 0; k < variable.Attribute.Values.Length; k++)
                    {
                        int  Njk = dataset.Filter(parentAttributeIndexes, valuesPointers.ToList<int>(),variable.Index, k).Count;
                        Nj += Njk;
                        double facNjk = Factorial(Njk);
                        part2 += Math.Log(facNjk);
                    }

                    double facR = Factorial(r - 1);
                    double facNjR = Factorial(Nj + r - 1);

                    part1 = Math.Log(facR/facNjR);

                    //double value = double.IsInfinity(part1 + part2) ? 0 : part1 + part2;

                    double value = part1 + part2;

                    k2 += value;


                    int attributePointer = valuesPointers.Length - 1;

                    while (true)
                    {
                        if (valuesPointers[attributePointer] == parentAttributes[attributePointer].Values.Length - 1)
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

            }

            return k2;
        }

        public double Factorial(double value)
        {
            //if (value == 1)
            //    return value;
            //return value * Factorial(value - 1);

            return value;

        }
    }
}
