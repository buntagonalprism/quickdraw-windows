using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.QDShapes
{
    public class QDPoint
    {
        public float x;
        public float y;
        public QDPoint() {}
        public QDPoint(QDPoint other) {
            this.x = other.x;
            this.y = other.y;
        }
        public QDPoint(float x_in, float y_in)
        {
            x = x_in;
            y = y_in;
        }
    }
}
