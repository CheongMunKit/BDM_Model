using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Vision.View.ImagePanel
{
    public class ResizeUserControl
    {
        public void Execute(FrameworkElement displayImageGrid, double imageHeight, double imageWidth, double zoomFactor)
        {
            double newHeight = imageHeight * zoomFactor;
            double newWidth = imageWidth * zoomFactor;

            if (newHeight == 0 && newWidth == 0)
            {
                newHeight = imageHeight;
                newWidth = imageWidth;
            }

            else
            {
                displayImageGrid.Height = (int)newHeight;
                displayImageGrid.Width = (int)newWidth;
            }

        }

        public void Execute(System.Windows.Forms.Panel mainPanel, int imageHeight, int imageWidth, float zoomFactor)
        {
            double newHeight = imageHeight * zoomFactor;
            double newWidth = imageWidth * zoomFactor;

            if (newHeight == 0 && newWidth == 0)
            {
                newHeight = imageHeight;
                newWidth = imageWidth;
            }

            else
            {
                mainPanel.Height = (int)newHeight;
                mainPanel.Width = (int)newWidth;
            }
        }

        public void Execute(System.Windows.Forms.PictureBox PictureBox, double imageHeight, double imageWidth, double zoomFactor)
        {
            double newHeight = imageHeight * zoomFactor;
            double newWidth = imageWidth * zoomFactor;

            if (newHeight == 0 && newWidth == 0)
            {
                newHeight = imageHeight;
                newWidth = imageWidth;
            }

            else
            {
                PictureBox.Height = (int)newHeight;
                PictureBox.Width = (int)newWidth;
            }
        }

        public void Execute(Border displayImageBorder, double imageHeight, double imageWidth, double zoomFactor)
        {
            double newHeight = imageHeight * zoomFactor;
            double newWidth = imageWidth * zoomFactor;

            if (newHeight == 0 && newWidth == 0)
            {
                newHeight = imageHeight;
                newWidth = imageWidth;
            }

            else
            {

                displayImageBorder.Height = (int)newHeight;
                displayImageBorder.Width = (int)newWidth;
            }

        }
    }
}
