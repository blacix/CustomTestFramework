using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace primeService
{
    class Program
    {
        static void Main(string[] args)
        {
            int portArg = AsynchronousSocketListener.DEFAULT_PORT;
            if(args.Count() == 0)
            {
                AsynchronousSocketListener.StartListening(false, portArg);
            }
            else if(args.Count() ==  1)
            {
                portArg = Int32.Parse(args[0]);
                AsynchronousSocketListener.StartListening(true, portArg);
            }                
            else if(args.Count() > 1)
            {
                throw new Exception();
            }
            
            Console.WriteLine("press any key to extit");
            Console.Read();
        }
    }
}
