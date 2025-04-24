// Cody Cusey - 11/28/2022

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CommissionCalcC#
{
    internal class Program
    {
        public static Dictionary<int, SalesPerson> SalePersons = new Dictionary<int, SalesPerson>();

        static void Main(string[] args)
        {
            int numOfPersons = 0;
            string firstName;
            int level = 0;
            double hoursWorked = 0;
            double amount = 0;
            double totalComm = 0;
            double totalPay = 0;
            double totalBonus = 0;
            double total = 0;

            Console.Write("Enter Number of Sales Persons You Would Like to Calculate For: ");
            numOfPersons = Convert.ToInt32(Console.ReadLine());
            Console.Write("\n");

            for(int x = 0; x < numOfPersons; x++)
            {
                Console.Write("Enter Sales Person First Name: ");
                firstName = Console.ReadLine();
                Console.Write("Enter Employee Sales Comission Level (1=Low / 4=High): ");
                try
                {
                    level = Convert.ToInt32(Console.ReadLine());
                }
                catch 
                {
                    level = 0;
                    Console.WriteLine("Invalid entry, please try again.");
                }
                Console.Write("Enter Weekly Hours Worked: ");
                hoursWorked = Convert.ToDouble(Console.ReadLine());
                Console.Write("Enter Sales Amount (in USD): ");
                amount = Convert.ToDouble(Console.ReadLine());
                Console.Write("\n");

                SalesPerson newPerson = new SalesPerson(x+1, firstName, level, amount, hoursWorked);
                SalePersons.Add(newPerson.id, newPerson);

                totalComm += newPerson.CalculateCommission();
                totalPay += newPerson.CalculatePay();
                totalBonus += newPerson.CalculateBonus();
            }
            total = totalComm + totalPay + totalBonus;
            Console.WriteLine("Total Commission: {0:C}", totalComm);
            Console.WriteLine("Total Wage Pay: {0:C}", totalPay);
            Console.WriteLine("Total Bonus: {0:C}", totalBonus);
            Console.WriteLine("Total Combined: {0:C}\n", total);
            
            
            foreach(SalesPerson person in SalePersons.Values)
            {
                Console.WriteLine(person);
            }

            Console.ReadKey();
        }
    }
}
