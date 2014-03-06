using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Main
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] digits = { "zero", "one", "two", "three", "four", "five", 
            "six", "seven", "eight", "nine" };

            Console.WriteLine("Example that uses a lambda expression:");
            var shortDigits = digits.Where((digit, index) => digit.Length < index);
            foreach (var sD in shortDigits)
            {
                Console.WriteLine(sD);
            }

            // Compare the following code, which arrives at the same list of short
            // digits but takes more work to get there.
            Console.WriteLine("\nExample that uses a for loop:");
            List<string> shortDigits2 = new List<string>();
            for (var i = 0; i < digits.Length; i++)
            {
                if (digits[i].Length < i)
                    shortDigits2.Add(digits[i]);
            }

            foreach (var d in shortDigits2)
            {
                Console.WriteLine(d);
            }
        }
    }
}
