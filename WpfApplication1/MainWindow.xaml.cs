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
        List<QDInputPointSet> ptSets = new List<QDInputPointSet>(); 

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_MouseDown_1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine("Mouse down at: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                currentPoint = e.GetPosition(this);
                ptSets.Add(new QDInputPointSet());
            }
        }

        private void Canvas_MouseMove_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Line line = new Line();

                line.Stroke = SystemColors.WindowFrameBrush;
                line.X1 = currentPoint.X;
                line.Y1 = currentPoint.Y;
                line.X2 = e.GetPosition(this).X;
                line.Y2 = e.GetPosition(this).Y;

                currentPoint = e.GetPosition(this);
     
                paintSurface.Children.Add(line);

                //if (ptSets.Last().addPoint(new QDPoint((float)line.X1, (float)line.X2)))
                //{
                //    Debug.WriteLine("Corner found");
                    // Code to handle line fitting goes in here
                //}



                //QDLine qdline = new QDLine();
            }
        }

        private void Canvas_MouseUp_1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Handle mouse up here by fitting a line to the current point set 
        }

        private void paintSurface_StylusMove(object sender, StylusEventArgs e)
        {
            double x = e.GetPosition(this).X;
            double y = e.GetPosition(this).Y;
        }

        private void paintSurface_StylusDown(object sender, StylusDownEventArgs e)
        {
            double x = e.GetPosition(this).X;
            double y = e.GetPosition(this).Y;
            Debug.WriteLine("Mouse down at: " + e.GetPosition(this).X + ", " + e.GetPosition(this).Y);
        }
    }



}
