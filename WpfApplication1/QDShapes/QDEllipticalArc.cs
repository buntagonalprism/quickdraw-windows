using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;
using System.Windows.Shapes;
using System.Windows.Media;
using System.Windows;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    class QDEllipticalArc : QDEllipse
    {
        //public Path.Direction mDir;
        SweepDirection direction = SweepDirection.Clockwise;
        public float mStartAngleD, mFinishAngleD;
        public Path path = new Path();

        // Empty default constructor
        public QDEllipticalArc() {}


        // Construct an elliptical arc as a segment of an existing ellipse object
        public QDEllipticalArc(QDEllipse parent, SweepDirection dir, float startAngle, float finishAngle)
        {
            constructFromFoci(parent.mFoci1, parent.mFoci2, parent.mFociToCirc);
            direction = dir;
            mStartAngleD = startAngle;
            mFinishAngleD = finishAngle;
        }

        // Create intermediate QDPoints by sweeping around a standard elliptical arc shape then manually
        // applying the rotation and translation to the QDPoint.
        //@Override
        public override List<SampledQDPoint> getIntermediatePoints(float spacing) {
            List<SampledQDPoint> pts = new List<SampledQDPoint>();
            // Get angle required to achieve desired QDPoint spacing at major axis extreme
            float angleStep = 2f * (float) Math.Atan(0.5f * spacing / mSemiX);

            // With a +ve CCW step, choose if we should sweep from start or finish to produce our arc
            float startSweep, finishSweep;
            if (direction == SweepDirection.Counterclockwise) { startSweep = QDUtils.QDUtils.degToRad(mStartAngleD); finishSweep = QDUtils.QDUtils.degToRad(mFinishAngleD); }
            else { startSweep = QDUtils.QDUtils.degToRad(mFinishAngleD); finishSweep = QDUtils.QDUtils.degToRad(mStartAngleD); }

            // Check whether the arc goes over the -pi barrier and adjust finish angle accordingly
            if (finishSweep < 0f && startSweep > finishSweep)
                finishSweep += 2.0f *(float) Math.PI;

            // Progress through the sweep
            for (float t = startSweep; t < finishSweep; t+= angleStep) {
                QDPoint parametric = new QDPoint(mSemiX * (float) Math.Cos(t), mSemiY * (float) Math.Sin(t));
                QDPoint rotatedPoint = new QDPoint((float) Math.Cos(mAngleR)*parametric.x - (float) Math.Sin(mAngleR)*parametric.y,
                        (float) Math.Sin(mAngleR)*parametric.x + (float) Math.Cos(mAngleR) * parametric.y);
                QDPoint translatedPoint = new QDPoint(rotatedPoint.x + mCentre.x, rotatedPoint.y + mCentre.y);
                SampledQDPoint sampled = new SampledQDPoint(translatedPoint, QDPointTypes.CIRCLE_CIRCUMFERENCE);
                pts.Add(sampled);
            }
            return pts;
        }


        //@Override
        public Path getPath() {
            if (path.Data == null)
            {
                PathFigure myPathFigure = new PathFigure();
                
                Point tempPt = new Point();
                Point startPt = new Point();
                Point finishPt = new Point();
                float mStartR = QDUtils.QDUtils.toRadians(mStartAngleD);
                float mFinishR = QDUtils.QDUtils.toRadians(mFinishAngleD);
                tempPt.X = mSemiX * Math.Cos(mStartR-mAngleR);
                tempPt.Y = mSemiY * Math.Sin(mStartR-mAngleR);
                startPt.X = (tempPt.X * Math.Cos(mAngleR) - tempPt.Y * Math.Sin(mAngleR)) + mCentre.x;
                startPt.Y = (tempPt.X * Math.Sin(mAngleR) + tempPt.Y * Math.Cos(mAngleR)) + mCentre.y;

                tempPt.X = mSemiX * Math.Cos(mFinishR - mAngleR);
                tempPt.Y = mSemiY * Math.Sin(mFinishR - mAngleR);
                finishPt.X = (tempPt.X * Math.Cos(mAngleR) - tempPt.Y * Math.Sin(mAngleR)) + mCentre.x;
                finishPt.Y = (tempPt.X * Math.Sin(mAngleR) + tempPt.Y * Math.Cos(mAngleR)) + mCentre.y;

                myPathFigure.StartPoint = startPt;

                ArcSegment myArcSegment = new ArcSegment();
                myArcSegment.Size = new Size(mSemiX, mSemiY);
                myArcSegment.Point = finishPt;
                float angleSweepD = 0.0f;
                
                if (direction == SweepDirection.Counterclockwise)
                {
                    angleSweepD = QDUtils.QDUtils.angleDiffCcwD(mStartAngleD, mFinishAngleD);
                }
                else if (direction == SweepDirection.Clockwise)
                {
                    angleSweepD = QDUtils.QDUtils.angleDiffCwD(mStartAngleD, mFinishAngleD);
                }
                if (angleSweepD > 180.0f)
                    myArcSegment.IsLargeArc = true;
                myArcSegment.RotationAngle = mAngleD;
                // Direction is calculated relative to top-left axis coordinate system
                // Direction is input to arc segment as visual appearance of direction, the opposite
                if (direction == SweepDirection.Clockwise)
                    myArcSegment.SweepDirection = SweepDirection.Counterclockwise;
                else
                    myArcSegment.SweepDirection = SweepDirection.Clockwise;
             
                //myArcSegment.IsLargeArc =
                //myArcSegment.RotationAngle = mAngleD;
                //myArcSegment.SweepDirection = SweepDirection.

                PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
                myPathSegmentCollection.Add(myArcSegment);

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


        // Draws an elliptical arc
        //@Override
        //public Path getPath(Point windowOrigin, float scaleFactor) {
        //    QDPoint centreLoc = absToLocCoords(mCentre, scaleFactor, windowOrigin);
        //    float semiXloc = mSemiX*scaleFactor;
        //    float semiYloc = mSemiY*scaleFactor;
        //    RectF boundLoc = new RectF( -semiXloc, -semiYloc, semiXloc, semiYloc);
        //    mRotMat.reset();
        //    mRotMat.postRotate(mAngleD);
        //    mRotMat.postTranslate(centreLoc.x, centreLoc.y);
        //    path.reset();
        //    if (mDir == Path.Direction.CCW) {
        //        path.addArc(boundLoc, mStartAngle - mAngleD, Utils.angleDiffCcwD(mStartAngle, mFinishAngle));
        //    }
        //    else {
        //        path.addArc(boundLoc, mStartAngle - mAngleD, -Utils.angleDiffCwD(mStartAngle, mFinishAngle));
        //    }
        //    path.transform(mRotMat);
        //    return path;
        //}

    }
}
