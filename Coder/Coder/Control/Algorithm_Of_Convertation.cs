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
        private static void buildEncryptedDataArray(BitArray data, BitArray EncriptedData, int encDataIterator, int dataIterator)
        {
            // Here information about each part of data (color component, messageLength and message) is merging in one array

            //*********************************************************************************//
            //***** EncriptedData - data array                 encDataIterator - iterator *****//
            //***** data          - parts of data              dataIterator    - iterator *****//
            //*********************************************************************************//

            while (dataIterator < data.Length)
            {
                EncriptedData[encDataIterator] = data[dataIterator];
                encDataIterator++;
                dataIterator++;
            }
        }
        private static void setting(Data_Image_Message obj, BitArray EncriptedData, int i, int j, StringBuilder sb)
        {
            // Here generated information is setting into image pixels

            Color pixel;
            BitArray pixelValue;

            pixel = obj.image_With_Info.GetPixel(j, i);

            pixelValue = new BitArray(Encoding.ASCII.GetBytes(pixel.R.ToString()));
            
            pixelValue[0] = EncriptedData[obj.image_With_Info.Width * i + j];
            
            //sb.Append("Pixel " + (obj.image_With_Info.Width * i + j) + " : " + pixelValue[0] + "\n"); // Logging TO DEBUG

            byte[] new_value = new byte[pixelValue.Length];

            pixelValue.CopyTo(new_value, 0);

            obj.image_With_Info.SetPixel(j, i, Color.FromArgb(int.Parse(Encoding.ASCII.GetString(new_value)), pixel.G, pixel.B));
        }

        private static void processInsertingInfo(Data_Image_Message obj, BitArray EncriptedData, StringBuilder sb)
        {
            // Here processes the calculatings of used space of picture for the information

            // Calculating amount of rows and cols used for information
            int cals = EncriptedData.Length % obj.image_With_Info.Width;
            int rows = EncriptedData.Length / obj.image_With_Info.Width;

            // Inserting information into picture
            int i = 0; // Rows
            int j = 0; // Cals

            // Iterating through the picture's pixels and inserting hidden information

            // Full cols (image width)
            while (i < rows)
            {
                j = 0;
                do
                {
                    setting(obj, EncriptedData, i, j, sb);
                    j++;
                } while (j < obj.image_With_Info.Width);
                i++;
            }

            // Not full col
            if (i == rows)
            {
                j = 0;
                do
                {
                    setting(obj, EncriptedData, rows, j, sb);
                    j++;
                } while (j < cals);
            }
        }
        private static void getting(Bitmap obj, ref BitArray pixelValue, int i, int j, BitArray message, int offset, StringBuilder sb)
        {
            // Processing getting bits of information from pixels

            pixelValue = new BitArray(Encoding.ASCII.GetBytes(obj.GetPixel(j, i).R.ToString()));
            message.Set(obj.Width * i + j - offset, pixelValue[0]);
            sb.Append("Pixel " + (obj.Width * i + j) + " : " + pixelValue[0] + "\n"); // TO DEBUG
        }

        public static Bitmap integrateInformation(ref Data_Image_Message obj)
        {
            // Main method of inserting informaion


            // Message to Bit Array
            BitArray message = new BitArray(Encoding.ASCII.GetBytes(obj.message));
            
            Console.WriteLine("Message length is: {0}", message.Length);

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
            Console.WriteLine("messageLength Length: {0}", messageLength.Length);

            //Creating array with fixed size
            BitArray EncriptedData = new BitArray(8 + 40 + message.Length);
            Console.WriteLine("Encripted Data Length is {0}", EncriptedData.Length);
            Console.WriteLine("Array Created");

            //Merging data in one wariable

            //Color component
            buildEncryptedDataArray(colorComponent, EncriptedData, 0, 0);
            Console.WriteLine("Color component merged");

            //Message length
            buildEncryptedDataArray(messageLength, EncriptedData, 8, 0);
            Console.WriteLine("MessageLength merged");

            //Message
            buildEncryptedDataArray(message, EncriptedData, 48, 0);
            Console.WriteLine("Message merged");

            StringBuilder sb = new StringBuilder(); // TO debug
            Console.WriteLine("Inserting information");


            // Processing the integration
            processInsertingInfo(obj, EncriptedData, sb);


            Console.WriteLine("Resultive message length (STRING): {0}", obj.message.Length);
            
            //System.IO.File.WriteAllText(@"d:\Encode.txt", sb.ToString()); // TO DEBUG


            // TO DEBUG
            //for (int item = message.Length - 1; item >= 0; item--)
            //{
            //    Console.Write(Convert.ToInt32(message[item]));
            //}

            Console.WriteLine();

            Console.WriteLine();
            return obj.image_With_Info;
        }





        

        public static void getInformation(Bitmap obj)
        {
            StringBuilder sb = new StringBuilder();
            BitArray colorComponent = new BitArray(8);
            byte[] colorComponentBytes;

            colorComponentBytes = processGettingData(obj, colorComponent, 0, 0, 0, 1, sb);

            Console.WriteLine("colorComponent is {0}", Encoding.ASCII.GetString(colorComponentBytes));


            BitArray messageLength = new BitArray(40);
            byte[] messageLengthBytes;

            messageLengthBytes = processGettingData(obj, messageLength, 0, 8, 8, 5, sb);
            Console.WriteLine("messageLength is {0}", Encoding.ASCII.GetString(messageLengthBytes));
            
            BitArray message = new BitArray(int.Parse(Encoding.ASCII.GetString(messageLengthBytes)));
            byte[] messageBytes;

            messageBytes = processGettingData(obj, message, 0, 48, 48, (int.Parse(Encoding.ASCII.GetString(messageLengthBytes)) / 8), sb);

            Console.WriteLine("Encripted Data Length is {0}", message.Length + messageLength.Length + colorComponent.Length);

            Console.WriteLine("Resultive message length (STRING): {0}", Encoding.ASCII.GetString(messageBytes).Length);
            Console.WriteLine(Encoding.ASCII.GetString(messageBytes));

            System.IO.File.WriteAllText(@"d:\Decode.txt", sb.ToString()); // TO DEBUG
        }

        private static byte[] processGettingData(Bitmap obj, BitArray incommingData, int i, int j, int offset, int resultLength, StringBuilder sb)
        {
            BitArray pixelValue = null;

            byte[] decodedDataBytes = new byte[resultLength];

            int cals = (incommingData.Length + offset) % obj.Width;
            int rows = (incommingData.Length + offset) / obj.Width;

            i = 0;
            j = offset;


            while (i < rows)
            {
                do
                {
                    getting(obj, ref pixelValue, i, j, incommingData, offset, sb);
                    j++;
                } while (j < obj.Width);

                j = 0;
                i++;
            }
            if (i == rows)
            {
                j =  offset;
                do
                {
                    getting(obj, ref pixelValue, rows, j, incommingData, offset, sb);
                    j++;
                } while (j < cals);
            }
            incommingData.CopyTo(decodedDataBytes, 0);
            return decodedDataBytes;
        }


    }
}
