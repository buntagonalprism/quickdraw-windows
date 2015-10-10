using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;

namespace WpfApplication1.QDUtils
{
    public class QDDebugUtils
    {
        public static Ellipse drawDebugCircle(float x, float y, float radius, float strokeWidth, Color colour)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = radius;
            ellipse.Width = radius;
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = colour;
            ellipse.Stroke = brush;
            ellipse.StrokeThickness = strokeWidth;
            Matrix matrix = new Matrix();
            matrix.Translate(x, y);
            ellipse.RenderTransform = new MatrixTransform(matrix);
            return ellipse;
        }
    }
}
