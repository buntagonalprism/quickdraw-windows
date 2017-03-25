# quickdraw-windows
Windows (WPF) version of QuickDraw app which fits lines and arcs to hand-drawn sketches.

This application is a sketching program designed to take hand-drawn sketches from finger or stylus input on touchscreens and convert them on-the-fly to precise vector drawings. Currently supported features include straight lines, elliptical curves and corners between straight or curved lines. 

## Example
In the image below, the coloured dots on the right represent individual input points captured by a touchscreen as a stylus was used to draw an ice cream scoop and waffle cone. The lines in black on the left are linear and elliptical curve best fits of the input points, with lines sections segmented when corners are detected. 

![Alt text](IceCreamSketch.PNG?raw=true "Converted Sketch")

## Requirements
Visual Studio 2015 or later

## Known Issues
The following issues have not yet been solved:
* Corner detection: gentle corners may be treated as a single continuous curved line instead
* Corner closure: when a user draws two lines meeting in a corner in a single movement, the corner will be detected but the lines are not explicitly joined
* Curve thresholds: slight curves are interpreted as straight lines

