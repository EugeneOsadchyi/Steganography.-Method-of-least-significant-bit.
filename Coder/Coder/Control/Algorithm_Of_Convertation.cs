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

        //***********************************************************************************************************************************************************//
        //********************************************************    Main method of inserting informaion    ********************************************************//
        //***********************************************************************************************************************************************************//

        public static Bitmap encodeInformation(ref Data_Image_Message obj)
        {
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
            BitArray encodedData = new BitArray(8 + 40 + message.Length);
            Console.WriteLine("Encripted Data Length is {0}", encodedData.Length);
            Console.WriteLine("Array Created");

            //Merging data in one wariable

            //Color component
            collectEncodedData(colorComponent, encodedData, 0, 0);
            Console.WriteLine("Color component merged");

            //Message length
            collectEncodedData(messageLength, encodedData, 8, 0);
            Console.WriteLine("MessageLength merged");

            //Message
            collectEncodedData(message, encodedData, 48, 0);
            Console.WriteLine("Message merged");

            StringBuilder sb = new StringBuilder(); // TO debug
            Console.WriteLine("Inserting information");


            // Processing the integration
            processIntegrating(obj.image_With_Info, encodedData, sb);


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


        //***********************************************************************************************************************************************************//
        //********************************************************    Main method of getting informaion    ********************************************************//
        //***********************************************************************************************************************************************************//
        public static void decodeInformation(Bitmap image)
        {
            StringBuilder sb = new StringBuilder();
            BitArray colorComponent = new BitArray(8);
            byte[] colorComponentBytes;

            colorComponentBytes = processDecodding(image, colorComponent, 0, 0, 0, 1, sb);

            Console.WriteLine("colorComponent is {0}", Encoding.ASCII.GetString(colorComponentBytes));


            BitArray messageLength = new BitArray(40);
            byte[] messageLengthBytes;

            messageLengthBytes = processDecodding(image, messageLength, 0, 8, 8, 5, sb);
            Console.WriteLine("messageLength is {0}", Encoding.ASCII.GetString(messageLengthBytes));

            BitArray message = new BitArray(int.Parse(Encoding.ASCII.GetString(messageLengthBytes)));
            byte[] messageBytes;

            messageBytes = processDecodding(image, message, 0, 48, 48, (int.Parse(Encoding.ASCII.GetString(messageLengthBytes)) / 8), sb);

            Console.WriteLine("Encripted Data Length is {0}", message.Length + messageLength.Length + colorComponent.Length);

            Console.WriteLine("Resultive message length (STRING): {0}", Encoding.ASCII.GetString(messageBytes).Length);
            Console.WriteLine(Encoding.ASCII.GetString(messageBytes));

            System.IO.File.WriteAllText(@"d:\Decode.txt", sb.ToString()); // TO DEBUG
        }




        //***********************************************************************************************************************************************************//
        //******************************    Each part of data (color component, messageLength and message) is merging in one array    *******************************//
        //***********************************************************************************************************************************************************//
        private static void collectEncodedData(BitArray data, BitArray encodedData, int encDataIterator, int dataIterator)
        {

            //*********************************************************************************//
            //***** EncriptedData - data array                 encDataIterator - iterator *****//
            //***** data          - parts of data              dataIterator    - iterator *****//
            //*********************************************************************************//

            while (dataIterator < data.Length)
            {
                encodedData[encDataIterator] = data[dataIterator];
                encDataIterator++;
                dataIterator++;
            }
        }


        
        //***********************************************************************************************************************************************************//
        //**********************************************    Going througth the picture and setting the information in it    *****************************************//
        //***********************************************************************************************************************************************************//
        private static void processIntegrating(Bitmap image, BitArray EncriptedData, StringBuilder sb)
        {
            // Here processes the calculatings of used space of picture for the information

            // Calculating amount of rows and cols used for information
            int cals = EncriptedData.Length % image.Width;
            int rows = EncriptedData.Length / image.Width;

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
                    setPixel(image, EncriptedData, i, j, sb);
                    j++;
                } while (j < image.Width);
                i++;
            }

            // Not full col
            if (i == rows)
            {
                j = 0;
                do
                {
                    setPixel(image, EncriptedData, rows, j, sb);
                    j++;
                } while (j < cals);
            }
        }



        //***********************************************************************************************************************************************************//
        //********************************************    Going througth the picture and getting the information from it    *****************************************//
        //***********************************************************************************************************************************************************//
        private static byte[] processDecodding(Bitmap image, BitArray data, int i, int j, int offset, int resultLength, StringBuilder sb)
        {
            BitArray pixelValue = null;

            byte[] decodedDataBytes = new byte[resultLength];

            int cals = (data.Length + offset) % image.Width;
            int rows = (data.Length + offset) / image.Width;

            i = 0;
            j = offset;


            while (i < rows)
            {
                do
                {
                    getPixel(image, ref pixelValue, i, j, data, offset, sb);
                    j++;
                } while (j < image.Width);

                j = 0;
                i++;
            }
            if (i == rows)
            {
                j = offset;
                do
                {
                    getPixel(image, ref pixelValue, rows, j, data, offset, sb);
                    j++;
                } while (j < cals);
            }

            data.CopyTo(decodedDataBytes, 0);
            return decodedDataBytes;
        }



        //***********************************************************************************************************************************************************//
        //******************************************************************    Setting  pixels    ******************************************************************//
        //***********************************************************************************************************************************************************//
        private static void setPixel(Bitmap image, BitArray EncriptedData, int i, int j, StringBuilder sb)
        {
            // Here generated information is setting into image pixels

            Color pixel;
            BitArray pixelValue;

            pixel = image.GetPixel(j, i);

            pixelValue = new BitArray(Encoding.ASCII.GetBytes(pixel.R.ToString()));

            pixelValue[0] = EncriptedData[image.Width * i + j];
            
            //sb.Append("Pixel " + (obj.image_With_Info.Width * i + j) + " : " + pixelValue[0] + "\n"); // Logging TO DEBUG

            byte[] new_value = new byte[pixelValue.Length];

            pixelValue.CopyTo(new_value, 0);

            image.SetPixel(j, i, Color.FromArgb(int.Parse(Encoding.ASCII.GetString(new_value)), pixel.G, pixel.B));
        }


        //***********************************************************************************************************************************************************//
        //******************************************************************    Getting  pixels    ******************************************************************//
        //***********************************************************************************************************************************************************//
        private static void getPixel(Bitmap image, ref BitArray pixelValue, int i, int j, BitArray message, int offset, StringBuilder sb)
        {
            // Processing getting bits of information from pixels

            pixelValue = new BitArray(Encoding.ASCII.GetBytes(image.GetPixel(j, i).R.ToString()));
            message.Set(image.Width * i + j - offset, pixelValue[0]);

            // sb.Append("Pixel " + (image.Width * i + j) + " : " + pixelValue[0] + "\n"); // TO DEBUG
        }

    }
}
