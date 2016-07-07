using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public struct  Term
    {
        // if attribute index=-1 then it is the class attribute

        public int AttributeIndex;
        public int ValueIndex;

        public Term(int attributeIndex, int valueIndex)
        {
            this.AttributeIndex = attributeIndex;
            this.ValueIndex = valueIndex;
        }
    }
}
