using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Shapes;

namespace WpfApplication1.QDShapes
{
    public class QDEllipse : QDShape
    {
        
        public QDPoint mFoci1;
        public QDPoint mFoci2;
        public QDPoint mCentre;
        public float mSemiX, mSemiY, mLengthX, mLengthY;
        public float mFocalLength, mFociToCirc;
        public float mAngleR, mAngleD;
        public RectangleF mBoundBox;
        Matrix mRotMat = new Matrix();

        public QDEllipse() {}

        public QDEllipse(QDPoint centre, float semiX, float semiY, float angle) {

        }

        public QDEllipse(RectangleF boundBox, float angle) {

        }

        public QDEllipse(QDPoint foci1, QDPoint foci2, float fociToCirc) {
            constructFromFoci(foci1, foci2, fociToCirc);
        }

        /**
         * Constructs an QDEllipse shape using the five basic parameters of the x and y coordinates of
         * the two foci locations and the sum distance between each circumference point and the foci
         * @param foci1         Point of the first foci
         * @param foci2         Point of the second foci
         * @param fociToCirc    Sum distance between each circumference point and the foci
         */
        protected void constructFromFoci(QDPoint foci1, QDPoint foci2, float fociToCirc) {
            mFoci1 = foci1;
            mFoci2 = foci2;
            mFociToCirc = fociToCirc;

            // The centre is the average between the two foci
            mCentre = new QDPoint(0.5f*(mFoci1.x + mFoci2.x), 0.5f*(mFoci1.y + mFoci2.y));

            // The focal length can be found by distance between the centre and foci
            mFocalLength = (float) Math.Sqrt(Math.Pow(mFoci1.x - mCentre.x, 2) + Math.Pow(mFoci1.y - mCentre.y, 2));

            // The angle of the QDEllipse is the angle drawn between the two foci
            mAngleR = (float) Math.Atan((mFoci1.y - mFoci2.y) / (mFoci1.x - mFoci2.x));
            mAngleD = 180.0f * mAngleR / (float) Math.PI;

            // The semi-major axis is half the sum distance between circumference points and the foci
            mSemiX = mFociToCirc * 0.5f;
            mLengthX = mFociToCirc;

            // Find semi-minor axis by pythagoras between focal length and semi-major axis
            mSemiY = (float) Math.Sqrt(mSemiX*mSemiX - mFocalLength*mFocalLength);
            mLengthY = 2*mSemiY;

            // Create the bounding box for the unrotated, untranslated QDEllipse using its semi-axes
            mBoundBox = new RectangleF(- mSemiX, - mSemiY, + mSemiX, + mSemiY);

            // Create the rotation and translation matrix to position the QDEllipse
            mRotMat = new Matrix();
            mRotMat.Rotate(mAngleD);
            mRotMat.Translate(mCentre.x, mCentre.y);

        }

        // Create intermediate points by sweeping around a standard elliptical shape then manually
        // applying the rotation and translation to the point.

        public override List<QDShapeDBPoint> getIntermediatePoints(float spacing) {
            List<QDShapeDBPoint> pts = new List<QDShapeDBPoint>();

            for (float t = 0f; t < 2*Math.PI; t = t + 0.05f) {
                QDPoint parametric = new QDPoint(mSemiX * (float) Math.Cos(t), mSemiY * (float) Math.Sin(t));
                QDPoint rotatedPoint = new QDPoint((float) Math.Cos(mAngleR)*parametric.x - (float) Math.Sin(mAngleR)*parametric.y,
                        (float) Math.Sin(mAngleR)*parametric.x + (float) Math.Cos(mAngleR) * parametric.y);
                QDPoint translatedPoint = new QDPoint(rotatedPoint.x + mCentre.x, rotatedPoint.y + mCentre.y);
                QDShapeDBPoint sampled = new QDShapeDBPoint(translatedPoint, QDPointTypes.CIRCLE_CIRCUMFERENCE);
                pts.Add(sampled);
            }
            return pts;
        }

        // Return the QDEllipse transformed by its position and rotation
        public override Path getPath()
        {
            if (path.Data == null)
            {
                //path.addOval(mBoundBox, Path.Direction.CCW);
                //path.transform(mRotMat);
            }
            return path;
        }

        // Use the scalefactor to scale the bounding box, and windowOriging to translate the QDEllipse
        //@Override
        //public Path getPath(Point windowOrigin, float scaleFactor) {
        //    Point centreLoc = absToLocCoords(mCentre, scaleFactor, windowOrigin);
        //    float semiXloc = mSemiX*scaleFactor;
        //    float semiYloc = mSemiY*scaleFactor;
        //    RectF boundLoc = new RectF( -semiXloc,  - semiYloc,
        //              semiXloc,    semiYloc);
        //    mRotMat.reset();
        //    mRotMat.postRotate(mAngleD);
        //    mRotMat.postTranslate(centreLoc.x, centreLoc.y);
        //    path.reset();
        //    path.addOval(boundLoc, Path.Direction.CCW);
        //    path.transform(mRotMat);
        //    return path;
        //}


        // Pretty sure this is what is called by Shape.class, here we don't want to do anything
        // different so we just override to reuse the default
        //private void writeObject(ObjectOutputStream oos) throws IOException {
        //    oos.defaultWriteObject();
        //}


        //private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException {
        //    ois.defaultReadObject();
        //    // Initialise data which didn't serialise
        //    mRotMat = new Matrix();
        //    mBoundBox = new RectF(- mSemiX, - mSemiY, + mSemiX, + mSemiY);

        //}

        //@Override
        public override String getSvgString()
        {
            return null;
        }
    }
}
