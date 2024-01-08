using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Converters;

namespace CampingUI.NewFolder
{
    public class Transform
    {

        public Transform(Canvas field, double desiredWidth, double desiredHeight, string fieldname) {
            double screenWidth = SystemParameters.PrimaryScreenWidth;
            double screenHeight = SystemParameters.PrimaryScreenHeight;

      

            double aspectRatio = desiredWidth / desiredHeight;
            double screenAspectRatio = screenWidth / screenHeight;

            double scaleX = 1.0;
            double scaleY = 1.0;


            if (screenAspectRatio > aspectRatio)
            {
                scaleX = screenWidth / desiredWidth / 2;
                scaleY = scaleX;
            }
            else
            {
                scaleY = screenHeight / desiredHeight / 2;
                scaleX = scaleY;
            }
            ApplyScaleTransform(scaleX, scaleY, field, fieldname);
        }
        private static void ApplyScaleTransform(double scaleX, double scaleY, Canvas field, string fieldname)
        {
            if (field.FindName(fieldname) is ScaleTransform plattegrond )
            {
                plattegrond.ScaleX = scaleX;
                plattegrond.ScaleY = scaleY;
            }
        }
    }
}
