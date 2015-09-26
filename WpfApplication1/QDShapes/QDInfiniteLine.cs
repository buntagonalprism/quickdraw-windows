using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    class QDInfiniteLine
    {
        public List<float> coeffs = new List<float>();

        public QDInfiniteLine(QDPoint pt, float angleD) {
            calcCoeffs(pt, angleD);
        }
        public QDInfiniteLine(QDPoint pt1, QDPoint pt2)
        {
            float angleR = (float) Math.Atan2(pt1.y - pt2.y, pt1.x - pt2.x);
            calcCoeffs(pt1, (float) QDUtils.QDUtils.toDegrees(angleR));
        }

        private void calcCoeffs(QDPoint pt, float angleD)
        {
            float sinA = (float)Math.Sin(QDUtils.QDUtils.toRadians(angleD));
            float cosA = (float)Math.Cos(QDUtils.QDUtils.toRadians(angleD));
            coeffs.Add(sinA);
            coeffs.Add(-cosA);
            coeffs.Add(cosA*pt.y - sinA*pt.x);
        }

        public float getDistToPoint(QDPoint pt)
        {
            float a = this.coeffs[0];
            float b = this.coeffs[1];
            float c = this.coeffs[2];
            return (float) (Math.Abs(a * pt.x + b * pt.y + c) / Math.Sqrt(a*a + b*b));
        }

        public QDPoint intersect(QDInfiniteLine other) {
            float a,b,c,j,k,l;
            a = this.coeffs[0];
            b = this.coeffs[1];
            c = this.coeffs[2];
            j = other.coeffs[0];
            k = other.coeffs[1];
            l = other.coeffs[2];
            // TODO: Check for parallel lines
            // Simultaneous solution to lines in general form
            return new QDPoint((c*k - b*l)/(b*j-a*k),(a*l-c*j)/(b*j-a*k));
        }
    }
}
