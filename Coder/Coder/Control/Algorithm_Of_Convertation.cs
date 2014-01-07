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
            Color pixel;
            BitArray pixelValue;

            //Message
            BitArray message = new BitArray(Encoding.ASCII.GetBytes(obj.message));

            {
                long imageSize = obj.image_With_Info.Width * obj.image_With_Info.Height;
                int infoSize = 8 + 40 + message.Length;

                if (imageSize < infoSize)
                {
                    throw new Exception("Image is too small");
                }
            }

            //Color component length is 8 bits
            BitArray colorComponent = new BitArray(Encoding.ASCII.GetBytes(obj.ColorComponent.ToString()));
            Console.WriteLine("Color Component: {0}", colorComponent.Length);

            //Message length (max value is 40 bits)
            BitArray messageLength = new BitArray(Encoding.ASCII.GetBytes(message.Length.ToString()));

            //Creating array with fixed size
            BitArray EncriptedData = new BitArray(8 + 40 + message.Length);
            Console.WriteLine("Array Created");

            //Merging data in one wariable
            
            int i = 0;
            int g = 0;
                
            //Color component
            while (g < colorComponent.Length)
            {
                EncriptedData[g] = colorComponent[g];
                g++;
            }
            Console.WriteLine("Color component merged");
                
            //Message length
            g = 8;
            i = 0;
            while (i < messageLength.Length)
            {
                EncriptedData[g] = messageLength[i];
                g++; i++;
            }
            Console.WriteLine("MessageLength merged");
            
            //Message
            g = 40;
            i = 0;
            while (i < message.Length)
            {
                EncriptedData[g] = message[i];
                g++; i++;
            }
            Console.WriteLine("Message merged");
            

            //Calculating amount of rows and cols used for information
            int cals = EncriptedData.Length % obj.image_With_Info.Width;
            int rows = EncriptedData.Length / obj.image_With_Info.Width;

            //Inserting information into picture
            i = 0; // Rows
            int j = 0; // Cals
            StringBuilder sb = new StringBuilder(); // TO debug
            Console.WriteLine("Inserting information");
            do 
            {
                j = 0;
                do
                {
                    pixel = obj.image_With_Info.GetPixel(j, i);

                    pixelValue = new BitArray(Encoding.ASCII.GetBytes(pixel.R.ToString()));

                    sb.Append("Pixel " + (obj.image_With_Info.Width * i + j) + " : " + pixelValue[0] + "\n"); // TO DEBUG
                    //sb.Append("Pixel cal=" + j + ", row=" + i + " : " + pixelValue[0] + "\n"); // TO DEBUG

                    pixelValue[0] = EncriptedData[obj.image_With_Info.Width * i + j];

                    j++;
                } while (j < cals);
                i++;
            }while (i < rows);

            System.IO.File.WriteAllText(@"d:\WriteText.txt", sb.ToString()); // TO DEBUG

            //for (int item = message.Length - 1; item >= 0; item--)
            //{
            //    Console.Write(Convert.ToInt32(message[item]));
            //}

            Console.WriteLine();
        }

    }
}
