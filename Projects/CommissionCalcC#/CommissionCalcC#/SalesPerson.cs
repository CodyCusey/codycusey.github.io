// Cody Cusey - 11/28/2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommissionCalcC#

    internal class SalesPerson
    {
        public string name { get; set; }
        public int level { get; set; }
        public double sales { get; set; }
        public double hoursWorked { get; set; }
        public int id { get; set; }

        public SalesPerson(int id, string name, int level, double sales, double hoursWorked)
        {
            this.id = id;
            this.name = name;
            this.level = level;
            this.sales = sales;
            this.hoursWorked = hoursWorked;
        }

        public override string ToString()
        {
            return string.Format("Employee Name: {0} \nCommission: {1:C} \nWage Pay: {2:C} \nBonus: {3:C} \nGross Pay: {4:C}\n", name, CalculateCommission(), CalculatePay(), CalculateBonus(), CalculateGrandTotal());
        }

        public double CalculateCommission()
        {
            double totalComm = 0;
            double commRate = 0;

            if (level == 1)
            {
                if (sales <= 2000)
                {
                    commRate = .0525;
                }
                else
                {
                    if (sales < 3500)
                    {
                        commRate = .075;
                    }
                    else
                    {
                        commRate = .1075;
                    }
                }
            }
            else
            {
                if (level == 2)
                {
                    if (sales <= 2800)
                    {
                        commRate = .065;
                    }
                    else
                    {
                        commRate = .0775;
                    }
                }
                else
                {
                    if (level == 3)
                    {
                        commRate = .0825;
                    }
                    else
                    {
                        if (level == 4)
                        {
                            commRate = .0975;
                        }
                        else
                        {
                            commRate = 0;
                        }
                    }
                }
            }
            return totalComm = sales * commRate;

        }

        public double CalculatePay()
        {
            double hourlyRate = 0;
            double totalPay = 0;
            double regularPay = 0;
            double otPay = 0;

            switch (level)
            {
                case 1:
                    hourlyRate = 12.75;
                    break;
                case 2:
                    hourlyRate = 14.25;
                    break;
                case 3:
                    hourlyRate = 17.50;
                    break;
                case 4:
                    hourlyRate = 21.75;
                    break;
            }

            regularPay = hourlyRate * hoursWorked;

            if(hoursWorked > 40)
            {
                otPay = (hoursWorked - 40) * (hourlyRate * 1.5);
                regularPay = hourlyRate * 40;
            }

            return totalPay = regularPay + otPay;
        }

        public double CalculateBonus()
        {
            double bonusTotal = 0;

            if(sales > 24000 && level > 0 && level < 5)
            {
                bonusTotal = .03;
                bonusTotal = sales * bonusTotal;
            }

            return bonusTotal;
        }

        public double CalculateGrandTotal()
        {
            double grandTotal = CalculateBonus() + CalculateCommission() + CalculatePay();

            return grandTotal;
        }




    }
}
