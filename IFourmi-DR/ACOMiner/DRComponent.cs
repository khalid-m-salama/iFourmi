using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.ACOMiner
{
    public class DRComponent
    {
        public bool Include;
        public int ElemetIndex;
        public DatasetElementType ElementType;

        public DRComponent(DatasetElementType elementType, int elementIndex,bool include)            
        {
            this.ElementType = elementType;
            this.ElemetIndex = elementIndex;
            this.Include = include; 
        }
  
        public override string ToString()
        {
            return ElementType.ToString() + "," + ElemetIndex.ToString() + Include + ",";
        }
    }

    public enum DatasetElementType
    {
        Attribute,
        Instance
    }
}
