using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;
using System.Collections;

namespace Coder.Control
{
    class Algorithm_Of_Convertation
    {
        public static void integrateInformation(ref Data_Image_Message obj)
        {
            BitArray message = new BitArray(Encoding.ASCII.GetBytes(obj.message));
            BitArray messageLength = new BitArray(Encoding.ASCII.GetBytes(message.Length.ToString()));

            
            for (int item = message.Length-1; item>=0; item--)
            {
                Console.Write(Convert.ToInt32(message[item]));
            }
            Console.WriteLine();
        }
    }
}
