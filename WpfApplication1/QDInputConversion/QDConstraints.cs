using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDShapes;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDInputConversion
{
    public class QDConstraints
    {
        QDShapeDatabase shapeDB;
        private float line_snap_thresh = 10f;
        private static float SEARCH_RADIUS = 50f;

        public QDConstraints(QDShapeDatabase shapeDB_in)
        {
            shapeDB = shapeDB_in;
        }

        // Returns the list of other shapes modified which need redrawing
        public List<QDShape> analyse(QDInputPointSet ptSet)
        {
            List<QDShape> modifiedShapes = new List<QDShape>();
            getIntrinsicConstraints(ptSet);
            modifiedShapes = getExtrinsicConstraints(ptSet);
            return modifiedShapes;
        }

        public void getRelatedShapes()
        {

        }
        public void getIntrinsicConstraints(QDInputPointSet ptSet)
        {
            if (ptSet.shapeType == QDShapeTypes.LINE)
            {
                QDLine line = (QDLine)ptSet.initialFit;
                float angle = line.angleD;
                // Check for vertical line and adjust by midpoint pivot
                if ((90.0f - line_snap_thresh < angle && angle < 90.0f + line_snap_thresh) ||
                        (-90.0f - line_snap_thresh < angle && angle < -90.0f + line_snap_thresh) && !line.vertical)
                {
                    QDPoint newStart, newFinish;
                    
                    if (line.start.y > line.getMidpoint().y)
                    {
                        newStart = new QDPoint(line.midPoint.x, line.midPoint.y + (0.5f * line.length));
                        newFinish = new QDPoint(line.midPoint.x, line.midPoint.y - (0.5f * line.length));
                    }
                    else
                    {
                        newStart = new QDPoint(line.midPoint.x, line.midPoint.y - (0.5f * line.length));
                        newFinish = new QDPoint(line.midPoint.x, line.midPoint.y + (0.5f * line.length));
                    }
                    ptSet.fittedShape = new QDLine(newStart, newFinish);
                    ptSet.constraints.Add(QDConstraintTypes.VERTICAL_LINE);
                }
                // Check for horizontal line and adjust by midpoint pivot
                else if ((-line_snap_thresh < angle && angle < line_snap_thresh) ||
                        (180.0f - line_snap_thresh < angle || angle < -180.0f + line_snap_thresh))
                {
                    float length = (float)Math.Sqrt(Math.Pow(line.start.x - line.finish.x, 2.0f) + Math.Pow(line.start.y - line.finish.y, 2.0f));
                    QDPoint midPt = new QDPoint((line.start.x + line.finish.x) * 0.5f, (line.start.y + line.finish.y) * 0.5f);
                    QDPoint newStart = new QDPoint();
                    QDPoint newFinish = new QDPoint();
                    newStart.y = newFinish.y = midPt.y;
                    if (line.start.x > midPt.x)
                    {
                        newStart.x = midPt.x + (0.5f * length);
                        newFinish.x = midPt.x - (0.5f * length);
                    }
                    else
                    {
                        newStart.x = midPt.x - (0.5f * length);
                        newFinish.x = midPt.x + (0.5f * length);
                    }
                    ptSet.fittedShape = new QDLine(newStart, newFinish);
                    ptSet.constraints.Add(QDConstraintTypes.HORIZONTAL_LINE);
                }
            }
        }
        public List<QDShape> getExtrinsicConstraints(QDInputPointSet ptSet)
        {
            List<QDShape> modifiedShapes = new List<QDShape>();
            if (ptSet.shapeType == QDShapeTypes.LINE) { 

                QDLine line = (QDLine)ptSet.fittedShape;
                List<QDShapeDBPoint> nearStartPts = shapeDB.getShapesNearPoint(line.start, SEARCH_RADIUS);
                foreach (QDShapeDBPoint pt in nearStartPts)
                {
                    if (pt.type == QDPointTypes.LINE_START || pt.type == QDPointTypes.LINE_FINISH)
                    {
                        QDLine nearLine = (QDLine)pt.shape;

                        if (!modifiedShapes.Contains(nearLine))
                            modifiedShapes.Add(nearLine);

                        QDInfiniteLine thisInf = new QDInfiniteLine(line.start, line.finish);
                        QDInfiniteLine nearInf = new QDInfiniteLine(nearLine.start, nearLine.finish);
                        // Check for nearly parallel lines
                        if (Math.Abs(QDUtils.QDUtils.angleDiffMinorD(line.angleD, nearLine.angleD)) > 10f) {
                            QDPoint intersectPt = thisInf.intersect(nearInf);
                            if (QDUtils.QDUtils.getPtToPtDist(line.start, intersectPt) < SEARCH_RADIUS*1.25)
                            {
                                line.start = intersectPt;
                                if (pt.type == QDPointTypes.LINE_START)
                                    nearLine.start = intersectPt;
                                else
                                    nearLine.finish = intersectPt;
                            }
                        }
                        
                    }
                }

                /**
                * GOTTA BE A BETTER WAY OF DOING THIS RATHER THAN DOING IT ONCE FOR START AND ONCE FOR FINISH
                */
                List<QDShapeDBPoint> nearFinishPts = shapeDB.getShapesNearPoint(line.finish, SEARCH_RADIUS);
                foreach (QDShapeDBPoint pt in nearFinishPts)
                {
                    if (pt.type == QDPointTypes.LINE_START || pt.type == QDPointTypes.LINE_FINISH)
                    {
                        QDLine nearLine = (QDLine)pt.shape;

                        if (!modifiedShapes.Contains(nearLine))
                            modifiedShapes.Add(nearLine);

                        QDInfiniteLine thisInf = new QDInfiniteLine(line.start, line.finish);
                        QDInfiniteLine nearInf = new QDInfiniteLine(nearLine.start, nearLine.finish);
                        // Check for nearly parallel lines
                        if (Math.Abs(QDUtils.QDUtils.angleDiffMinorD(line.angleD, nearLine.angleD)) > 10f)
                        {
                            QDPoint intersectPt = thisInf.intersect(nearInf);
                            if (QDUtils.QDUtils.getPtToPtDist(line.finish, intersectPt) < SEARCH_RADIUS * 1.25)
                            {
                                line.finish = intersectPt;
                                if (pt.type == QDPointTypes.LINE_START)
                                    nearLine.start = intersectPt;
                                else
                                    nearLine.finish = intersectPt;
                            }
                        }

                    }
                }

                
            }
            return modifiedShapes;
        }

        

        public void selectConstraints()
        {

        }
        public void applyConstraints()
        {

        }
    }
}
