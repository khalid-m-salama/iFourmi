using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iFourmi.OptimizationFunctions
{
    public interface IFunction
    {
        /// <summary>
        /// return the Calculated fitness as inverse if its minization problem, and as it is if not.
        /// </summary>
        /// <param name="variables"></param>
        /// <returns></returns>
         double Calculate(double[] variables);

         OptimizationType Type
         {
             get;
         }

         int Size
         {
             get;
         }

         double MaximumVariableValue
         {
             get;             
         }

         double BestFittness
         {
             get;
         }

         double MinimumVariableValue
         {
             get;             
         }


         string Description
         {
             get;
         }

         
 
    }
}
