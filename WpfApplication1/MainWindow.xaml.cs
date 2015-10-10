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
using WpfApplication1.QDUtils;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;
using System.Xml.Serialization;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public Point currentPoint = new Point();
        public Point firstPoint = new Point();
        public List<QDInputPointSet> ptSets = new List<QDInputPointSet>();
        public QDShapeDatabase shapeDB = new QDShapeDatabase();
        public QDConstraints constraints;
        public Color debugPointsColourForSeg;
        private QDDebugGraph graphTest;

        public bool isMouseDown = false;
        

        public MainWindow()
        {
            InitializeComponent();
            paintSurface.AddHandler(InkCanvas.MouseDownEvent, new MouseButtonEventHandler(Canvas_MouseDown_1), true);
            paintSurface.AddHandler(InkCanvas.MouseUpEvent, new MouseButtonEventHandler(Canvas_MouseUp_1), true);
            constraints = new QDConstraints(shapeDB);
            graphTest = new QDDebugGraph(paintSurface);
            graphTest.setAxes(0, 50, 5, -180, 180, 30);
            graphTest.setLocation(1200, 450, 450, 600);
            graphTest.construct();
            
        }

        private void MyMouseDown(Point pt)
        {
            isMouseDown = true;
            currentPoint = pt;
            ptSets.Add(new QDInputPointSet());
            ptSets.Last().addPoint(new QDPoint((float)pt.X, (float) pt.Y));
            firstPoint = pt;
            debugPointsColourForSeg = getRandomColour();
        }

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
           
            if (e.ButtonState == MouseButtonState.Pressed && !isMouseDown)
            {
                Debug.WriteLine("Mouse down from mouse down event: " + e.GetPosition(paintSurface).X + ", " + e.GetPosition(paintSurface).Y);
                MyMouseDown(e.GetPosition(paintSurface));
            }
        }

        private Color getRandomColour()
        {
            Random rand = new Random();
            byte r = (byte)Math.Floor(255 * rand.NextDouble());
            byte g = (byte)Math.Floor(255 * rand.NextDouble());
            byte b = (byte)Math.Floor(255 * rand.NextDouble());
            return Color.FromRgb(r, g, b);
        }

        public Ellipse drawDebugCircle(float x, float y, float radius, float strokeWidth, Color colour) {
            Ellipse ellipse = new Ellipse();
            ellipse.Height = radius;
            ellipse.Width = radius;
            SolidColorBrush brush = new SolidColorBrush();
            brush.Color = colour;
            ellipse.Stroke = brush;
            ellipse.StrokeThickness = strokeWidth;
            Matrix matrix = new Matrix();
            matrix.Translate(x, y);
            //ellipse.LayoutTransform = new MatrixTransform(matrix);
            ellipse.RenderTransform = new MatrixTransform(matrix);
            //EllipseGeometry ellipGeo = new EllipseGeometry();
            //Transform transform = new Ge
            return ellipse;
            //paintSurface.Children.Add(ellipse);
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            
            if (e.LeftButton == MouseButtonState.Pressed && !isMouseDown)
            {
                Debug.WriteLine("Mouse down from mouse move event: " + e.GetPosition(paintSurface).X + ", " + e.GetPosition(paintSurface).Y);
                MyMouseDown(e.GetPosition(paintSurface));
            }
            else if (e.LeftButton == MouseButtonState.Pressed)
            {
                float x = (float) e.GetPosition(paintSurface).X;
                float y = (float) e.GetPosition(paintSurface).Y;
                Debug.WriteLine("mouse move event: " + x + ", " + y);

                

                //Line line = new Line();
                //line.Stroke = SystemColors.WindowFrameBrush;
                //line.X1 = currentPoint.X;
                //line.Y1 = currentPoint.Y;
                //line.X2 = e.GetPosition(this).X;
                //line.Y2 = e.GetPosition(this).Y;
                //currentPoint = e.GetPosition(this);
                //paintSurface.Children.Add(line);

               
                if (ptSets.Last().addPoint(new QDPoint(x,y)))
                {
                    Debug.WriteLine("Corner found");
                    ptSets.Add(ptSets.Last().getCornerOverlapSet());
                    handleInputPointSet(ptSets[ptSets.Count - 2]);
                    debugPointsColourForSeg = getRandomColour();
                    // Code to handle line fitting goes in here
                }

                // For debugging
                paintSurface.Children.Add(QDDebugUtils.drawDebugCircle(x + 500f, y, 10, 2, debugPointsColourForSeg));
                if (ptSets.Last().anglesD.Count > 0)
                {
                    graphTest.AddPoint(ptSets.Last().anglesD.Count, ptSets.Last().anglesD.Last(), debugPointsColourForSeg);
                }


            }
        }

        private void Canvas_MouseUp_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Check that a valid mouse down event had previously occured on the canvas
            // It could have come pressed down from the title bar, which will cause an error
            if (isMouseDown)
            {
                // Handle mouse up here by fitting a line to the current point set 
                Debug.WriteLine("Mouse Up");
                handleInputPointSet(ptSets.Last());

                isMouseDown = false;
                paintSurface.Strokes.RemoveAt(paintSurface.Strokes.Count - 1);
            }


            //Line line = new Line();
            //line.X1 = firstPoint.X;
            //line.Y1 = firstPoint.Y;
            //Point thisPt = e.GetPosition(this);
            //line.X2 = thisPt.X;
            //line.Y2 = thisPt.Y;
            //line.Stroke = SystemColors.WindowFrameBrush;
            //paintSurface.Children.Add(line);

       

            //PathFigure myPathFigure = new PathFigure();
            //myPathFigure.StartPoint = new Point(20, 50);

            //ArcSegment myLineSegment = new ArcSegment();
            //myLineSegment.Point = new Point(220, 50);
            //myLineSegment.Size = new Size(120, 50);
            //myLineSegment.RotationAngle = 10;
            //myLineSegment.IsLargeArc = true;
            //myLineSegment.SweepDirection = SweepDirection.Clockwise;
            //myLineSegment.IsStroked = true;

            //PathSegmentCollection myPathSegmentCollection = new PathSegmentCollection();
            //myPathSegmentCollection.Add(myLineSegment);

            //myPathFigure.Segments = myPathSegmentCollection;

            //PathFigureCollection myPathFigureCollection = new PathFigureCollection();
            //myPathFigureCollection.Add(myPathFigure);

            //PathGeometry myPathGeometry = new PathGeometry();
            //myPathGeometry.Figures = myPathFigureCollection;

            //Path myPath = new Path();
            //myPath.Stroke = Brushes.Black;
            //myPath.StrokeThickness = 1;
            //myPath.Data = myPathGeometry;

            //paintSurface.Children.Add(myPath);

        }

        private void paintSurface_StylusMove(object sender, StylusEventArgs e)
        {
            double x = e.GetPosition(paintSurface).X;
            double y = e.GetPosition(paintSurface).Y;
        }

        private void paintSurface_StylusDown(object sender, StylusDownEventArgs e)
        {
            if (!isMouseDown)
            {
                Debug.WriteLine("Mouse down from stylus down event: " + e.GetPosition(paintSurface).X + ", " + e.GetPosition(paintSurface).Y);
                MyMouseDown(e.GetPosition(paintSurface));
            }
        }

        private void handleInputPointSet(QDInputPointSet pointSet)
        {

            QDShapeFitting fitter = new QDShapeFitting();
            bool shapeFitted = fitter.analyse(pointSet);
            
            //pointSet = scaleToAbs(pointSet);
            if (shapeFitted)
            {              
                constraints.analyse(pointSet);
                shapeDB.addInputPointSet(pointSet);

                // SERIOUS HACKERY
                //paintSurface.Children.Clear();
                foreach (QDInputPointSet ptSet in shapeDB.getPtSets())
                {
                    ptSet.fittedShape.path = new System.Windows.Shapes.Path(); // this wipes the old one in case we changed it in constraints
                    paintSurface.Children.Add(ptSet.fittedShape.getPath());
                }
            }
        }

        private void handleMenuNew(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("File new selected");
            ptSets.Clear();
            shapeDB.reset();
            paintSurface.Children.Clear();
            graphTest.redrawEmptyGraph();
        }

        private void handleMenuSave(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("File save selected");
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".qdr";
            dlg.Filter = "QuickDraw Files (*.qdr)|*.qdr";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Debug.WriteLine("File to save: " + fileName);
                XmlSerializer x = new XmlSerializer(ptSets.GetType());
                using (StreamWriter stream = new StreamWriter(fileName))
                {
                    x.Serialize(stream, ptSets);
                }

            }
        }

        private void handleMenuOpen(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("File open selected");
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".qdr";
            dlg.Filter = "QuickDraw Files (*.qdr)|*.qdr";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Debug.WriteLine("File to open: " + fileName);
                XmlSerializer x = new XmlSerializer(ptSets.GetType());
                using (StreamReader stream = new StreamReader(fileName))
                {
                    ptSets.Clear();
                    paintSurface.Children.Clear();
                    List<QDInputPointSet> tempPtSets = (List<QDInputPointSet>) x.Deserialize(stream);
                    // Try any validation here?
                    ptSets = tempPtSets;
                    foreach (QDInputPointSet ptSet in ptSets)
                    {
                        paintSurface.Children.Add(ptSet.fittedShape.getPath());
                    }
                }

            }
        }

        private void handleMenuDebugPtsOutput(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Debug points output selected");

            using (StreamWriter writer = new StreamWriter(Properties.Settings.Default.DebutPtsPath))
            {
                foreach (QDInputPointSet ptSet in ptSets)
                {
                    foreach (QDPoint pt in ptSet.pts)
                    {
                        writer.WriteLine(String.Format("{0:F2}, {1:F2}", pt.x, pt.y));
                    }
                    writer.WriteLine();
                }

            }
        }

        private void handleMenuDebugOutputChangeLocation(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Debut output location change selected");
            SaveFileDialog dlg = new SaveFileDialog();
            //dlg.DefaultExt = ".qdr";
            //dlg.Filter = "QuickDraw Files (*.qdr)|*.qdr";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                string fileName = dlg.FileName;
                Properties.Settings.Default.DebutPtsPath = fileName;
                Properties.Settings.Default.Save();
            }
        }
    }
}
