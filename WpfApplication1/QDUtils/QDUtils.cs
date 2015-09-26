using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDShapes;

namespace WpfApplication1.QDUtils
{
    class QDUtils
    {
        public static float toDegrees(float rad)
        {
            return 180.0f * rad / (float)Math.PI;
        }

        public static float toRadians(float deg)
        {
            return (float)Math.PI * deg / 180.0f;
        }

        /**
        * Returns the CCW angle between start and finish between [0,360] deg
        * Range of output values: [0,360]
        * @param start     Angle to start from
        * @param finish    Angle to finish at
        * @return          Angle from start to finish traversed CCW
        */
        public static float angleDiffCcwD(float start, float finish)
        {
            if (finish > start)
                return finish - start;
            else
                return finish - start + 360.0f;
        }

        /**
         * Returns the CW angle between start and finish between [0,360] deg (always positive despite
         * being a CW angle
         * Range of output values: [0,360]
         * @param start     Angle to start from
         * @param finish    Angle to finish at
         * @return          Angle from start to finish traversed CW - always positive
         */
        public static float angleDiffCwD(float start, float finish)
        {
            if (finish < start)
                return start - finish;
            else
                return start - finish + 360;
        }

        /**
         * Returns the signed (CCW positive) angle between start and finish along the minor segment
         * Range of output values: [-180,180]
         * @param start     Angle to start from
         * @param finish    Angle to finish at
         * @return          Signed minor-segment angle from start to finish
         */
        public static float angleDiffMinorD(float start, float finish)
        {
            float diff = finish - start;
            diff += (diff > 180.0f) ? -360.0f : (diff < -180.0f) ? 360.0f : 0.0f;
            return diff;
        }

        /**
         * Returns the signed (CCW positive) angle between start and finish along the major segment
         * Range of output values: [-360,360]
         * @param start     Angle to start from
         * @param finish    Angle to finish at
         * @return          Signed major-segment angle from start to finish
         */
        public static float angleDiffMajorD(float start, float finish)
        {
            return 360.0f - angleDiffMinorD(start, finish);
        }

        /**
         * Returns the signed (CCW positive) angle from the start point to the finish point
         * Range of output values: (-180,180]
         * @param start     Point to start from
         * @param finish    Point to finish at
         * @return          Signed angle from start to finish point
         */
        public static float getPtToPtAngleD(QDPoint start, QDPoint finish)
        {
            float angle;
            float run = finish.x - start.x;
            float rise = finish.y - start.y;

            // Check for near-vertical div-by-zero issues
            if (Math.Abs(run) < 1e-2f)
            {
                if (finish.y > start.y) { angle = 90.0f; }
                else { angle = -90.0f; }
            }
            else
            {
                angle = ((float)Math.Atan2(rise, run)) * 180.0f / ((float)Math.PI);
            }
            return angle;
        }

        /**
         * Wraps an angle from [-2pi,2pi] to an angle [-pi,pi]. Note it does not cover the issue
         * of angles outside an extra revolution - that shouldn't happen if this is called regularly
         * Range of output values: [-pi,pi]
         * @param angle     Angle to wrap
         * @return          Angle wrapped to [-pi,pi]
         */
        public static float wrapToPi(float angle)
        {
            return angle += (angle > (float)Math.PI) ? -2.0f * (float)Math.PI : (angle < -(float)Math.PI) ? 2.0f * (float)Math.PI : 0.0f;
        }

        /**
         * Wraps an angle from [-360,360] to an angle [-180,180]. Note it does not cover the issue
         * of angles outside an extra revolution - that shouldn't happen if this is called regularly
         * Range of output values: [-180,180]
         * @param angle     Angle to wrap
         * @return          Angle wrapped to [-180,180]
         */
        public static float wrapTo180(float angle)
        {
            return angle += (angle > 180.0f) ? -360.0f : (angle < -180.0f) ? 360.0f : 0.0f;
        }

        /**
         * Conver an angle from radians to degrees
         * @param angle     Angle in radians
         * @return          Angle in degrees
         */
        public static float radToDeg(float angle)
        {
            return angle * 180.0f / (float)Math.PI;
        }


        /**
         * Convert an angle from degrees to radians
         * @param angle     Angle in degrees
         * @return          Angle in radians
         */
        public static float degToRad(float angle)
        {
            return angle * (float)Math.PI / 180.0f;
        }


        /**
         * Get the distance between two points
         * @param p1    Point 1
         * @param p2    Point 2
         * @return      Straight line distance between points
         */
        public static float getPtToPtDist(QDPoint p1, QDPoint p2)
        {
            return (float)Math.Pow(Math.Pow(p1.x - p2.x, 2.0) + Math.Pow(p1.y - p2.y, 2), 0.5);
        }


        /**
         * Convert a point from absolute coordinates into the local viewing frame using the origin of
         * the view and the current scale factor
         * @param absPt     The point in absolute coordinates to convert
         * @param SF        The current scale factor of the viewing window
         * @param origin    The origin of the viewing window
         * @return          The point in local coordinates
         */
        public static QDPoint absToLocCoords(QDPoint absPt, float SF, QDPoint origin)
        {
            return new QDPoint((absPt.x - origin.x) * SF, (absPt.y - origin.y) * SF);
        }

        /**
         * Convert a point from local coordinates into the global reference viewing frame using the
         * origin of the view and the current scale factor
         * @param locPt     The point in local coordinates to convert
         * @param SF        The current scale factor of the viewing window
         * @param origin    The origin of the viewing window
         * @return          The point in local coordinates
         */
        public static QDPoint locToAbsCoords(QDPoint locPt, float SF, QDPoint origin)
        {
            return new QDPoint((locPt.x / SF) + origin.x, (locPt.y / SF) + origin.y);
        }

        /**
         * Converts a value in density indepdendent pixels into a pixel value
         * @param context   Contenxt object, used for fetching the phone screen density
         * @param dp        The distance measurement in density independent pixels 'dp' to convert
         * @return          The equivalent distance in pixels for the screen density
         */
        //public static float dpToPx(Context context, float dp)
        //{
        //    float density = context.getResources().getDisplayMetrics().density;
        //    return dp * density;
        //}

    }
}
