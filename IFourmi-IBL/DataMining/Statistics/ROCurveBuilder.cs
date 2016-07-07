using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Statistics
{
    public class ROCurveBuilder : CurveBuilder
    {
        public override Curve CreateCurve(double[][] values)
        {
            Curve curve = new Curve();

            CurveBuilder.Sort(values);
            double threshold = values[0][0];
            double pTotal = CurveBuilder.GetClassCount(values, 1);
            double nTotal = CurveBuilder.GetClassCount(values, 0);

            int tpCounter = 0;
            int fpCounter = 0;

            double x = -1;
            double y = -1;

            for (int i = 0; i < values.Length; i++)
            {
                if (values[i][0] != threshold && (tpCounter + fpCounter) != 0)
                {
                    if (curve.IsEmpty)
                    {
                        curve.Add(0.0, 0.0);
                    }

                    x = (fpCounter == 0 ? 0.0 : fpCounter / nTotal);
                    y = (tpCounter == 0 ? 0.0 : tpCounter / pTotal);
                    curve.Add(x, y);
                    threshold = values[i][0];
                }

                if (values[i][1] == 1)
                {
                    tpCounter++;
                }
                else
                {
                    fpCounter++;
                }
            }

            // adds the last point to the curve

            x = (fpCounter == 0 ? 0.0 : fpCounter / nTotal);
            y = (tpCounter == 0 ? 0.0 : tpCounter / pTotal);
            curve.Add(x, y);
            curve.Add(1.0, 1.0);

            return curve;
        }

    }
}
