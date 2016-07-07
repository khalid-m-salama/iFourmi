using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Statistics
{
    public class PRCurveBuilder:CurveBuilder
    {
     
		private static double DELTA = 1e-15;


        public override Curve CreateCurve(double[][] values)
		{
			Curve curve = new Curve();

			CurveBuilder.Sort(values);
			double threshold = values[0][0];
			int pTotal = CurveBuilder.GetClassCount(values, 1);

			int tpCounter = 0;
			int fpCounter = 0;
			int previousTp = 0;
			int previousFp = 0;

			for (int i = 0; i < values.Length; i++)
			{
				if (values[i][0] != threshold && (tpCounter + fpCounter) != 0)
				{
					double precision =
						tpCounter / (double) (tpCounter + fpCounter);
					double recall = tpCounter / (double) pTotal;

					if (!curve.IsEmpty)
					{
						// interpolating precision-recall points
						interpolate(tpCounter, fpCounter, previousTp,
							previousFp, pTotal, curve);
					}

					Normalize(curve, precision, recall);
					previousTp = tpCounter;
					previousFp = fpCounter;

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

			// add the last point to the curve

			interpolate(tpCounter, fpCounter, previousTp, previousFp, pTotal,
				curve);

			double p = tpCounter / (double) (tpCounter + fpCounter);
			double r = tpCounter / (double) pTotal;
			Normalize(curve, p, r);

			return curve;
		}

        public Curve CreateCurve(double[][] values, double[] thresholds)
		{
			Curve curve = new Curve();

			CurveBuilder.Sort(values);
			Array.Sort(thresholds);

			int pTotal = CurveBuilder.GetClassCount(values, 1);

			int tpCounter = 0;
			int fpCounter = 0;
			int previousTp = 0;
			int previousFp = 0;
			int index = 0;

			for (int i = (thresholds.Length - 1); i >= 0; i--)
			{
				while ((index < values.Length)
					&& (values[index][0] >= thresholds[i]))
				{
					if (values[index][1] == 1)
					{
						tpCounter++;
					}
					else
					{
						fpCounter++;
					}

					index++;
				}

				if ((previousTp != tpCounter) || (previousFp != fpCounter))
				{
					double precision =
						tpCounter / (double) (tpCounter + fpCounter);
					double recall = tpCounter / (double) pTotal;

					if (curve.IsEmpty)
					{
						curve.Add(0.0, precision);
					}
					else
					{
						// interpolating precision-recall points
						interpolate(tpCounter, fpCounter, previousTp,
							previousFp, pTotal, curve);
					}

					Normalize(curve, precision, recall);
					previousTp = tpCounter;
					previousFp = fpCounter;
				}
			}

			// add the last point to the curve

			interpolate(tpCounter, fpCounter, previousTp, previousFp, pTotal,
				curve);

			double p = tpCounter / (double) (tpCounter + fpCounter);
			double r = tpCounter / (double) pTotal;
			Normalize(curve, p, r);

			return curve;
		}

		/**
		 * Interpolates precision-recall values.
		 * 
		 * @param tpCounter the current TP counter.
		 * @param fpCounter the current FP counter.
		 * @param previousTp the previous TP counter.
		 * @param previousFp the previous FP counter.
		 * @param pTotal the total positive count.
		 * @param prCurve the curve object.
		 */
		private void interpolate(int tpCounter, int fpCounter, int previousTp,
				int previousFp, int pTotal, Curve curve)
		{
			for (int x = 1; x < (tpCounter - previousTp); x++)
			{
				int tpX = (previousTp + x);
				double skew =
					(fpCounter - previousFp)
						/ (double) (tpCounter - previousTp);

				double p = tpX / (tpX + previousFp + (skew * x));
				double r = tpX / (double) pTotal;

				Normalize(curve, p, r);
			}
		}

		private void Normalize(Curve curve, double precision, double recall)
		{
			if (curve.IsEmpty)
			{
				curve.Add(0.0, precision);
			}

			Point p1 = curve.GetLast(1);
			Point p2 = curve.GetLast(2);

			if (p1 != null && p2 != null
				&& !(p1.X == recall && p1.Y == recall))
			{
				if (Math.Abs(p1.Y - precision) < DELTA
					&& Math.Abs(p2.Y - precision) < DELTA)
				{
					p1.X=recall;
				}
				else if (Math.Abs(p1.X - recall) < DELTA
					&& Math.Abs(p2.X - recall) < DELTA)
				{
					p1.Y=precision;
				}
				else
				{
					curve.Add(new Point(recall, precision));
				}
			}
			else
			{
				curve.Add(new Point(recall, precision));
			}
		}
	}
    
}
