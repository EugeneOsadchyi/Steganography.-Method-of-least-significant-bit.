using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using Coder.Control;

namespace Coder
{
    class Program
    {
        static void Main(string[] args)
        {
            Data_Image_Message obj = new Data_Image_Message("D:\\a.jpg", "Hello World");

            Algorithm_Of_Convertation.integrateInformation(ref obj);

            Console.WriteLine("Completed");
            Console.ReadLine();
        }
    }
}
