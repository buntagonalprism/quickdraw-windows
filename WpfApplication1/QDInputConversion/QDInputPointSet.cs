using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;
using WpfApplication1.QDShapes;
using System.Collections;

namespace WpfApplication1.QDInputConversion
{
    class QDInputPointSet
    {
        public List<QDConstraintTypes> constraints = new List<QDConstraintTypes>();
        public QDShape initialFit = null;
        public QDShape fittedShape = null;
        public QDShapeTypes shapeType;

        public List<QDPoint> pts = new List<QDPoint>();
        public List<float> anglesD = new List<float>();                 // Point to point angles to understand input line
        public List<float> meanSmoothedAnglesD = new List<float>();     // Angle smoothing to reduce noise
        public List<float> sampledPtAnglesD = new List<float>();        // Sub-sampling for greater noise reduction and overall line shape
        private QDPoint sampledMarkerPt = new QDPoint();

        QDInputPointSet newSeg = null;

        //protected transient Path ptsPathDebug = new Path();
        protected float lastScaleFactorDebug = 1.0f;
        protected QDPoint lastWindowOriginDebug = new QDPoint(0.0f, 0.0f);

        QDPoint start;
        //Point finish;
        float angle_max = -181.0f;
        float angle_min = 181.0f;
        float angle_sum = 0.0f;

        int CORNER_WINDOW = 7;
        float CNR_ANGLE_THRESH = 30.0f;
        float SUBSAMP_PT_DIST = 40.0f;

        public bool cornerTerminated = false;



        // Empty default constructor
        public QDInputPointSet(){}

        // Constructor allows variable window size and angleD threshold used to find corners
        public QDInputPointSet(int cnr_window, float cnr_angle_thresh) {
            CORNER_WINDOW = cnr_window;
            CNR_ANGLE_THRESH = cnr_angle_thresh;
        }

        // Copy constructor
        public QDInputPointSet(QDInputPointSet other) {
            this.pts = other.pts;
            this.anglesD = other.anglesD;
            this.meanSmoothedAnglesD = other.meanSmoothedAnglesD;
            this.start = other.start;
            //this.finish = other.finish;
            this.angle_max = other.angle_max;
            this.angle_min = other.angle_min;
            this.angle_sum = other.angle_sum;
        }

        // Returns a new InputSegment containing all the points at the finish of the segment after the
        // corner was found, ready to be used in the next segment.
        public QDInputPointSet getCornerOverlapSet() {
            return newSeg;
        }

        // Add a point to the segment list and check for a corner in the segment
        // Returns true if a corner was detected, false otherwise
        public bool addPoint(QDPoint pt) {
            // Special case for the first point
            if (pts.Count == 0) {
                start = pt;
                pts.Add(pt);
                sampledMarkerPt = pt;
                return false;
            }

            // Get the pt to pt gradient
            float angle = QDUtils.QDUtils.getPtToPtAngleD(pts.Last(), pt);

            // Store summary statistics
            if (angle > angle_max) { angle_max = angle; }
            if (angle < angle_min) { angle_min = angle; }
            angle_sum += angle;

            // Only stores the angles between points sufficiently far apart. This is the most
            // noise robust, and useful for getting the general form of the input shape
            if (QDUtils.QDUtils.getPtToPtDist(sampledMarkerPt, pt) > SUBSAMP_PT_DIST) {
                sampledPtAnglesD.Add(QDUtils.QDUtils.getPtToPtAngleD(sampledMarkerPt, pt));
                sampledMarkerPt = pt;
            }

            // Add values to vectors
            anglesD.Add(angle);
            pts.Add(pt);


            // Step 1: track back to get testPt
            float TEST_RADIUS = 20.0f;
            float CNR_RADIUS = 10.0f;
            QDPoint testPt = null, cnrCandidatePt = null, currPt = pt;
            int midIdx = -1, testIdx = -1, cnrCandidateIdx = -1;
            midIdx = backtrackByDist(pts, pts.Count - 2, TEST_RADIUS/2.0f);
            if (midIdx < 0) return false;
            testIdx = backtrackByDist(pts, midIdx, TEST_RADIUS/2.0f);
            if (testIdx < 0) return false;
            testPt = pts[testIdx];

            // Step 2: construct line from testPt to currPt
            QDInfiniteLine testLine = new QDInfiniteLine(testPt, currPt);

            // Step 3: Find perp dist from each pt between test and curr to the line
            float maxDist = 0.0f;
            int maxIdx = -1;
            for (int i = testIdx + 1; i < pts.Count - 1; i++) {
                float thisDist = testLine.getDistToPoint(pts[i]);
                if ( thisDist > maxDist) {
                    maxDist = thisDist;
                    maxIdx = i;
                }
            }

            // Step 4: confirm cnrCandidatePt
            if (maxIdx < 0) {
                return false;
            }
            cnrCandidatePt = pts[maxIdx];
            int cnrIdx = maxIdx;
            if (!(QDUtils.QDUtils.getPtToPtDist(cnrCandidatePt, currPt) > CNR_RADIUS))
            {
                return false;
            }

            // Step 5: construct lines
            float oldAngleD = new QDLine(testPt, cnrCandidatePt).angleD;
            float newAngleD = new QDLine(cnrCandidatePt, currPt).angleD;

            // Step 6: test angle between lines
            if (Math.Abs(QDUtils.QDUtils.angleDiffMinorD(oldAngleD, newAngleD)) > CNR_ANGLE_THRESH) {

                // Step 7: test for continuous curvature
                int strTestIdx = backtrackByDist(pts, testIdx, CNR_RADIUS);
                if (strTestIdx < 0) return false;
                int strTest2Idx = backtrackByDist(pts, strTestIdx, CNR_RADIUS);
                if (strTest2Idx < 0) return false;
                float seg1 = new QDLine(pts[strTest2Idx], pts[strTestIdx]).angleD;
                float seg2 = new QDLine(pts[strTestIdx], pts[testIdx]).angleD;
                if (Math.Abs(QDUtils.QDUtils.angleDiffMinorD(seg1, seg2)) < 15.0f)
                    cornerTerminated = true;
                else
                    cornerTerminated = false;
            }

            if (cornerTerminated) {
                newSeg = new QDInputPointSet(CORNER_WINDOW, CNR_ANGLE_THRESH);
                newSeg.addPoint(cnrCandidatePt);                    // Add the corner point
                int lastIdx = pts.Count - 1;
                for (int i = 0; i < lastIdx - cnrIdx; i++) {        // For how many points there are after the corner
                    newSeg.addPoint(this.pts[cnrIdx + 1]);      // Copy points after the corner into new seg
                    this.pts.RemoveAt(cnrIdx + 1);                    // Delete points after corner from current set
                    this.anglesD.Remove(cnrIdx);                    // There is one less angle than pts since first two pts make 1 angle
                }
                // Remove subsampled angles as well up to before the corner
                int deleteSubSamp = (int) Math.Ceiling(CNR_RADIUS/SUBSAMP_PT_DIST);
                for (int i = 0; i < deleteSubSamp; i++) {
                    sampledPtAnglesD.Remove(sampledPtAnglesD.Count - deleteSubSamp + i);
                }
                return true;
            }

            return false;
        }

        // From a given start index in a point vector, backtrack until a point is found which is
        // greater than the radius distance away from the starting point
        private int backtrackByDist(List<QDPoint> pts, int startIdx, float radius) {
            for (int i = startIdx - 1; i >= 0; i--) {
                if (QDUtils.QDUtils.getPtToPtDist(pts[i], pts[startIdx]) > radius) {
                    return i;
                }
            }
            return -1;
        }

        //public Path getPointsAsPathDEBUG(float ptCircRadius, Point windowOrigin, float scaleFactor) {
        //    //if (ptsPathDebug.isEmpty() || windowOrigin != lastWindowOriginDebug || scaleFactor != lastScaleFactorDebug) {
        //        lastScaleFactorDebug = scaleFactor;
        //        lastWindowOriginDebug = windowOrigin;
        //        ptsPathDebug.reset();
        //        for (Point pt : pts ) {
        //            Point localPt = Utils.absToLocCoords(pt, scaleFactor, windowOrigin);
        //            ptsPathDebug.addCircle(localPt.x, localPt.y, ptCircRadius, Path.Direction.CW);
        //        }
        //   // }
        //    return ptsPathDebug;
        //}

    }
}
