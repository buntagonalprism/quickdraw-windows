using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDShapes;
using WpfApplication1.QDUtils;
using System.Windows.Media;

namespace WpfApplication1.QDInputConversion
{
    class QDShapeFitting
    {
        //float mFitCost = 0f;

        // Return whether a shape fit has been successfully applied
        public bool analyse(QDInputPointSet pointSet) {
            // Determine what type of shape the user has drawn and fit accordingly
            // Do this using the distance-sampled angles to get the overall shape
            float angleMinD = 181.0f;
            float anglePosMinD = 181.0f;
            float angleMaxD = -181.0f;
            float angleNegMaxD = -181.0f;

            if (pointSet.sampledPtAnglesD.Count == 0)
                return false;
            foreach (float angle in pointSet.sampledPtAnglesD) {
                if (angle > angleMaxD) angleMaxD = angle;
                if (angle < angleMinD) angleMinD = angle;
                if (angle >= 0.0f && angle < anglePosMinD) anglePosMinD = angle;
                if (angle < 0.0f && angle > angleNegMaxD) angleNegMaxD = angle;
            }
            float angleRangeD = 0f;
            // Only include the positive or negative angular ranges if the curve had +ve or -ve angles
            if (anglePosMinD < 181.0f)
                angleRangeD += (angleMaxD - anglePosMinD);
            if (angleNegMaxD > -181.0f)
                angleRangeD += (angleNegMaxD - angleMinD);

            // A line has a similar angle throughout - the difference between max and min is small and the mean is in between
            if (angleRangeD < 40.0f) {
                pointSet.initialFit = pointSet.fittedShape = fitLine(pointSet);
                pointSet.shapeType = QDShapeTypes.LINE;
                pointSet.constraints.Add(QDConstraintTypes.STRAIGHT_LINE);
                if (((QDLine) pointSet.initialFit).vertical)
                    pointSet.constraints.Add(QDConstraintTypes.VERTICAL_LINE);
            }
            // TODO: other options apart from fit a circle
            else {
                pointSet.initialFit = pointSet.fittedShape = ellipseFit(pointSet.pts);
                pointSet.shapeType = QDShapeTypes.ELLIPSE;
                // TODO: change the constraint type
                pointSet.constraints.Add(QDConstraintTypes.CIRCLE);
                SweepDirection direction = getDirection(pointSet);
                float sweptAngle = findSweptArc(pointSet, (QDEllipse) pointSet.initialFit);
                if (sweptAngle < 330f) {
                    float startAngle = findArcStartD(direction, pointSet, (QDEllipse)pointSet.initialFit);
                    float finishAngle = findArcEndD(direction, pointSet, (QDEllipse)pointSet.initialFit);
                    pointSet.shapeType = QDShapeTypes.ELLIPTICAL_ARC;
                    pointSet.initialFit = pointSet.fittedShape = new QDEllipticalArc((QDEllipse)pointSet.initialFit, direction, startAngle, finishAngle);
                }
            }

            return true;
        }

            /**
             * Find angle swept by points around the centre of an elliptical arc. Used to determine
             * Whether it should be treated as a full ellipse or an arc
             */
            private float findSweptArc(QDInputPointSet ptSet, QDEllipse ellipse) {
            QDPoint centre = ellipse.mCentre;
            List<QDPoint> pts = ptSet.pts;

            float lastAngle = QDUtils.QDUtils.getPtToPtAngleD(pts[0], centre);
            float angleSweep = 0f;
            foreach (QDPoint pt in pts) {
                float angle = QDUtils.QDUtils.getPtToPtAngleD(pt, centre);
                angleSweep += QDUtils.QDUtils.angleDiffMinorD(angle, lastAngle);
                lastAngle = angle;
            }

            return angleSweep;
        }

            /**
             * Finds the final angle of an elliptical arc given its direction, searching points near
             * the last drawn point in case of small backtracks
             */
            private float findArcStartD(SweepDirection direction, QDInputPointSet ptSet, QDEllipse ellipse)
            {
            
                float startAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts[0]);
                if (direction == SweepDirection.Clockwise)
                {
                    // Check for any points near the first point more CCW than the current to set as start
                    for (int i = 1; i < Math.Ceiling(ptSet.pts.Count * 0.25f); i++)
                    {
                        float thisAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts[i]);
                        if (QDUtils.QDUtils.angleDiffMinorD(startAngle, thisAngle) > 0f)
                        {
                            startAngle = thisAngle;
                        }
                    }
                }
                else if (direction == SweepDirection.Counterclockwise)
                { // CCW arc
                    for (int i = 1; i < Math.Ceiling(ptSet.pts.Count * 0.25f); i++)
                    {
                        float thisAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts[i]);
                        if (QDUtils.QDUtils.angleDiffMinorD(startAngle, thisAngle) < 0f)
                        {
                            startAngle = thisAngle;
                        }
                    }
                }
                return startAngle;
            }

            /**
             * Finds the beginning angle of an elliptical arc given its direction, searching points near
             * the first drawn point in case of small backtracks
             */
            private float findArcEndD(SweepDirection direction, QDInputPointSet ptSet, QDEllipse ellipse)
            {
                float finishAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts.Last());
                if (direction == SweepDirection.Clockwise)
                {
                    // Check for any points near the first point more CCW than the current to set as start
                    for (int i = ptSet.pts.Count - 2; i < Math.Floor(ptSet.pts.Count * 0.75f); i--)
                    {
                        float thisAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts[i]);
                        if (QDUtils.QDUtils.angleDiffMinorD(finishAngle, thisAngle) < 0f)
                        {
                            finishAngle = thisAngle;
                        }
                    }
                }
                else if (direction == SweepDirection.Counterclockwise)
                { // CCW arc
                    for (int i = ptSet.pts.Count - 2; i < Math.Floor(ptSet.pts.Count * 0.75f); i--)
                    {
                        float thisAngle = QDUtils.QDUtils.getPtToPtAngleD(ellipse.mCentre, ptSet.pts[i]);
                        if (QDUtils.QDUtils.angleDiffMinorD(finishAngle, thisAngle) > 0f)
                        {
                            finishAngle = thisAngle;
                        }
                    }
                }
                return finishAngle;
            }

            /**
             * Evaluates the direction of an arc as being either CW or CCW
             */
            private SweepDirection getDirection(QDInputPointSet pointSet)
            {

                float angleDiffSum = 0f;
                if (pointSet.pts.Count >= 2)
                {
                    for (int i = 1; i < pointSet.anglesD.Count; i++)
                    {
                        float diff = QDUtils.QDUtils.angleDiffMinorD(pointSet.anglesD[i - 1], pointSet.anglesD[i]);
                        if (Math.Abs(diff) < 30.0f) // Reject significant pt-to-pt angle changes as outliers
                            angleDiffSum += diff;
                    }
                    if (angleDiffSum > 0)
                        return SweepDirection.Counterclockwise;
                    else
                        return SweepDirection.Clockwise;
                }
                else
                    return SweepDirection.Clockwise;
            }




            private QDLine fitLine(QDInputPointSet pointSet)
            {
                int n = pointSet.pts.Count - 1;

                float xsum = 0.0f;
                float ysum = 0.0f;
                for (int i = 0; i < n; i++)
                {
                    xsum += pointSet.pts[i].x;
                    ysum += pointSet.pts[i].y;
                }
                float xbar = xsum / n;
                float ybar = ysum / n;
                float denom, numer, gradient, diffx;
                denom = numer = 0.0f;
                for (int i = 0; i < n; i++)
                {
                    diffx = pointSet.pts[i].x - xbar;
                    numer += diffx * (pointSet.pts[i].y - ybar);
                    denom += diffx * diffx;
                }

                QDLine line = new QDLine();

                if (denom < 1e-2f)
                {                            // Check for vertical case

                    line.vertical = true;
                    line.start.x = line.finish.x = xbar;        // x coords at the average

                    line.start.y = pointSet.pts[0].y;       // y coords are limits of drawn line
                    line.finish.y = pointSet.pts[n].y;

                    // Check direction
                    if (line.finish.y > line.start.y) { line.angleD = 90.0f; }
                    else { line.angleD = -90.0f; }

                }
                else
                {
                    // Otherwise choose start and finish values as limits of line
                    gradient = numer / denom;
                    line.angleD = ((float)Math.Atan2(numer, denom)) * 180.0f / ((float)Math.PI);
                    line.intercept = ybar - (gradient * xbar);

                    if (Math.Abs(line.angleD) < 45.0f)
                    {
                        line.start.x = pointSet.pts[0].x;
                        line.start.y = line.start.x * gradient + line.intercept;

                        line.finish.x = pointSet.pts[n].x;
                        line.finish.y = pointSet.pts[n].y;
                    }
                    else
                    {
                        line.start.y = pointSet.pts[0].y;
                        line.start.x = (line.start.y - line.intercept) / gradient;

                        line.finish.y = pointSet.pts[n].y;
                        line.finish.x = (line.finish.y - line.intercept) / gradient;
                    }
                }
                return line;
            }

            public QDEllipse ellipseFit(List<QDPoint> pts)
            {

                List<float> thetaVec = initialQDEllipseGuess(pts);

                float alpha = 0.1f;
                List<float> gradient;
                //float lastCost;
                //mFitCost = Float.MAX_VALUE;
                // Iterate until we get less than 0.01% cost reduction between iterations
                int iters = 0;
                do
                {
                    //lastCost = mFitCost;
                    // mFitCost = 0f;
                    gradient = ellipseCostGradient(thetaVec, pts);
                    for (int i = 0; i < 4; i++)
                    {
                        thetaVec[i] = thetaVec[i] - alpha * gradient[i];
                    }
                    //} while (mFitCost < lastCost*0.99999);
                } while (iters++ < 500);

                return new QDEllipse(new QDPoint(thetaVec[0], thetaVec[1]),
                        new QDPoint(thetaVec[2], thetaVec[3]), thetaVec[4]);
            }

            /**
             * Calculate the theta parameters for an initial elliptical fit guess. The current method
             * takes three points reasonably distributed around the shape and fit a circle to them. Then
             * construct the elliptical parameters corresponding to that circle but slightly perturbed
             * to prevent the focal points getting stuck on top of each other.
             */
            public List<float> initialQDEllipseGuess(List<QDPoint> pts) {
            float distTotal = 0f;
            for (int i = 1; i < pts.Count; i++) {
                distTotal += QDUtils.QDUtils.getPtToPtDist(pts[i], pts[i-1]);
            }

            // Get three points distributed by 30% of distance travelled for good spacing
            List<QDPoint> threePt = new List<QDPoint>();
            threePt.Add(pts[0]);
            float distSoFar = 0f;
            int start = 1;
            for (int x = 0; x < 2; x++) {
                for (int i = start; i < pts.Count; i++) {
                    distSoFar += QDUtils.QDUtils.getPtToPtDist(pts[i],pts[i-1]);
                    if (distSoFar > 0.3* distTotal) {
                        threePt.Add(pts[i]);
                        distSoFar = 0;
                        start = i;
                        break;
                    }
                }
            }
            if (threePt.Count < 3)
                threePt.Add(pts[pts.Count-1]); // add the last point if we didn't make it to the end

            QDCircle circle = threePointCircleFit(threePt);
            List<float> thetaVec = new List<float>();
            RandGauss rand = new RandGauss();

            thetaVec.Add(circle.centre.x + circle.radius * 0.05f * rand.nextGaussF());
            thetaVec.Add(circle.centre.y + circle.radius * 0.05f * rand.nextGaussF());
            thetaVec.Add(circle.centre.x + circle.radius * 0.05f * rand.nextGaussF());
            thetaVec.Add(circle.centre.y + circle.radius * 0.05f * rand.nextGaussF());
            thetaVec.Add(2*circle.radius);
            return thetaVec;
        }

            /**
             * Fit a circle to three given points on the circumference. The method draws lines (chords)
             * between two pairs of the points, constructs their perpendicular bisectors, and finds the
             * intersection point of those two lines. This is the centre of the circle as radii passing
             * through the centre are perpendicular bisectors of chords in a circle. The radius is the
             * distance between the centre and any one of the initial three points.
             */
            private QDCircle threePointCircleFit(List<QDPoint> pts)
            {
                QDLine l1 = new QDLine(pts[0], pts[1]);
                QDLine l2 = new QDLine(pts[1], pts[2]);
                QDInfiniteLine i1 = new QDInfiniteLine(l1.getMidpoint(), l1.getPerpAngleD());
                QDInfiniteLine i2 = new QDInfiniteLine(l2.getMidpoint(), l2.getPerpAngleD());
                QDPoint centre = i1.intersect(i2);
                float radius = QDUtils.QDUtils.getPtToPtDist(centre, pts[0]);
                return new QDCircle(centre, radius);

            }

            /**
             * Calculate the gradient of the elliptical fit cost function for the current theta vector
             * and points vector. This calculated by iterating through all the points and summing their
             * individual contributions to the partial gradients with respect to each theta value.
             */
            public List<float> ellipseCostGradient(List<float> theta, List<QDPoint> pts) {
            float t1 = theta[0], t2 = theta[1], t3 = theta[2];
            float t4 = theta[3], t5 = theta[4];

            float g1 = 0f, g2 = 0f, g3 = 0f, g4 = 0f, g5 = 0f;
            for (int i = 0; i < pts.Count; i++) {
                float x = pts[i].x;
                float y = pts[i].y;
                float t1x = t1-x;
                float t2y = t2-y;
                float t3x = t3-x;
                float t4y = t4-y;

                float distDiff = t5 - (float) Math.Sqrt(t1x*t1x + t2y*t2y) - (float) Math.Sqrt(t3x*t3x + t4y*t4y);

               // mFitCost += distDiff*distDiff;

                g1 += 2 * (t1x * (-distDiff)) / (float)Math.Sqrt(t1x * t1x + t2y * t2y);
                g2 += 2 * (t2y * (-distDiff)) / (float)Math.Sqrt(t1x * t1x + t2y * t2y);
                g3 += 2 * (t3x * (-distDiff)) / (float)Math.Sqrt(t3x * t3x + t4y * t4y);
                g4 += 2 * (t4y * (-distDiff)) / (float)Math.Sqrt(t3x * t3x + t4y * t4y);
                g5 += 2*distDiff;
            }

            List<float> gradient = new List<float>();
            gradient.Add(g1/pts.Count);
            gradient.Add(g2/pts.Count);
            gradient.Add(g3/pts.Count);
            gradient.Add(g4/pts.Count);
            gradient.Add(g5/pts.Count);

            return gradient;
        }
    }
}
