using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace WpfApplication1.QDShapes
{
    public abstract class QDShape
    {
        public Path path = new Path();

        public abstract List<SampledQDPoint> getIntermediatePoints(float spacing);
        

        public abstract Path getPath();

        //public abstract Path getPath(Point windowOrigin, float scaleFactor);

        public abstract String getSvgString();

       // public transient Path path = new Path();

        //public transient Paint paint = new Paint();


        // The following implements serialising
        private static long serialVersionUID = 2563841975326L;

        //private void writeObject(ObjectOutputStream oos) throws IOException {

        //    // Default fields
        //    oos.defaultWriteObject();
        //    // write the object

        //    oos.writeInt(paint.getColor());
        //    oos.writeInt(paint.getAlpha());
        //    oos.writeFloat(paint.getStrokeWidth());
        //    oos.writeInt(Paint.Style.STROKE.ordinal());
        //    oos.writeInt(Paint.Join.ROUND.ordinal());

        //}

        //private void readObject(ObjectInputStream ois) throws ClassNotFoundException, IOException {
        //    // default deserialization to fill the standard fields
        //    ois.defaultReadObject();
        //    // read back the additional data in the order we wrote it
        //    path = new Path();
        //    paint = new Paint();
        //    paint.setColor(ois.readInt());
        //    paint.setAlpha(ois.readInt());
        //    paint.setStrokeWidth(ois.readFloat());
        //    paint.setStyle(Paint.Style.values()[ois.readInt()]);
        //    paint.setStrokeJoin(Paint.Join.values()[ois.readInt()]);
        //}

        protected QDPoint absToLocCoords(QDPoint absPt, float SF, QDPoint origin)
        {
            return new QDPoint((absPt.x - origin.x) * SF, (absPt.y - origin.y) * SF);
        }
    }
}
