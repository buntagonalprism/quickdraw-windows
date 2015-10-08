using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.QDShapes
{
    public class QuadTree
    {
        private QuadNode masterNode;

        public QuadTree(float rootNodeSize)
        {
            masterNode = new QuadNode(0.0f, 0.0f, rootNodeSize);
        }

        public void addPoints(List<QDPoint> pts)
        {
            foreach (QDPoint pt in pts)
                addPoint(pt);
        }

        public void addPoint(QDPoint pt)
        {
            // If the current master node does not contain the point, keep increasing the square size until it does
            while (!masterNode.contains(pt))
            {
                bool expandXPos = true; // Whether the quad tree should expand in positive or negative direction
                bool expandYPos = true;
                float newCnrX, newCnrY;
                // Check where new point is located relative to current master node
                if (pt.x > masterNode.maxX) {
                    expandXPos = true;
                    newCnrX = masterNode.minX;
                }
                else {
                    expandXPos = false;
                    newCnrX = masterNode.minX - masterNode.width;
                }
                if (pt.y > masterNode.maxY) {
                    expandYPos = true;
                    newCnrY = masterNode.minY;
                }
                else {
                    expandYPos = false;
                    newCnrY = masterNode.minY - masterNode.height;
                }

                // Initialise new master node
                QuadNode newMaster = new QuadNode(newCnrX, newCnrY, masterNode.size * 2f);
                newMaster.topL = new QuadNode(newCnrX, newCnrY, masterNode.size);
                newMaster.topR = new QuadNode(newCnrX + masterNode.size, newCnrY, masterNode.size);
                newMaster.botL = new QuadNode(newCnrX, newCnrY + masterNode.size, masterNode.size);
                newMaster.botR = new QuadNode(newCnrX + masterNode.size, newCnrY + masterNode.size, masterNode.size);

                // Insert current master node into newMaster
                if (expandXPos && expandYPos)
                    newMaster.topL = masterNode;
                else if (expandXPos && !expandYPos)
                    newMaster.botL = masterNode;
                else if (!expandXPos && expandYPos)
                    newMaster.topR = masterNode;
                else
                    newMaster.botR = masterNode;

                masterNode = newMaster;

            }
            masterNode.addPoint(pt);
        }

        public void getPoints(QDPoint pt, float radius, List<QDPoint> pts)
        {
            Square search = new Square(pt.x - radius, pt.y - radius, 2.0f * radius);
            masterNode.getPoints(search, pts);
        }

        public class Rectangle
        {
            public float minX = 0.0f;
            public float maxX = 0.0f;
            public float minY = 0.0f;
            public float maxY = 0.0f;
            public float width = 0.0f;
            public float height = 0.0f;

            public Rectangle(float cnr_x, float cnr_y, float width_in, float height_in)
            {
                minX = cnr_x;
                minY = cnr_y;
                width = width_in;
                height = height_in;
                maxX = minX + width;
                maxY = minY + height;
            }

            public Boolean intersect(Rectangle r)
            {
                return this.minX <= r.maxX && this.maxX >= r.minX && this.minY <= r.maxY && this.maxY >= r.minY;
            }

            public Boolean contains(QDPoint pt)
            {
                return contains(pt.x, pt.y);
            }

            public Boolean contains(float x, float y)
            {
                return this.minX <= x && this.maxX >= x && this.minY <= y && this.maxY >= y;
            }
        }

        public class Square : Rectangle 
        {
            public float size = 0.0f;

            public Square(float cnr_x, float cnr_y, float size_in) : base(cnr_x, cnr_y, size_in, size_in){
                size = size_in;
            }
        }

        public class QuadNode : Square 
        {
            private int child_per_branch = 1;
            public List<QDPoint> pts;
            public QuadNode topL, topR, botL, botR;

            public QuadNode(float minX_in, float minY_in, float size_in) :base(minX_in, minY_in, size_in)
            {
                topL = topR = botL = botR = null;
                pts = new List<QDPoint>();
            }

            public bool addPoint(QDPoint pt) {
                // Perform the check that the QDPoint is actually inside the region
                if (!this.contains(pt)) return false;

                // If we're at size and have no children create them
                if (pts.Count >= child_per_branch && topL == null)
                {
                    subdivide();
                    pts.Add(pt);
                    foreach (QDPoint thisPt in pts)
                    {
                        if (topL.addPoint(thisPt)) continue;// Alternatively consider making the contains check a part of the addPoint function itself so it will return whether its possible to add or not
                        if (topR.addPoint(thisPt)) continue;
                        if (botL.addPoint(thisPt)) continue;
                        if (botR.addPoint(thisPt)) continue;
                    }
                    pts.Clear();
                }
                // if we already have children, add it to them
                else if (topL != null)
                {
                    if (topL.addPoint(pt)) return true;
                    if (topR.addPoint(pt)) return true;
                    if (botL.addPoint(pt)) return true;
                    if (botR.addPoint(pt)) return true;
                }
                // otherwise add it to the current node list
                else
                {
                    pts.Add(pt);
                }
                return true;
            }

            public void getPoints(Rectangle search, List<QDPoint> query_pts) {

                // Check to make sure the search criteria applies to this node
                if (!this.intersect(search)) return;

                // If this node contains points and no child nodes, return the points
                if (pts.Count > 0)
                {
                    foreach (QDPoint pt in pts)
                    {
                        if (search.contains(pt))
                            query_pts.Add(pt);
                    }
                    return;
                }

                // If there are children, add their points
                if (topL != null)
                {
                    topL.getPoints(search, query_pts);
                    topR.getPoints(search, query_pts);
                    botL.getPoints(search, query_pts);
                    botR.getPoints(search, query_pts);
                }

                // Return whatever has been added (an empty set if this node is empty
                //return query_pts;
            }

            private void subdivide() {
                topL = new QuadNode(minX, minY, size * 0.5f);
                topR = new QuadNode(minX + size * 0.5f, minY, size * 0.5f);
                botL = new QuadNode(minX, minY + size * 0.5f, size * 0.5f);
                botR = new QuadNode(minX + size * 0.5f, minY + size * 0.5f, size * 0.5f);
            }
        }
    }
}
