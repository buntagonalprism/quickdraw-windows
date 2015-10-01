using WpfApplication1.QDUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;


namespace WpfApplication1.QDShapes
{
    public class QDLine : QDShape
    {
        public QDPoint start = new QDPoint();
        public QDPoint finish = new QDPoint();
        public QDPoint midQDPoint = null;
        public Boolean vertical = false;
        public float intercept;
        // TODO storing both is probably bad practice, radians is better computationally, but degrees
        // is better for understanding in terms of angle values
        public float angleD = 181.0f;
        public float angleR = 1.0f+(float)Math.PI;
        public float length = -1.0f;


        private float toDegrees(float rad)
        {
            return 180.0f * rad / (float)Math.PI;
        }

        // Empty default constructor
        public QDLine(){}

        public QDLine(QDPoint start_in, QDPoint finish_in) {
            start = start_in;
            finish = finish_in;
            getMidpoint();
            length = (float) Math.Pow(Math.Pow(start.x - finish.x,2.0f) + Math.Pow(start.y - finish.y, 2.0f),0.5f);
            angleR = (float) Math.Atan2(finish.y - start.y, finish.x - start.x);
            angleD = (float) toDegrees(angleR);
            //path.moveTo(start.x, start.y);
            //path.QDLineTo(finish.x,finish.y);
        }

        public float getLength() {
            if (length < 0.0f) {
                length = (float) Math.Pow(Math.Pow(start.x - finish.x,2.0f) + Math.Pow(start.y - finish.y, 2.0f),0.5f);
            }
            return length;
        }

        //@Override
        public override List<SampledQDPoint> getIntermediatePoints(float spacing) {
            List<SampledQDPoint> intermediateQDPoints = new List<SampledQDPoint>();
            intermediateQDPoints.Add(new SampledQDPoint(start, QDPointTypes.LINE_ENDPOINT));
            intermediateQDPoints.Add(new SampledQDPoint(finish, QDPointTypes.LINE_ENDPOINT));
            if (getLength() > spacing) {
                int numPts = (int) Math.Ceiling(getLength() / spacing);
                float betweenPts = getLength() / numPts;
                for (int i = 1; i < numPts ; i++) {
                    QDPoint pt = new QDPoint(start.x + (float) Math.Cos(angleR)*i*betweenPts, start.y + (float) Math.Sin(angleR)*i*betweenPts);
                    intermediateQDPoints.Add(new SampledQDPoint(pt, QDPointTypes.LINE_INTERMEDIATE));
                }
            }
            return intermediateQDPoints;
        }

        public float getPerpAngleD() {
            getAngleD();
            if (angleD > 0.0f)
                return angleD - 90.0f;
            else
                return angleD + 90.0f;

        }

        public float getAngleD() {
            if (angleD > 180.0f) {
                angleR = (float) Math.Atan2(finish.y - start.y, finish.x - start.x);
                angleD = (float) toDegrees(angleR);
            }
            return angleD;
        }

        public float getAngleR() {
            getAngleD();
            return angleR;
        }

        public QDPoint getMidpoint() {
            if (midQDPoint == null)
                midQDPoint = new QDPoint((start.x + finish.x)/2.0f, (start.y + finish.y)/2.0f);
            return midQDPoint;
        }

        public override Path getPath() {
            if (path.Data == null) {
                PathFigure myPathFigure = new PathFigure();
                myPathFigure.StartPoint = new Point(start.x, start.y);

                LineSegment myLineSegment = new LineSegment();
                myLineSegment.Point = new Point(finish.x, finish.y);

                PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
                myPathSegmentCollection.Add(myLineSegment);

                myPathFigure.Segments = myPathSegmentCollection;

                PathFigureCollection myPathFigureCollection = new PathFigureCollection();
                myPathFigureCollection.Add(myPathFigure);

                PathGeometry myPathGeometry = new PathGeometry();
                myPathGeometry.Figures = myPathFigureCollection;
               
                path.Stroke = Brushes.Black;
                path.StrokeThickness = 1;
                path.Data = myPathGeometry;
               
            }
            return path;
        }

        //public Path getPath(QDPoint windowOrigin, float scaleFactor) {

        //    QDPoint startLoc = absToLocCoords(start, scaleFactor, windowOrigin);
        //    QDPoint finishLoc = absToLocCoords(finish, scaleFactor, windowOrigin);

        //    path.reset();

        //    path.moveTo(startLoc.x, startLoc.y);
        //    path.QDLineTo(finishLoc.x,finishLoc.y);

        //    return path;
        //}

        //public void addToPath(Path path) {
        //    path.moveTo(start.x, start.y);
        //    path.QDLineTo(finish.x, finish.y);
        //}


        public override String getSvgString()
        {
            //String outString = String.format("<QDLine x1=\"%.2f\" y1=\"%.2f\" x2=\"%.2f\" y2=\"%.2f\" stroke=\"rgb(%d, %d, %d)\" stroke-width=\"%.1f\"/>\n",
            //        start.x, start.y, finish.x, finish.y, Color.red(paint.getColor()), Color.green(paint.getColor()), Color.blue(paint.getColor()), paint.getStrokeWidth());
            //return outString;
            return "";
        }

    }
}
