using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IconParsing
{
    class ImageParse
    {
        #region "Fields"

        private int spacing;
        private Bitmap image;

        #endregion

        #region "Constructors"

        public ImageParse(int spacing, Bitmap image)
        {
            this.spacing = spacing;
            this.image = image;
        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"

        public Bitmap GetBitmap(int index)
        {
            int w = image.Height;
            int h = image.Height;

            Bitmap bitmap = new Bitmap(w, h);
            for (int x = 0; x < w; x++)
            {
                for (int y = 0; y < h; y++)
                {
                    bitmap.SetPixel(x, y, image.GetPixel(index * w + spacing + x, y));
                }
            }

            return bitmap;
        }

        public Icon GetIcon(int index)
        {
            return Icon.FromHandle(GetBitmap(index).GetHicon());
        }

        #endregion

        #region "Abstract/Virtual Methods"



        #endregion

        #region "Inherited Methods"



        #endregion

        #region "Static Methods"



        #endregion

        #region "Operators"



        #endregion
    }
}
