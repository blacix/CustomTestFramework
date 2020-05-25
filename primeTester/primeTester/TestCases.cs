using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using primeTester.utils;

namespace primeTester
{
    /// <summary>
    /// class that defines and executes test cases on the primeServer server
    /// </summary>
    class TestCases
    {
        // the TCP port of the primeServer server
        // set this to the same value as in primeServer config file or program argument
        public const int SERVER_PORT = 55555;


        //////////////////////////////////////////////////////////////////
        // DEFINE TEST CASE FUNCTIONS HERE
        // EACH FUNCTION DEFINES ONE TEST CASE
        //////////////////////////////////////////////////////////////////

        /// <summary>
        /// tests primeServer availability
        /// </summary>
        /// <returns>true on successful connection establishment</returns>
        public bool PrimeServerAvailability()
        {
            // the test result
            bool result = true;

            // create a test value: 35 or 36
            Random random = new Random();            
            int testValue = 35 + random.Next(2);

            // send the request to the server using ServerConnection.Send() function
            string request = testValue.ToString();
            
            // send the request and receive response
            Response response = ServerConnection.Send(request);

            // process response and determine test result
            if(response == Response.CONNECTION_ERROR
                || response == Response.UNKNOWN_ERROR)
            {
                result = false;
            }

            // reutrn test result
            return result;
        }


        /// <summary>
        /// Another Test Case
        /// </summary>
        /// <returns>true if passed</returns>
        public bool ExampleTestCase()
        {
            // use of IsPrime function
            bool prime = IsPrime(11);
            return false;
        }



        //////////////////////////////////////////////////////////////////
        // SELECT AND EXECUTE TEST CASES HERE
        //////////////////////////////////////////////////////////////////
        static void Main(string[] args)
        {
            TestCases testCases = new TestCases();

            // add test case functions as follows
            TestExecutor.AddTestCase(testCases.PrimeServerAvailability, "Prime Server Availability");
            TestExecutor.AddTestCase(testCases.PrimeServerAvailability, "Prime Server Availability");
            TestExecutor.AddTestCase(testCases.ExampleTestCase, "Some test case");

            // execute test cases
            TestExecutor.ExecuteTestCases();
        }


        /// <summary>
        /// function to decide if the number provided as parameter is prime or not
        /// </summary>
        /// <param name="number"></param>
        /// <returns>true if number is prime</returns>
        public static bool IsPrime(int number)
        {
            if (number == 0 || number == 1)
                return false;

            bool isPrime = true;
            for (int i = 2; i < number - 1; i++)
            {
                if (number % i == 0) // found divisor, not a prime number
                    isPrime = false;
            }

            return isPrime;
        }
    }
}
