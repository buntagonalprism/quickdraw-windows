using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApplication1.QDUtils
{
    public enum QDConstraintTypes
    {
        STRAIGHT_LINE,
        VERTICAL_LINE,
        HORIZONTAL_LINE,
        INTERSECTING_LINES,
        CONNECTED_LINES,
        CONNECTION_AT_ENDPOINT,
        CONNECTION_AT_MIDPOINT,
        TANGENT,
        CONNECTION_AT_CENTRE,
        PARALLEL_TO,
        OFFSET_FROM,
        MIRROR_VERTICAL_FROM,
        MIRROR_HORIZONTAL_FROM,
        EQUAL_LENGTH_TO,
        SQUARE,
        RECTANGLE,
        POLYGON,
        KITE,
        RHOMBUS,
        CIRCLE,
        ARC
    }
}
