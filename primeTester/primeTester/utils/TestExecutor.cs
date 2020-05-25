using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace primeTester.utils
{
    public class TestExecutor
    {
        public delegate bool TestCaseFunction();
        static List<KeyValuePair<TestCaseFunction, string>> TestCaseDelegates = new List<KeyValuePair<TestCaseFunction, string>>();
        public static void ExecuteTestCases()
        {
            Console.WriteLine("-------------------------");
            Console.WriteLine("Executing {0} test cases...", TestCaseDelegates.Count);
            Console.WriteLine("-------------------------");
            Console.WriteLine();
            int passed = 0;
            for (int i = 0; i < TestCaseDelegates.Count; i++)
            {
                DateTime start = DateTime.Now;                
                KeyValuePair<TestCaseFunction, string> entry = TestCaseDelegates.ElementAt(i);
                Console.WriteLine("Executing: " + entry.Value);
                try
                {
                    bool result = entry.Key();
                    if (result)
                        passed++;
                    Console.WriteLine(" Result: " + result.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine(" ERROR: failed to execute: " + entry.Value);
                }                
                
                TimeSpan timeItTook = DateTime.Now - start;
                Console.WriteLine(" Duration: " + timeItTook.TotalMilliseconds + " ms");
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine("Finished");
            Console.WriteLine("passed: {0}/{1}", passed, TestCaseDelegates.Count);
            TestCaseDelegates.Clear();
            Console.WriteLine("Press enter to continue");
            Console.Read();
        }

        public static void AddTestCase(TestCaseFunction f, string title)
        {
            TestCaseDelegates.Add(new KeyValuePair<TestCaseFunction, string >( f, title));
        }
    }
}
