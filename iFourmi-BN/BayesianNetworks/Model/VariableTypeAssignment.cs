using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianNetworks.Model
{
    public enum VariableType
    {
        Cause,
        Effect,
        None
    }

    public struct VariableTypeAssignment
    {
        public int VariableIndex;
        public VariableType Type;
        public VariableTypeAssignment(int index, VariableType type)
        {
            this.VariableIndex = index;
            this.Type = type;
        }

        public override string ToString()
        {
            return "[" + VariableIndex.ToString() + "-"+this.Type.ToString() + "]";
        }
    }

}
