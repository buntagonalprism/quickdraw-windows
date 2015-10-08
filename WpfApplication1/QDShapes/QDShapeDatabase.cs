using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDInputConversion;

namespace WpfApplication1.QDShapes
{
    public class QDShapeDatabase
    {
        private QuadTree quadTree = new QuadTree(200);
        private List<QDInputPointSet> ptSets = new List<QDInputPointSet>();
        public float PT_SPACING = 30.0f;

        public QDInputPointSet getPtSetById(int objectID) { return ptSets[objectID]; }

        public List<QDInputPointSet> getPtSets()
        {
            return ptSets;
        }

        public void setPtSetById(QDInputPointSet ptSet, int ID)
        {
            ptSets[ID] = ptSet;
        }

        public void setPtSets(List<QDInputPointSet> ptSetsIn)
        {
            ptSets = ptSetsIn;
        }

        public void addInputPointSet(QDInputPointSet set)
        {
            int objectID = ptSets.Count;
            ptSets.Add(set);
            List<QDShapeDBPoint> intermediatePts = set.fittedShape.getIntermediatePoints(PT_SPACING);
            foreach (QDShapeDBPoint pt in intermediatePts)
            {
                pt.shape = set.fittedShape;
                quadTree.addPoint(pt);
            }
        }

        public void reset()
        {
            ptSets.Clear();
            quadTree = new QuadTree(200);
        }

        public List<QDShapeDBPoint> getShapesNearPoint(QDPoint point, float radius)
        {
            // Get the ShapeSampledPoints near the query point
            List<QDPoint> pts = new List<QDPoint>();
            quadTree.getPoints(point, radius, pts);

            // Create a list of all the PointSets this corresponds to
            //HashMap<QDInputPointSet, HashSet<PointTypes>> returnPtSets = new HashMap<>();
            List<QDShapeDBPoint> returnPts = new List<QDShapeDBPoint>();
            foreach (QDPoint pt in pts)
            {
                returnPts.Add((QDShapeDBPoint)pt);
                //            QDInputPointSet ptSet = ptSets.get( ((ShapeSampledPoint)pt).objectID );
                //            HashSet<PointTypes> types = new HashSet<>();
                //            // check to see if the list already contains the PointSet
                //            if (returnPtSets.containsKey(ptSet)){
                //                types = returnPtSets.get(ptSet);
                //                types.add(((SampledPoint) pt).type);
                //            }
                //            else {
                //                types.add(((SampledPoint) pt).type);
                //
                //            }
                //            returnPtSets.put(ptSet, types);
            }

            return returnPts;
        }
    }
}
