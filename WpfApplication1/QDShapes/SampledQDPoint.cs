using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApplication1.QDUtils;

namespace WpfApplication1.QDShapes
{
    public class SampledQDPoint : QDPoint
    {
        public QDPointTypes type;

        public SampledQDPoint(SampledQDPoint other) :base(other) {
            type = other.type;
        }

        public SampledQDPoint(QDPoint pt, QDPointTypes type_in)  : base(pt)
        {
            type = type_in;
        }

    }
}
