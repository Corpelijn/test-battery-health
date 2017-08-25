using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BatteryHistory
{
    class GraphRenderer
    {

        #region "Fields"

        private Bitmap bitmap;
        private TimeData[] data;
        private int totalImageWidth;
        private int startXPosition;

        #endregion

        #region "Constructors"

        public GraphRenderer(TimeData[] data, int width, int height)
        {
            bitmap = new Bitmap(width, height);
            this.data = data;
            totalImageWidth = (int)(bitmap.Width * 0.95f);
            startXPosition = (int)(bitmap.Width * 0.05f);
        }

        #endregion

        #region "Properties"



        #endregion

        #region "Methods"

        public Bitmap GenerateImage()
        {
            Graphics g = Graphics.FromImage(bitmap);

            DrawLegend(g);
            DrawBackground(g);
            DrawGrid(g);
            DrawPercentages(g);

            return bitmap;
        }

        public void DrawLegend(Graphics g)
        {
            int blockW = (int)(bitmap.Width * 0.025f);
            int blockH = (int)(bitmap.Height * 0.05f);
            Brush text = new SolidBrush(Color.Black);
            Font textF = new Font("Arial", 12);

            int lastXPosition = blockW;
            g.FillRectangle(new SolidBrush(Color.LightBlue), new Rectangle(lastXPosition, blockH / 2, blockW, blockH));
            lastXPosition += blockW;

            int textWidth = (int)g.MeasureString("Geen data", textF).Width;
            g.DrawString("Geen data", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;

            g.FillRectangle(new SolidBrush(Color.PaleGreen), new Rectangle(lastXPosition, blockH / 2, blockW, blockH));
            lastXPosition += blockW;

            textWidth = (int)g.MeasureString("Adapter ingestoken", textF).Width;
            g.DrawString("Adapter ingestoken", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;

            g.FillRectangle(new SolidBrush(Color.LimeGreen), new Rectangle(lastXPosition, blockH / 2, blockW, blockH));
            lastXPosition += blockW;

            textWidth = (int)g.MeasureString("Adapter ingestoken, opladen", textF).Width;
            g.DrawString("Adapter ingestoken, opladen", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;

            g.FillRectangle(new SolidBrush(Color.LightSalmon), new Rectangle(lastXPosition, blockH / 2, blockW, blockH));
            lastXPosition += blockW;

            textWidth = (int)g.MeasureString("Adapter niet ingestoken", textF).Width;
            g.DrawString("Adapter niet ingestoken", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;



            g.DrawLine(new Pen(Color.Black, 4), lastXPosition, blockH / 2, lastXPosition + blockW, blockH / 2);
            lastXPosition += blockW;

            textWidth = (int)g.MeasureString("Percentage", textF).Width;
            g.DrawString("Percentage", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;

            g.DrawLine(new Pen(Color.Red, 4), lastXPosition, blockH / 2, lastXPosition + blockW, blockH / 2);
            lastXPosition += blockW;

            textWidth = (int)g.MeasureString("Geen data", textF).Width;
            g.DrawString("Geen data", textF, text, new Point(lastXPosition, blockH / 2));
            lastXPosition += textWidth;
        }

        public void DrawBackground(Graphics g)
        {
            // Calcuate the amount of data samples per column
            float samplesPerColumn = (float)data.Length / (float)totalImageWidth;
            int blockWidth = totalImageWidth / data.Length;
            blockWidth = blockWidth < 1 ? 1 : blockWidth;

            float lastIndex = 0;
            int index = 0;
            while (lastIndex < data.Length && index < totalImageWidth)
            {
                int current = 0;

                float toIndex = lastIndex + samplesPerColumn;

                int dataCount = 0;
                for (int i = (int)lastIndex; i < toIndex; i++)
                {
                    if (i >= data.Length)
                        break;

                    if (data[i].Charger == PowerLineStatus.Online)
                    {
                        current++;
                        dataCount++;
                    }
                    else if (data[i].Charger == PowerLineStatus.Offline)
                    {
                        current--;
                        dataCount++;
                    }
                }

                Brush color;
                if (current > 0 && (data[(int)(samplesPerColumn * index)].Status & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging)
                    color = new SolidBrush(Color.LimeGreen);
                else if (current > 0)
                {
                    color = new SolidBrush(Color.PaleGreen);
                }
                else if (dataCount == 0)
                    color = new SolidBrush(Color.LightBlue);
                else
                {
                    color = new SolidBrush(Color.LightSalmon);
                }

                g.FillRectangle(color, new Rectangle(startXPosition + index, (int)(bitmap.Height * 0.09f), index + blockWidth, (int)(bitmap.Height * 0.8 + bitmap.Height * 0.02f)));

                index++;

                lastIndex = toIndex;
            }
        }

        public void DrawGrid(Graphics g)
        {
            Pen p = new Pen(Color.DimGray);
            p.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;

            int h = (int)(bitmap.Height * 0.1f);
            float totalHeight = bitmap.Height * 0.8f / 10;
            int stepWidth = totalImageWidth / 20;

            for (int i = 0; i <= 10; i++)
            {
                g.DrawLine(p, new Point(0, (int)(h + i * totalHeight)), new Point(bitmap.Width, (int)(h + i * totalHeight)));
                g.DrawString(((10 - i) * 10).ToString() + "%", new Font("Arial", 8), new SolidBrush(Color.DimGray), new Point(5, (int)(h + i * totalHeight + totalHeight / 10f)));
            }

            StringFormat format = new StringFormat();
            format.Alignment = StringAlignment.Center;
            Font f = new Font("Arial", 9);
            float textStep = (float)data.Length / 20f;

            for (int i = 0; i <= 20; i++)
            {
                g.DrawLine(p, new Point(startXPosition + i * stepWidth, (int)(bitmap.Height * 0.1f)), new Point(startXPosition + i * stepWidth, (int)(bitmap.Height * 0.9f)));
                if (i != 20)
                {
                    string text = data[(int)(i * textStep)].Time.ToString("dd-MM-yyyy\nHH:mm");
                    int half = (int)(g.MeasureString(text, f).Width / 2);
                    g.DrawString(text, f, new SolidBrush(Color.Black), new Point(startXPosition + i * stepWidth, (int)(bitmap.Height * 0.91f)), format);
                }
            }
        }

        public void DrawPercentages(Graphics g)
        {
            // Calcuate the amount of data samples per column
            float samplesPerColumn = (float)data.Length / (float)totalImageWidth;
            int blockWidth = totalImageWidth / data.Length;
            blockWidth = blockWidth < 1 ? 1 : blockWidth;

            float lastIndex = 0;
            int index = 0;
            Point lastPosition = new Point(0, 0);
            bool previousHadNoData = false;
            while (lastIndex < data.Length)
            {
                float toIndex = lastIndex + samplesPerColumn;

                int dataCount = 0;
                float totalData = 0;
                for (int i = (int)lastIndex; i < toIndex; i++)
                {
                    if (i >= data.Length)
                        break;

                    if (!data[i].NoData)
                    {
                        totalData += data[i].Percentage;
                        dataCount++;
                    }
                }

                if (dataCount == 0)
                {
                    index++;

                    lastIndex = toIndex;
                    previousHadNoData = true;
                    continue;
                }

                float avg = totalData / dataCount / 100f;
                int h = (int)((bitmap.Height * 0.8f) * avg);

                Point newP = new Point(startXPosition + index, (int)(bitmap.Height * 0.8f - h + bitmap.Height * 0.1f));
                Pen p = new Pen(previousHadNoData ? Color.Red : Color.Black, 3);
                if (!(lastPosition.X == 0 && lastPosition.Y == 0))
                {
                    g.DrawLine(p, lastPosition, newP);
                    previousHadNoData = false;
                }

                index++;

                lastIndex = toIndex;
                lastPosition = newP;
            }
        }


        // Draw a rotated string at a particular position.
        private void DrawRotatedTextAt(Graphics gr, float angle,
            string txt, Point position, Font the_font, Brush the_brush)
        {
            // Save the graphics state.
            GraphicsState state = gr.Save();
            gr.ResetTransform();

            // Rotate.
            gr.RotateTransform(angle);

            // Translate to desired position. Be sure to append
            // the rotation so it occurs after the rotation.
            gr.TranslateTransform(position.X, position.Y, MatrixOrder.Append);

            // Draw the text at the origin.
            gr.DrawString(txt, the_font, the_brush, 0, 0);

            // Restore the graphics state.
            gr.Restore(state);
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
