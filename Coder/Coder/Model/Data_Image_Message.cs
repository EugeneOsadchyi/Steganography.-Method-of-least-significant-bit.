using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Drawing;

namespace Coder
{
    class Data_Image_Message
    {
        private Bitmap imageWoInf;
        private Bitmap imageWiInf;

        public Data_Image_Message(string path, string msg)
        {
            image_Without_Info = new Bitmap(path);
            message = msg;
        }

        public Bitmap image_Without_Info
        {
            get
            {
                return imageWoInf;
            }
            set
            {
                imageWoInf = value;
            }
        }
        
        public Bitmap image_With_Info
        {
            get
            {
                return imageWiInf;
            }
            set
            {
                imageWiInf = value;
            }
        }

        public String message { get; set; }
    }
}
