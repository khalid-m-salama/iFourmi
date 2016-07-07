using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace iFourmi.BayesianNetworks.Model
{
    [Serializable()]
    public class ConditionalProbabilityTable
    {
        #region Data Members

        private int [,] _ctp;

        private string _desc;

        #endregion

        #region Properties

        public int [,] Table
        {
            get { return this._ctp; }
        }

        public string Description
        {
            get { return this._desc; }
        }

        

        #endregion

        #region Constructors

        public ConditionalProbabilityTable(string desc, int size)
        {
            this._desc = desc;
            _ctp = new int [size, 2];
        }

        #endregion

        #region Methods

        public int Get(int indexExpression)
        {
            int count = -1;

            for (int index = 0; index < this._ctp.Length; index++)
            {
                if (this._ctp[index, 0] == indexExpression)
                {
                    count = this._ctp[index, 1];
                    break;
                }


            }

            return count;
        }

        public void Set(int position, int indexExpression, int value)
        {
            this._ctp[position, 0] = indexExpression;
            this._ctp[position, 1] = value;
        }

        public override string  ToString()
        {
            StringBuilder builder = new StringBuilder();
            for (int rowIndex = 0; rowIndex < this._ctp.GetLength(0); rowIndex++)
            {
                builder.Append(this._ctp[rowIndex,0]+","+this._ctp[rowIndex,1]);
                builder.Append(";");
            }

            builder.Remove(builder.Length - 1, 1);
            return builder.ToString();
        }

        public void LoadFormString(string expression)
        {
            string[] rows = expression.Split(';');
            for (int rowIndex = 0; rowIndex < this._ctp.GetLength(0); rowIndex++)
            {
                string[] columns = rows[rowIndex].Split(',');
                this._ctp[rowIndex, 0] = int.Parse(columns[0]);
                this._ctp[rowIndex, 1] = int.Parse(columns[1]);
            }

        }

        public double GetConditonalProbability(int indexExpression)
        {
            double probability1 = 0;
            double probability2 = 0;

            int lenght = (int)Math.Pow(10, indexExpression.ToString().Length - 1);
            int rem = indexExpression % lenght;

            for (int index = 0; index < this._ctp.GetLength(0); index++)
            {
                int currentExpression = this._ctp[index, 0];

                if (currentExpression % lenght == rem)
                {
                    probability2 += this._ctp[index, 1];
                }


                if (indexExpression == currentExpression)
                {
                    probability1 = this._ctp[index, 1];

                }


            }

            if (probability2 == 0)
                probability1 = 0;
            else
                probability1 = probability1 / probability2;

            return probability1;
        }

        #endregion

     
    }
}
