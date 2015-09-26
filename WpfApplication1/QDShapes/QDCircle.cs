using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    class QDCircle
    {
        public QDPoint centre;
        public float radius;
        public float circumference = -1.0f;
        //public Path.Direction direction;


        public QDCircle() {}
        // TODO: non-default direction - what does it even do?
        public QDCircle(QDPoint centrePt, float radiusLength) {
            centre = centrePt;
            radius = radiusLength;
            //direction = Path.Direction.CCW;
        }

        //@Override
        public List<SampledQDPoint> getIntermediatePoints(float spacing) {
            List<SampledQDPoint> intermediatePoints = new List<SampledQDPoint>();
            intermediatePoints.Add(new SampledQDPoint(centre, QDPointTypes.CIRCLE_CENTRE));
            if (radius > spacing) {
                float angleR = 2.0f * (float) Math.Sin((spacing/2.0f)/radius);
                int numPts = (int) Math.Ceiling(2.0f * Math.PI / angleR);
                float betweenPts = 2.0f * (float) Math.PI  / numPts;
                for (int i = 1; i < numPts ; i++) {
                    QDPoint pt = new QDPoint(centre.x + (float) Math.Cos(-180.0f + i*betweenPts)*radius, centre.y + (float) Math.Sin(-180.0f + i*betweenPts)*radius);
                    intermediatePoints.Add(new SampledQDPoint(pt, QDPointTypes.CIRCLE_CIRCUMFERENCE));
                }
            }
            return intermediatePoints;
        }

        public float getCircumference() {
            if (circumference < 0.0f) {
                circumference = 2.0f* radius * (float) Math.PI;
            }
            return circumference;
        }

        //public Path getPath() {
        //    if (path.isEmpty()) {
        //        path.addCircle(centre.x, centre.y, radius, direction);
        //    }
        //    return path;
        //}

        //public Path getPath(Point windowOrigin, float scaleFactor) {
        //    Point centreLoc = absToLocCoords(centre, scaleFactor, windowOrigin);
        //    path.reset();
        //    path.addCircle(centreLoc.x, centreLoc.y, radius*scaleFactor, direction);
        //    return path;
        //}



        //public String getSvgString() {
        //    String outString = String.format("<circle cx = \"%.2f\" cy = \"%.2f\" r=\"%.2f\" fill=\"none\" stroke=\"rgb(%d, %d, %d)\" stroke-width=\"%.1f\"/>\n",
        //            centre.x, centre.y, radius, Color.red(paint.getColor()), Color.green(paint.getColor()), Color.blue(paint.getColor()), paint.getStrokeWidth());
        //    return outString;
        //}
    }
}
