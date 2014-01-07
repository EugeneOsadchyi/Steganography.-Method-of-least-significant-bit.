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
        private Bitmap image;
        public char ColorComponent { get; set; }

        public Data_Image_Message(string path, string msg, char component)
        {
            image_With_Info = new Bitmap(path);
            message = msg;
            ColorComponent = component;
        }
        
        public Bitmap image_With_Info
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }

        public String message { get; set; }
        public void SaveImage(string path)
        {
            image_With_Info.Save(path);
        }
    }
}
