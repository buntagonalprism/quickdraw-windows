using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApplication1.QDInputConversion;
using WpfApplication1.QDShapes;
using System.Diagnostics;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        Point currentPoint = new Point();
        Point firstPoint = new Point();
        List<QDInputPointSet> ptSets = new List<QDInputPointSet>();
        bool isMouseDown = false;
        

        public MainWindow()
        {
            InitializeComponent();
            paintSurface.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(Canvas_MouseDown_1), true);
            paintSurface.AddHandler(InkCanvas.MouseUpEvent, new MouseButtonEventHandler(Canvas_MouseUp_1), true);
        }

        private void MyMouseDown(Point pt)
        {
            isMouseDown = true;
            currentPoint = pt;
            ptSets.Add(new QDInputPointSet());
            firstPoint = pt;
        }

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
            if (e.ButtonState == MouseButtonState.Pressed && !isMouseDown)
            {
                Debug.WriteLine("Mouse down from mouse down event: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
                MyMouseDown(e.GetPosition(this));
            }
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed && !isMouseDown)
            {
                Debug.WriteLine("Mouse down from mouse move event: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
                MyMouseDown(e.GetPosition(this));
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                Debug.WriteLine("mouse move event: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
                Line line = new Line();

                

                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                currentPoint = e.GetPosition(this);
     
                //paintSurface.Children.Add(line);

                if (ptSets.Last().addPoint(new QDPoint((float)e.GetPosition(this).X, (float)e.GetPosition(this).Y)))
                {
                    Debug.WriteLine("Corner found");
                    ptSets.Add(ptSets.Last().getCornerOverlapSet());
                    handleInputPointSet(ptSets[ptSets.Count - 2]);
                    // Code to handle line fitting goes in here
                }



                //QDLine qdline = new QDLine();
            }
        }

        private void Canvas_MouseUp_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Handle mouse up here by fitting a line to the current point set 
            Debug.WriteLine("Mouse Up");
            handleInputPointSet(ptSets.Last());
            //Line line = new Line();
            //line.X1 = firstPoint.X;
            //line.Y1 = firstPoint.Y;
            //Point thisPt = e.GetPosition(this);
            //line.X2 = thisPt.X;
            //line.Y2 = thisPt.Y;
            //line.Stroke = SystemColors.WindowFrameBrush;
            //paintSurface.Children.Add(line);

            isMouseDown = false;
            paintSurface.Strokes.RemoveAt(paintSurface.Strokes.Count - 1);

            //Ellipse ellipse = new Ellipse();
            //ellipse.Height = 100.0;
            //ellipse.Width = 200.0f;
            //ellipse.Stroke = SystemColors.WindowFrameBrush;
            //paintSurface.Children.Add(ellipse);

            PathFigure myPathFigure = new PathFigure();
            myPathFigure.StartPoint = new Point(20, 50);

            ArcSegment myLineSegment = new ArcSegment();
            myLineSegment.Point = new Point(220, 50);
            myLineSegment.Size = new Size(120, 50);
            myLineSegment.RotationAngle = 10;
            myLineSegment.IsLargeArc = true;
            myLineSegment.SweepDirection = SweepDirection.Clockwise;
            myLineSegment.IsStroked = true;
            

            PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            myPathSegmentCollection.Add(myLineSegment);

            myPathFigure.Segments = myPathSegmentCollection;

            PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            myPathFigureCollection.Add(myPathFigure);

            PathGeometry myPathGeometry = new PathGeometry();
            myPathGeometry.Figures = myPathFigureCollection;

            Path myPath = new Path();
            myPath.Stroke = Brushes.Black;
            myPath.StrokeThickness = 1;
            myPath.Data = myPathGeometry;

            //paintSurface.Children.Add(myPath);

        }

        private void paintSurface_StylusMove(object sender, StylusEventArgs e)
        {
            double x = e.GetPosition(this).X;
            double y = e.GetPosition(this).Y;
        }

        private void paintSurface_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (!isMouseDown)
            {
                Debug.WriteLine("Mouse down from stylus down event: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
                MyMouseDown(e.GetPosition(this));
            }
        }

        private void handleInputPointSet(QDInputPointSet pointSet)
        {

            QDShapeFitting fitter = new QDShapeFitting();
            bool shapeFitted = fitter.analyse(pointSet);
            
            //pointSet = scaleToAbs(pointSet);
            if (shapeFitted)
            {
                //QDLine line = (QDLine) pointSet.fittedShape;
                //float x = line.start.x;
                //paintSurface.Children.Add(line.getPath());
                paintSurface.Children.Add(pointSet.fittedShape.getPath());
                //independent.analyse(pointSet);
                //spatial.analyse(pointSet);
                //pointSet.fittedShape.paint = new Paint(current_paint);
            }
            //shapeDB.addInputPointSet(pointSet);
            //imageButtonAdapter.clear();
            //imageButtonAdapter.addAll(mapConstraintsToImages(pointSet.constraints));
            //invalidate();

            //new FitShapeTask().execute(pointSet);

        }
    }



}
