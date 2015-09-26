using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    class QDEllipticalArc : QDEllipse
    {
        //public Path.Direction mDir;
        bool ccwDirection = false;
        public float mStartAngle, mFinishAngle;

        // Empty default constructor
        public QDEllipticalArc() {}


        // Construct an elliptical arc as a segment of an existing ellipse object
        public QDEllipticalArc(QDEllipse parent, bool dir, float startAngle, float finishAngle)
        {
            constructFromFoci(parent.mFoci1, parent.mFoci2, parent.mFociToCirc);
            ccwDirection = dir;
            mStartAngle = startAngle;
            mFinishAngle = finishAngle;
        }

        // Create intermediate QDPoints by sweeping around a standard elliptical arc shape then manually
        // applying the rotation and translation to the QDPoint.
        //@Override
        public List<SampledQDPoint> getIntermediatePoints(float spacing) {
            List<SampledQDPoint> pts = new List<SampledQDPoint>();
            // Get angle required to achieve desired QDPoint spacing at major axis extreme
            float angleStep = 2f * (float) Math.Atan(0.5f * spacing / mSemiX);

            // With a +ve CCW step, choose if we should sweep from start or finish to produce our arc
            float startSweep, finishSweep;
            if (ccwDirection) { startSweep = QDUtils.QDUtils.degToRad(mStartAngle); finishSweep = QDUtils.QDUtils.degToRad(mFinishAngle); }
            else { startSweep = QDUtils.QDUtils.degToRad(mFinishAngle); finishSweep = QDUtils.QDUtils.degToRad(mStartAngle); }

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
        //public Path getPath() {
        //    mRotMat.reset();
        //    mRotMat.postRotate(mAngleD);
        //    mRotMat.postTranslate(mCentre.x, mCentre.y);
        //    path.reset();
        //    if (mDir == Path.Direction.CCW) {
        //        path.addArc(mBoundBox, mStartAngle - mAngleD, Utils.angleDiffCcwD(mStartAngle, mFinishAngle));
        //    }
        //    else {
        //        path.addArc(mBoundBox, mStartAngle - mAngleD, -Utils.angleDiffCwD(mStartAngle, mFinishAngle));
        //    }
        //    path.transform(mRotMat);
        //    return path;
        //}


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
