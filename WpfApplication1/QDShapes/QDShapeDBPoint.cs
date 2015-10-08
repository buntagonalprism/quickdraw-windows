using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    public class QDShapeDBPoint : QDPoint
    {
        public QDPointTypes type;
        public QDShape shape = null;

        public QDShapeDBPoint(QDShapeDBPoint other) :base(other) {
            type = other.type;
        }

        public QDShapeDBPoint(QDPoint pt, QDPointTypes type_in)  : base(pt)
        {
            type = type_in;
        }

    }
}
