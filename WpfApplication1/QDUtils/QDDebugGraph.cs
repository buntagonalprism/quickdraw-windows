using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace WpfApplication1.QDUtils
{
    public class QDDebugGraph
    {
        float cnrX, cnrY, height, width; // Pixel units for describing the physical positioning of the graph of the screen
        float xStart, yStart, xFinish, yFinish, xStep, yStep; // Arbitrary units for drawing the drawing
        float markerSize = 10f;
        float textMarginSize = 5;
        Line xAxis, yAxis;
        List<Line> xLineMarkers = new List<Line>();
        List<Line> yLineMarkers = new List<Line>();
        List<TextBlock> xTextMarkers = new List<TextBlock>();
        List<TextBlock> yTextMarkers = new List<TextBlock>();
        InkCanvas canvas;
        List<Ellipse> points = new List<Ellipse>();
        float xPxToUnits, yPxToUnits;

        public QDDebugGraph(InkCanvas canvas_in)
        {
            canvas = canvas_in;
        }

        public void setLocation(float cnrX_in, float cnrY_in, float height_in, float width_in)
        {
            cnrX = cnrX_in;
            cnrY = cnrY_in;
            height = height_in;
            width = width_in;
        }

        public void setAxes(float xStart_in, float xFinish_in, float xStep_in, float yStart_in, float yFinish_in, float yStep_in)
        {
            xStart = xStart_in;
            xFinish = xFinish_in;
            xStep = xStep_in;
            yStart = yStart_in;
            yFinish = yFinish_in;
            yStep = yStep_in;

        }

        public void construct()
        {
            xAxis = new Line();
            xAxis.X1 = cnrX - markerSize;
            xAxis.Y1 = cnrY + height;
            xAxis.X2 = cnrX + width;
            xAxis.Y2 = xAxis.Y1;
            yAxis = new Line();
            yAxis.X1 = cnrX;
            yAxis.Y1 = cnrY;
            yAxis.X2 = yAxis.X1;
            yAxis.Y2 = cnrY + height + markerSize;
            yAxis.Stroke = SystemColors.WindowFrameBrush;
            yAxis.Stroke = SystemColors.WindowFrameBrush;

            xPxToUnits = width / (xFinish - xStart);
            for (float x = xStart; x < xFinish; x += xStep)
            {
                Line xMarker = new Line();
                xMarker.Y1 = cnrY;
                xMarker.X1 = (x - xStart) * xPxToUnits + cnrX;
                xMarker.X2 = xMarker.X1;
                xMarker.Y2 = cnrY + height + markerSize;
                xMarker.Stroke = SystemColors.WindowFrameBrush;
                xLineMarkers.Add(xMarker);

                TextBlock tbx = new TextBlock();
                tbx.Text = String.Format("{0:F1}", x);
                InkCanvas.SetLeft(tbx, xMarker.X1 + textMarginSize);
                InkCanvas.SetTop(tbx, xMarker.Y2 + textMarginSize);
                xTextMarkers.Add(tbx);
            }

            yPxToUnits = height / (yFinish - yStart);
            for (float y = yStart; y < yFinish; y += yStep)
            {
                Line yMarker = new Line();
                yMarker.X1 = cnrX - markerSize;
                yMarker.Y1 = cnrY + height - (y - yStart) * yPxToUnits;
                yMarker.X2 = cnrX + width;
                yMarker.Y2 = yMarker.Y1;
                yMarker.Stroke = SystemColors.WindowFrameBrush;
                yLineMarkers.Add(yMarker);

                TextBlock tbx = new TextBlock();
                tbx.Text = String.Format("{0:F1}", y);
                InkCanvas.SetLeft(tbx, yMarker.X1 - 5 * textMarginSize);
                InkCanvas.SetTop(tbx, yMarker.Y2 + textMarginSize);
                yTextMarkers.Add(tbx);
            }

            foreach (Line line in xLineMarkers)
            {
                canvas.Children.Add(line);
            }
            foreach (Line line in yLineMarkers)
            {
                canvas.Children.Add(line);
            }
            foreach (TextBlock text in yTextMarkers)
            {
                canvas.Children.Add(text);
            }
            foreach (TextBlock text in xTextMarkers)
            {
                canvas.Children.Add(text);
            }

        }

        public void AddPoint(float x, float y, Color colour)
        {
            float yPx = cnrY + height - (y - yStart) * yPxToUnits;
            float xPx = (x - xStart) * xPxToUnits + cnrX;
            Ellipse ellipse = QDDebugUtils.drawDebugCircle(xPx, yPx, 7, 1, colour);
            points.Add(ellipse);
            canvas.Children.Add(ellipse);
        }

        public void clearGraph()
        {
            foreach (Ellipse ellipse in points)
            {
                canvas.Children.Remove(ellipse);
            }
        }

        // To be used when the InkCanvas has been reset and we just want the graph body back
        public void redrawEmptyGraph()
        {
            clearGraph();
            foreach (Line line in xLineMarkers)
            {
                canvas.Children.Add(line);
            }
            foreach (Line line in yLineMarkers)
            {
                canvas.Children.Add(line);
            }
            foreach (TextBlock text in yTextMarkers)
            {
                canvas.Children.Add(text);
            }
            foreach (TextBlock text in xTextMarkers)
            {
                canvas.Children.Add(text);
            }
        }
    }
}
