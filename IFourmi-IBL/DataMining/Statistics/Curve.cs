using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.DataMining.Statistics
{
    public class Curve
    {
        #region Data Members

        private List<Point> _coordinates;

        #endregion

        #region Properties

        public int Size
        {
            get { return this._coordinates.Count; }

        }

        public Point[] Points
        {
            get
            {
                return this._coordinates.ToArray();
            }
        }

        public bool IsEmpty
        {
            get
            {
                return _coordinates.Count == 0;
            }
        }

        #endregion

        #region Constructors

        public Curve()
        {
            _coordinates = new List<Point>();
        }

        #endregion

        #region Methods

        public void Add(Point point)
        {
            _coordinates.Add(point);
        }


        public void Add(double x, double y)
        {
            this.Add(new Point(x, y));
        }


        public Point GetLast()
        {
            return this.GetLast(1);
        }


        public Point GetLast(int n)
        {
            if (_coordinates.Count < n)
            {
                return null;
            }

            return this._coordinates[_coordinates.Count - n];
        }


        public double CalculateArea()
        {
            if (_coordinates.Count < 2)
            {
                return 0.0;
            }

		double area = 0.0;
		Point from = _coordinates[0];

		for (int i = 1; i < _coordinates.Count; i++)
		{
			Point to = _coordinates[i];
			double baseValue = to.X - from.X;
			double height = (to.Y + from.Y) / 2.0;
			area += (baseValue * height);
			from = to;
		}

		return area;

        }

        #endregion


    }
}
