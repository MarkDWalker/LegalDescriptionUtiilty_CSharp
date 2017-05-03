using System;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Interop;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MWCadNameSpace
{
    public class MWCad
    {
        AcadApplication cadApp;

        public MWCad()
        {

            Console.WriteLine("Press Q to quit, C to continue...");
            //var an = Console.ReadLine();
            // if (an.ToUpper() != "Q")
            // {
            string progId = "AutoCAD.Application";
            cadApp = null;

            try
            {
                cadApp = Marshal.GetActiveObject(progId) as AcadApplication;
                Console.WriteLine("Existing AutoCAD instance found.");

            }
            catch
            {
                Console.WriteLine("No existing AutoCAD instance found!");
                try
                {
                    Type t = Type.GetTypeFromProgID(progId);
                    cadApp = Activator.CreateInstance(t) as AcadApplication;
                    Console.WriteLine("New AutoCAD instance started.");
                }
                catch
                {
                    Console.WriteLine("Cannot start AutoCAD!");
                }
            }

            if (cadApp != null)
            {
                cadApp.Visible = true;
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadLine();



        }

        public bool cadConnection()
        {
            bool returnBool = false;
            if (cadApp != null)
            {
                returnBool = true;
            }

            return returnBool;
        }

        public void setCadScreen(int left, int top, int height, int width)
        {

            cadApp.WindowState = Autodesk.AutoCAD.Interop.Common.AcWindowState.acNorm;
            cadApp.Height = height;
            cadApp.Width = width;
            cadApp.WindowLeft = left;
            cadApp.WindowTop = top;

            cadApp.Visible = true;



        }

        public pnt3d getPickPoint(pnt3d origin)
        {
            double[] P = new double[3] { origin.getX(), origin.getY(), 0 };
            double[] tempPnt = cadApp.ActiveDocument.Utility.GetPoint(P, "_select new Origin Point");
            pnt3d pnt = new pnt3d(tempPnt[0], tempPnt[1], 0);
            return pnt;
        }

        public double[] getPlineCoords()
        {

            //works but needs honing//


            Autodesk.AutoCAD.Interop.AcadSelectionSets sSets = cadApp.ActiveDocument.SelectionSets;
            Autodesk.AutoCAD.Interop.AcadSelectionSet sSet = null;
            try
            {
                sSet = sSets.Add("set1");
            }

            catch
            {
                int set1Index = 0;
                for (int i = 0; i < sSets.Count; ++i)
                {
                    if (sSets.Item(i).Name == "set1")
                    {
                        set1Index = i;
                    }
                }

                sSet = sSets.Item(set1Index);
                sSet.Clear();
            }



            cadApp.Visible = true;
            sSet.SelectOnScreen();
            Autodesk.AutoCAD.Interop.Common.AcadLWPolyline pline = null;
            for (int i = 0; i < sSet.Count; ++i)
            {
                //Console.WriteLine 


                Autodesk.AutoCAD.Interop.Common.AcadEntity obj = sSet.Item(i) as Autodesk.AutoCAD.Interop.Common.AcadEntity;


                try
                {
                    pline = cadApp.ActiveDocument.HandleToObject(obj.Handle);

                    double[] test = pline.Coordinates as double[];
                    Console.WriteLine("Coord 3 " + test[3]);
                    Console.WriteLine("The buldge 0 is " + pline.GetBulge(0));
                    Console.WriteLine("The buldge 1 is " + pline.GetBulge(1));
                    Console.WriteLine("The buldge  2 is " + pline.GetBulge(2));
                    Console.WriteLine("The buldge  3 is " + pline.GetBulge(3));
                }

                catch
                {
                    Console.WriteLine("entity type error");
                }
            }
            return pline.Coordinates;
        }

        public List<VTex> getPlineEnityCoords()
        {
            List<VTex> returnVertices = new List<VTex>();

            Object out1 = new object();
            object out2 = new object();

            try
            {
                cadApp.ActiveDocument.Utility.GetEntity(out out1, out out2, "Select polyline:");

                if (out1 == null)
                {
                    MessageBox.Show("Nothing Selected!");
                }
                Autodesk.AutoCAD.Interop.Common.AcadLWPolyline pline = out1 as Autodesk.AutoCAD.Interop.Common.AcadLWPolyline;
                double[] coords = pline.Coordinates as double[];


                for (int i = 0; i < (coords.Length + 1) / 2; ++i)
                {
                    pnt3d pnt = new pnt3d(coords[2 * i], coords[2 * i + 1], 0);
                    VTex v = new VTex(pnt, pline.GetBulge(i));

                    returnVertices.Add(v);
                }

            }
            catch
            {
                MessageBox.Show("Error Selecting Entity!");
            }
            return returnVertices;
        }

        public bool checkCadConnection()
        {
            bool returnVal = false;
            if (cadApp != null)
            {
                returnVal = true;
            }
            return returnVal;
        }

        public string getTitle()
        {
            string returnVal = "nothing";

            if (cadApp != null)
            {
                try
                {
                    returnVal = cadApp.ActiveDocument.Name;
                }
                catch
                {
                    returnVal = "Call rejected";
                }
            }

            return returnVal;
        }


        public void changeAppSize()
        {



            cadApp.WindowState = Autodesk.AutoCAD.Interop.Common.AcWindowState.acNorm;
            cadApp.WindowLeft = 0;
            cadApp.WindowTop = 100;
            cadApp.Width = 500;
            cadApp.Height = 700;



        }

        public Autodesk.AutoCAD.Interop.Common.AcadLWPolyline drawPline(List<MWCadNameSpace.VTex> Vertices)
        {

            double[] coords = new double[Vertices.Count * 2];

            for (int i = 0; i < Vertices.Count; ++i)
            {
                coords.SetValue(Vertices[i].getPnt3d().getX(), 2 * i);
                coords.SetValue(Vertices[i].getPnt3d().getY(), 2 * i + 1);

            }

            Autodesk.AutoCAD.Interop.Common.AcadLWPolyline pLineObj = null;

            try
            {
                pLineObj = cadApp.ActiveDocument.ModelSpace.AddLightWeightPolyline(coords);
                pLineObj.color = Autodesk.AutoCAD.Interop.Common.ACAD_COLOR.acRed;
            }
            catch
            {
                MessageBox.Show("Error drawing polyline. Please try again.");
            }


            return pLineObj;

        }


        public void setPlineBuldges(Autodesk.AutoCAD.Interop.Common.AcadLWPolyline pline, List<VTex> vertices)
        {
            for (int i = 0; i < vertices.Count; ++i)
            {
                pline.SetBulge(i, vertices[i].getBuldge());
            }
        }

        public void DrawLine(double StartX1, double StartY1, double EndX2,
            double EndY2, string LineTypebool, bool DrawDonutsOnLineStart,
            bool DrawDonutsOnLineEnds)


        {

            Autodesk.AutoCAD.Interop.Common.AcadLine lineObj;
            //AcadLine lineObj;
            double[] startPoint = new double[3];
            double[] endPoint = new double[3]; ;

            startPoint[0] = StartX1;
            startPoint[1] = StartY1;
            startPoint[2] = 0.0;
            endPoint[0] = EndX2;

            endPoint[1] = EndY2;
            endPoint[2] = 0.01;
            lineObj = cadApp.ActiveDocument.ModelSpace.AddLine(startPoint, endPoint);

            //if (LineType.Length > 0)
            //{
            //    lineObj.Linetype = LineType; //'"HIDDEN"
            //    lineObj.LinetypeScale = 10;
            //    lineObj.Update();
            //}



            //if (DrawDonutsOnLineStart == true)
            //{
            //    DrawDonut((AcadBlock)gbl_doc.ModelSpace,
            //                  0, 3.0, StartX1, StartY1);

            //}

            //if (DrawDonutsOnLineEnds == true)
            //{
            //    DrawDonut((AcadBlock)gbl_doc.ModelSpace,
            //                      0, 3.0, EndX2, EndY2);

            //}
            cadApp.ZoomAll();
        }


    }


    public class pnt3d
    {
        double x;
        double y;
        double z;

        public pnt3d(double _x = 0, double _y = 0, double _z = 0)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public double getX()
        {
            return x;
        }

        public double getY()
        {
            return y;
        }

        public double getZ()
        {
            return z;
        }

    }


    public class VTex
    {
        pnt3d pnt;
        double buldge;

        public VTex(pnt3d _pnt, double _buldge)
        {
            pnt = _pnt;
            buldge = _buldge;
        }

        public pnt3d getPnt3d()
        {
            return pnt;
        }

        public void updatePnt(pnt3d newPnt)
        {
            pnt = newPnt;
        }

        public double getBuldge()
        {
            return buldge;
        }

        public void setBuldge(double newBuldge)
        {
            this.buldge = newBuldge;
        }
    }


    public class Segment
    {
        VTex gStart; //Global Vertex Data
        VTex gEnd;   //Global Vertex Data

        public Segment(VTex _gStart, VTex _gEnd)
        {
            gStart = _gStart;
            gEnd = _gEnd;
        }

        public double setBuldgeFromRadius(double radius, CurveLeftRightCategory leftRight)
        {
            double returnVal = 0;

            double t1 = 2 * radius / length();

            double t2SR = Math.Sqrt(Math.Pow(radius, 2) - Math.Pow(length() / 2, 2));

            double b1 =Math.Abs( t1 + 2 * t2SR / length());

            double b2 = Math.Abs(t1 - 2 * t2SR / length());

            if (leftRight.Value == "Right")
            {
                if (b1 <= 1)
                {
                    returnVal = -b1;
                }
                else
                {
                    returnVal = -b2;
                }
            }
            else if (leftRight.Value == "Left")
            {
                if (b1 <= 1)
                {
                    returnVal = b1;
                }
                else
                {
                    returnVal = b2;
                }
            }
            else
            {
                returnVal = 0;
            }

            this.gStart.setBuldge(returnVal);

            return returnVal;
        }
        

        public void updateStartPnt(pnt3d newStart)
        {
            gStart.updatePnt(newStart);
        }

        public VTex getGStart()
        {
            return gStart;
        }

        public VTex getGEnd()
        {
            return gEnd;
        }

        public void updateEndPnt(pnt3d newEnd)
        {
            gEnd.updatePnt(newEnd);
        }



        public double dx()
        {
            double returnVal = gEnd.getPnt3d().getX() - gStart.getPnt3d().getX();
            return returnVal;
        }

        public double dy()
        {
            double returnVal = gEnd.getPnt3d().getY() - gStart.getPnt3d().getY();
            return returnVal;
        }

        public VTex lStart() //Local start vertex information always 0,0,0 and the global start bulge
        {
            pnt3d p = new pnt3d();
            VTex returnVal = new VTex(p, gStart.getBuldge());
            return returnVal;
        }

        public VTex lEnd() //Local start vertex information always 0,0,0 and the global start bulge
        {

            pnt3d p = new pnt3d(dx(), dy(), 0);
            VTex returnVal = new VTex(p, gEnd.getBuldge());
            return returnVal;
        }

        public double gCadAngle()
        {
            double returnVal = -1000;

            double sX = gStart.getPnt3d().getX();
            double sY = gStart.getPnt3d().getY();
            double eX = gEnd.getPnt3d().getX();
            double eY = gEnd.getPnt3d().getY();

            double dxASB = Math.Abs(eX - sX);
            double dyABS = Math.Abs(eY - sY);

            double phiRad = Math.Atan(dyABS / dxASB);
            double phi = phiRad * (180 / Math.PI);

            double dx = eX - sX;
            double dy = eY - sY;

            if (dx >= 0 && dy >= 0)
            {
                returnVal = phi;
            }
            else if (dx < 0 && dy >= 0)
            {
                returnVal = 180 - phi;
            }
            else if (dx < 0 && dy < 0)
            {
                returnVal = 180 + phi;
            }
            else if (dx >= 0 && dy < 0)
            {
                returnVal = 360 - phi;
            }
            return returnVal;
        }

        public double length()
        {
            double returnVal = Math.Sqrt((dx() * dx()) + (dy() * dy()));
            return returnVal;
        }

        public double cCadAngle() //corrected local
        {
            double angleInSeconds = Convert.ToInt32(Math.Round(gCadAngle() * 60 * 60, 0));
            double angleInDegrees = angleInSeconds / 3600;
            return angleInDegrees;
        }

        public double cLength() //corrected local length
        {
            double correctedLength = Math.Round(length(), 2);
            return correctedLength;
        }

        public double cdx() //corrected local end vertex
        {
            double returnVal = 0;
            if (cCadAngle() >= 0 && cCadAngle() <= 90)
            {
                returnVal = cLength() * Math.Cos(cCadAngle() * Math.PI / 180);
            }
            else if (cCadAngle() > 90 && cCadAngle() <= 180)
            {
                returnVal = -cLength() * Math.Cos((180 - cCadAngle()) * Math.PI / 180);
            }
            else if (cCadAngle() > 180 && cCadAngle() <= 270)
            {
                returnVal = -cLength() * Math.Cos((cCadAngle() - 180) * Math.PI / 180);
            }
            else if (cCadAngle() > 270 && cCadAngle() < 360)
            {
                returnVal = cLength() * Math.Cos((360 - cCadAngle()) * Math.PI / 180);
            }
            return returnVal;
        }

        public double cdy() //corrected local end vertex
        {
            double returnVal = 0;
            if (cCadAngle() >= 0 && cCadAngle() <= 90)
            {
                returnVal = cLength() * Math.Sin(cCadAngle() * Math.PI / 180);
            }
            else if (cCadAngle() > 90 && cCadAngle() <= 180)
            {
                returnVal = cLength() * Math.Sin((180 - cCadAngle()) * Math.PI / 180);
            }
            else if (cCadAngle() > 180 && cCadAngle() <= 270)
            {
                returnVal = -cLength() * Math.Sin((cCadAngle() - 180) * Math.PI / 180);
            }
            else if (cCadAngle() > 270 && cCadAngle() < 360)
            {
                returnVal = -cLength() * Math.Sin((360 - cCadAngle()) * Math.PI / 180);
            }
            return returnVal;
        }

        public VTex cLEnd()
        {
            pnt3d correctedEndPnt = new pnt3d(cdx(), cdy(), 0);
            VTex returnVertex = new VTex(correctedEndPnt, gEnd.getBuldge());
            return returnVertex;
        }
    }



    public class SurveyBoundary
    {
        List<Segment> originalSegments = new List<Segment>();
        List<Segment> correctedSegments = new List<Segment>();
        List<Segment> translatedCorrectedSegments = new List<Segment>();

        public SurveyBoundary(List<VTex> vList)
        {


            for (int i = 0; i < vList.Count; ++i)
                if (i != 0) //((Convert.ToDouble(i) + 1) / 2 == Math.Round((Convert.ToDouble(i) + 1) / 2))  //odd check
                {
                    Segment newSegment = new Segment(vList[i - 1], vList[i]);
                    originalSegments.Add(newSegment);
                }

            this.setCorrectedSegements();
            this.setTranslatedCorrectedSegements();
        }


        public void translateSegmentList(pnt3d selectedPnt)
        {
            int test1 = this.translatedCorrectedSegments.Count;
            Console.WriteLine("nothing");

            double ox = translatedCorrectedSegments[0].getGStart().getPnt3d().getX();
            double oy = translatedCorrectedSegments[0].getGStart().getPnt3d().getY();

            double tx = selectedPnt.getX() - ox;
            double ty = selectedPnt.getY() - oy;

            int i = 0;
            for (i = 0; i < translatedCorrectedSegments.Count; ++i)
            {
                double currentStartX = translatedCorrectedSegments[i].getGStart().getPnt3d().getX();
                double currentStartY = translatedCorrectedSegments[i].getGStart().getPnt3d().getY();
                double newStartx = currentStartX + tx;
                double newStarty = currentStartY + ty;
                pnt3d updatedStartPnt = new pnt3d(newStartx, newStarty, 0);
                translatedCorrectedSegments[i].updateStartPnt(updatedStartPnt);
            }

            double currentEndX = translatedCorrectedSegments[i - 1].getGEnd().getPnt3d().getX();
            double currentEndY = translatedCorrectedSegments[i - 1].getGEnd().getPnt3d().getY();
            double newEndx = currentEndX + tx;
            double newEndy = currentEndY + ty;
            pnt3d updatedEndPnt = new pnt3d(newEndx, newEndy, 0);
            translatedCorrectedSegments[i - 1].updateEndPnt(updatedEndPnt);

            int test = this.translatedCorrectedSegments.Count;
            Console.WriteLine("nothing");



        }


        private void setTranslatedCorrectedSegements()
        {


            for (int i = 0; i < originalSegments.Count; ++i)
            {
                if (i == 0)
                {
                    double cx = originalSegments[i].getGStart().getPnt3d().getX() + originalSegments[i].cdx();
                    double cy = originalSegments[i].getGStart().getPnt3d().getY() + originalSegments[i].cdy();
                    pnt3d correctedEndPnt = new pnt3d(cx, cy, 0);
                    VTex correctedEndVertex = new VTex(correctedEndPnt, originalSegments[i].getGEnd().getBuldge());

                    pnt3d newStartPoint = new pnt3d(originalSegments[i].getGStart().getPnt3d().getX(), originalSegments[i].getGStart().getPnt3d().getY(), 0);
                    double newBulgeStart = originalSegments[i].getGStart().getBuldge();
                    VTex correctedStartVertex = new MWCadNameSpace.VTex(newStartPoint, newBulgeStart);

                    Segment newCorrectedSegment = new MWCadNameSpace.Segment(correctedStartVertex, correctedEndVertex);
                    translatedCorrectedSegments.Add(newCorrectedSegment);
                }
                else
                {
                    VTex correctedStartVertex = translatedCorrectedSegments[i - 1].getGEnd();
                    //double check_cdx = originalSegments[i].cdx();
                    //double check_cdy = originalSegments[i].cdy();

                    //MessageBox.Show("Corrected dx and dy " + check_cdx + "," + check_cdy);

                    //double check_getx = correctedStartVertex.getPnt3d().getX();
                    //double check_gety = correctedStartVertex.getPnt3d().getY();

                    //MessageBox.Show("Corrected startx and starty " + check_getx + "," + check_gety);

                    double cx = correctedStartVertex.getPnt3d().getX() + originalSegments[i].cdx();
                    double cy = correctedStartVertex.getPnt3d().getY() + originalSegments[i].cdy();



                    double newEndx = cx;
                    double newEndy = cy;

                    pnt3d correctedEndPnt = new pnt3d(newEndx, newEndy, 0);
                    VTex correctedEndVertex = new VTex(correctedEndPnt, originalSegments[i].getGEnd().getBuldge());

                    Segment newCorrectedSegment = new MWCadNameSpace.Segment(correctedStartVertex, correctedEndVertex);
                    translatedCorrectedSegments.Add(newCorrectedSegment);
                }

            }
        }

        private void setCorrectedSegements()
        {
            for (int i = 0; i < originalSegments.Count; ++i)
            {
                if (i == 0)
                {
                    double cx = originalSegments[i].getGStart().getPnt3d().getX() + originalSegments[i].cdx();
                    double cy = originalSegments[i].getGStart().getPnt3d().getY() + originalSegments[i].cdy();
                    pnt3d correctedEndPnt = new pnt3d(cx, cy, 0);
                    VTex correctedEndVertex = new VTex(correctedEndPnt, originalSegments[i].getGEnd().getBuldge());

                    pnt3d newStartPoint = new pnt3d(originalSegments[i].getGStart().getPnt3d().getX(), originalSegments[i].getGStart().getPnt3d().getY(), 0);
                    double newBulgeStart = originalSegments[i].getGStart().getBuldge();
                    VTex correctedStartVertex = new MWCadNameSpace.VTex(newStartPoint, newBulgeStart);

                    Segment newCorrectedSegment = new MWCadNameSpace.Segment(correctedStartVertex, correctedEndVertex);
                    correctedSegments.Add(newCorrectedSegment);
                }
                else
                {
                    VTex correctedStartVertex = correctedSegments[i - 1].getGEnd();
                    //double check_cdx = originalSegments[i].cdx();
                    //double check_cdy = originalSegments[i].cdy();

                    //MessageBox.Show("Corrected dx and dy " + check_cdx + "," + check_cdy);

                    //double check_getx = correctedStartVertex.getPnt3d().getX();
                    //double check_gety = correctedStartVertex.getPnt3d().getY();

                    //MessageBox.Show("Corrected startx and starty " + check_getx + "," + check_gety);

                    double cx = correctedStartVertex.getPnt3d().getX() + originalSegments[i].cdx();
                    double cy = correctedStartVertex.getPnt3d().getY() + originalSegments[i].cdy();



                    double newEndx = cx;
                    double newEndy = cy;

                    pnt3d correctedEndPnt = new pnt3d(newEndx, newEndy, 0);
                    VTex correctedEndVertex = new VTex(correctedEndPnt, originalSegments[i].getGEnd().getBuldge());

                    Segment newCorrectedSegment = new MWCadNameSpace.Segment(correctedStartVertex, correctedEndVertex);
                    correctedSegments.Add(newCorrectedSegment);
                }

            }
        }

        public int segmentCount()
        {
            return originalSegments.Count;
        }

        public List<VTex> getCorrectedSegmentVertices()
        {
            List<VTex> returnList = new List<VTex>();
            int i = 0;
            for (i = 0; i < correctedSegments.Count; ++i)
            {

                returnList.Add(correctedSegments[i].getGStart());
            }

            returnList.Add(correctedSegments[i - 1].getGEnd());

            return returnList;
        }

        public List<VTex> getTranslatedCorrectedSegmentVertices()
        {
            List<VTex> returnList = new List<VTex>();
            int i = 0;
            for (i = 0; i < translatedCorrectedSegments.Count; ++i)
            {

                returnList.Add(translatedCorrectedSegments[i].getGStart());
            }

            returnList.Add(translatedCorrectedSegments[i - 1].getGEnd());

            return returnList;
        }

        public Segment getOriginalSegment(int index)
        {
            return originalSegments[index];
        }

        public Segment getCorrectedSegment(int index)
        {
            return correctedSegments[index];
        }

        public Segment getTranslatedCorrectedSegment(int index)
        {
            return translatedCorrectedSegments[index];
        }

    }

    public class MWNumberSelection:MWSelection
    {
        double numberValue;

        public void setNumberValue(double number)
        {
            numberValue = number;
        }

        public double getNumberValue()
        {
            return numberValue;
        }
    }

    public class MWSelection
    {
        int startIndex;
        int endIndex;


        public MWSelection(int _start = -1, int _end = -1)
        {
            startIndex = _start;
            endIndex = _end;
        }

        public int length()
        {
            return (endIndex - startIndex);
        }

        public int getStartIndex()
        {
            return startIndex;
        }

        public void setStartIndex(int index)
        {
            startIndex = index;
        }

        public int getEndIndex()
        {
            return endIndex;
        }

        public void setEndIndex(int index)
        {
            endIndex = index;
        }
    }

    public class StartDirectionCategory
    {
        private StartDirectionCategory(string value) { Value = value; }

        public string Value { get; set; }

        public static StartDirectionCategory North { get { return new StartDirectionCategory("North"); } }
        public static StartDirectionCategory South { get { return new StartDirectionCategory("South"); } }

    }


    public class EndDirectionCategory
    {
        private EndDirectionCategory(string value) { Value = value; }

        public string Value { get; set; }

        public static EndDirectionCategory East { get { return new EndDirectionCategory("East"); } }
        public static EndDirectionCategory West { get { return new EndDirectionCategory("West"); } }

    }

    public class CurveLeftRightCategory
    {
        private CurveLeftRightCategory(string value) { Value = value; }

        public string Value { get; set; }

        public static CurveLeftRightCategory Left { get { return new CurveLeftRightCategory("Left"); } }
        public static CurveLeftRightCategory Right { get { return new CurveLeftRightCategory("Right"); } }
        public static CurveLeftRightCategory NotFound { get { return new CurveLeftRightCategory("NotFound"); } }
        public static CurveLeftRightCategory NA { get { return new CurveLeftRightCategory("NA"); } }

    }


    public class DeconstructedCallString
    {
        string theString;
        StartDirectionCategory startDirection;
        EndDirectionCategory endDirection;

        int degrees;
        int minutes;
        int seconds;
        double length;

        int globalStart;
        int globalEnd;

        CurveLeftRightCategory curveLeftRight;
        double radius;

        public double radiusAsBuldge()
        {
            double returnVal = 0;

            return returnVal;
        }

       public Segment extractAsSegment(pnt3d startPnt)
        {
            //set the start point
            VTex startV = new VTex(startPnt, this.radiusAsBuldge());

            //find the end point
            double bearingAngle = (Convert.ToDouble(degrees) + Convert.ToDouble(minutes)/60.00 + Convert.ToDouble(seconds) / 3600.00) * (Math.PI/180) ;

            double dx = 0;
            double dy = 0;
            if (startDirection.Value == "North" && endDirection.Value == "East")
            {
                dx = (Math.Sin(bearingAngle) * length);
                dy = (Math.Cos(bearingAngle) * length);
            }
            else if (startDirection.Value == "North" && endDirection.Value == "West")
            {
                dx = -(Math.Sin(bearingAngle) * length);
                dy = (Math.Cos(bearingAngle) * length);
            }
            else if (startDirection.Value == "South" && endDirection.Value == "West")
            {
                dx = -(Math.Sin(bearingAngle) * length);
                dy = -(Math.Cos(bearingAngle) * length);
            }
            else if (startDirection.Value == "South" && endDirection.Value == "East")
            {
                dx = +(Math.Sin(bearingAngle) * length);
                dy = -(Math.Cos(bearingAngle) * length);
            }

            pnt3d endPnt = new pnt3d(startV.getPnt3d().getX() + dx, startV.getPnt3d().getY() + dy);

            VTex endV = new VTex(endPnt, 0);

            Segment returnSeg = new Segment(startV, endV);
            return returnSeg;
        }

        public string getBearingAsString()
        {
            string returnString = "";
            returnString = startDirection.Value + " " + Convert.ToString(degrees) + "d "
                            + Convert.ToString(minutes) + "m " + Convert.ToString(seconds) + "s "
                            + endDirection.Value;                

            return returnString;
        }

        public double getLength()
        {
            double returnVal = length;
            return returnVal;
        }

       public StartDirectionCategory getStartDirection()
        {
            return startDirection;
        }

        public EndDirectionCategory getEndDirection()
        {
            return endDirection;
        }

        public int getDegrees()
        {
            return degrees;
        }

        public int getMinutes()
        {
            return minutes;
        }

        public int getSeconds()
        {
            return seconds;
        }

        public int getStart()
        {
            return globalStart;
        }

        public int getEnd()
        {
            return globalEnd;
        }

        public CurveLeftRightCategory getCurveLeftRight()
        {
            return curveLeftRight;
        }

        public double getRadius()
        {
            return radius;
        }

        public DeconstructedCallString(string aCallString, int _gStartindex, int _gEndIndex)
        {
            LegalDescriptionStringUtility util = new LegalDescriptionStringUtility(aCallString);
            theString = aCallString;

            globalStart = _gStartindex;
            globalEnd = _gEndIndex;

            //is is a curve and is it left or right
            if (util.findCurveKeyword(0, theString) == false) {
                curveLeftRight = CurveLeftRightCategory.NA;
            }
            else
            {
                curveLeftRight = util.findRightLeftKeyword(theString);

            }

            Console.WriteLine("Stop for Test");

            if (curveLeftRight.Value == CurveLeftRightCategory.Left.Value 
                || curveLeftRight.Value == CurveLeftRightCategory.Right.Value)
            {
                //find the radius
                radius = util.extractRadius(theString);
            }


            //set north or south
            MWSelection NSlocation = util.findStartDirection(0, theString);
            int begIndex = NSlocation.getStartIndex();

            if (theString[begIndex] == 'N' || theString[begIndex] == 'n')
            {
                startDirection = StartDirectionCategory.North;
            }
            else
            {
                startDirection = StartDirectionCategory.South;
            }

            //set east or west
            MWSelection EWlocation = util.findEndDirection(0, theString);
            begIndex = EWlocation.getStartIndex();

            if (theString[begIndex] == 'E' || theString[begIndex] == 'e')
            {
                endDirection = EndDirectionCategory.East;
            }
            else
            {
                endDirection = EndDirectionCategory.West;
            }

            //Find the degrees and its location in the string
            MWNumberSelection degreesSelection = util.extractDegrees(NSlocation.getEndIndex(), theString);
            degrees = Convert.ToInt32(degreesSelection.getNumberValue());

            //find the minutes
            MWNumberSelection minutesSelection = util.extractMinutes(degreesSelection.getEndIndex(), theString);
            this.minutes = Convert.ToInt32(minutesSelection.getNumberValue());

            //find the seconds
            MWNumberSelection secondsSelection = util.extractSeconds(minutesSelection.getEndIndex(), theString);
            //three might not be any seconds
            if (secondsSelection.getEndIndex() < EWlocation.getStartIndex())
            {
                this.seconds = Convert.ToInt32(secondsSelection.getNumberValue());
            }
            else
            {
                this.seconds = 0;
            }
           

            //find the length
            //find the seconds
            MWNumberSelection lengthSelection = util.extractLength(EWlocation.getEndIndex(), theString);
            this.length = Convert.ToDouble(lengthSelection.getNumberValue());



            

        }
        
    }


    public class LegalDescriptionStringUtility
    {

        string theString;

        public LegalDescriptionStringUtility(string aString)
        {
            theString = aString;
        }

        public List<MWSelection> mergeCurveDataWithCallList(List<MWSelection> curveList, List<MWSelection> callList)
        {
            List<MWSelection> modifiedCallList = new List<MWSelection>();

            for (int i = 0; i<curveList.Count; ++i)
            {
                int curveIndexMatch = -1;
                int curveKeywordLocation = curveList[i].getEndIndex();
      
                int j = 0;
                do
                {
                    if (curveKeywordLocation < callList[j].getStartIndex())
                    {
                        curveIndexMatch = j;
                        callList[j].setStartIndex(curveList[i].getStartIndex());
                    } 

                    ++j;
                } while (curveIndexMatch == -1 && j < callList.Count);
             
            }


            return callList;
        }

        public List<MWSelection> checkForPotentialCurveData()
        {

            List<MWSelection> returnSelectionList = new List<MWSelection>();
            //find any potential curve data with the use of the curve keyword
            int i = 0;
            do
            {
                MWSelection nextCurve = findNextCurve(i);

                if (nextCurve.getEndIndex() == -1){
                    //no curve info found
                    i = theString.Length;
                    //loop will exit
                }
                else
                {
                    returnSelectionList.Add(nextCurve);
                    i = nextCurve.getEndIndex();
                }
             

            } while (i < theString.Length);




            return returnSelectionList;

        }

        public MWSelection findNextPotentialCall(int startingIndex)
        {
            MWSelection returnSelection = new MWSelection();

            MWSelection nextStart = findStartDirection(startingIndex);
            MWSelection nextEnd = findEndDirection(startingIndex);
            MWSelection nextFeet = findFeet(startingIndex);

            int lenghthOfBearing = nextEnd.getEndIndex() - nextStart.getEndIndex();
            bool potentialBearing = false;
            if (lenghthOfBearing>=10 && lenghthOfBearing <= 25)
            {
                potentialBearing = true;

            }

            int lengthOfDistance = nextFeet.getEndIndex() - nextEnd.getEndIndex();
            bool potentialDistance = false;

            if (lengthOfDistance >=8 && lengthOfDistance <= 200)
            {
                potentialDistance = true;
            }

            if (potentialBearing == false || potentialDistance == false)
            {
                returnSelection.setEndIndex(-1);
                returnSelection.setStartIndex(-1);
            }
            else
            {
                returnSelection.setStartIndex(nextStart.getStartIndex());
                returnSelection.setEndIndex(nextFeet.getEndIndex()+1);
            }
            //if it is a potential call then it will return the position
            //if it does not look like the right lengths it will return -1 and -1
            return returnSelection;
        }

        public MWSelection findNextCurve(int startingIndex, string aCallString = "")
        {

            string[] stringArray = new string[] { "Curve", "CURVE", "curve", "Arc", "ARC" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;
        }

        public MWSelection findStartDirection(int startingIndex, string aCallString = "")
        {

            string[] stringArray = new string[] { "North", "NORTH", "South", "SOUTH" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);
           
            return returnSelectionObject;
        }

        public MWSelection findEndDirection(int startingIndex, string aCallString = "")
        {
            string[] stringArray = new string[] { "East", "EAST", "West", "WEST" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;

        }

        public MWSelection findPOB(int startingIndex)
        {
            string[] stringArray = new string[] { "REAL POINT OF BEGINNING", "POINT OF BEGINNING", "Real Point of Beginning", "Point of Beginning" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex);

            return returnSelectionObject;

        }

        public MWSelection findPOC(int startingIndex)
        {
            string[] stringArray = new string[] { "COMMENCING", "Commencing" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex);

            return returnSelectionObject;

        }

        public MWSelection findFeet(int startingIndex, string aCallString = "")
        {
            string[] stringArray = new string[] { "Feet", "FEET", "feet","FT", "FT.", "Ft", "Ft." };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;

        }

        public MWSelection findRight(int startingIndex, string aCallString = "")
        {
    
            string[] stringArray = new string[] { "Right", "RIGHT", "right", "rt.", "RT.", "Rt."};

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;

        }

        public MWSelection findLeft(int startingIndex, string aCallString = "")
        {
            string[] stringArray = new string[] { "Left", "LEFT", "left", "lt.", "LT.", "Lt." };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;

        }

        public MWSelection findRadius(int startingIndex, string aCallString = "")
        {
            string[] stringArray = new string[] { "Radius", "RADIUS", "radius", "rad.", "rad" };

            MWSelection returnSelectionObject = new MWSelection();

            returnSelectionObject = findNextGeneral(stringArray, startingIndex, aCallString);

            return returnSelectionObject;

        }

        public double extractRadius(string aCallString)
        {
            double returnVal = -1;

            MWNumberSelection aNumberSelection = new MWNumberSelection();
            
            MWSelection radiusSelection = new MWSelection();
            radiusSelection = findRadius(0, aCallString);
        
            if (radiusSelection.getStartIndex()>-1)
            {
               aNumberSelection = extractNextNumberVal(radiusSelection.getEndIndex() + 1, aCallString);
            }

            returnVal = aNumberSelection.getNumberValue();

            return returnVal;
        }

        public MWNumberSelection extractDegrees(int prevIndex, string aCallString = "")
        {
            MWNumberSelection returnVal = new MWNumberSelection();

            //find the next number after the north south keyword
            returnVal = extractNextNumberVal(prevIndex, aCallString);

            return returnVal;
        }

        public MWNumberSelection extractMinutes(int prevIndex, string aCallString = "")
        {
            MWNumberSelection returnVal = new MWNumberSelection();

            //find the next number after the north south keyword
            returnVal = extractNextNumberVal(prevIndex, aCallString);

            return returnVal;
        }

        public MWNumberSelection extractSeconds(int prevIndex, string aCallString = "")
        {
            MWNumberSelection returnVal = new MWNumberSelection();

            //find the next number after the north south keyword
            returnVal = extractNextNumberVal(prevIndex, aCallString);


            return returnVal;
        }

        public MWNumberSelection extractLength(int EndDirectionIndex, string aCallString = "")
        {
            MWNumberSelection returnVal = new MWNumberSelection();

            //find the feet keyword after the last end direction letter
            MWSelection feetLocation = findFeet(EndDirectionIndex, aCallString);
                
            //find the next number after the north south keyword
            returnVal = extractPreviousNumberVal(feetLocation.getStartIndex(), aCallString);

            return returnVal;
        }


        public MWNumberSelection extractPreviousNumberVal(int startSearchIndex, string aCallString)
        {
            MWNumberSelection returnVal = new MWNumberSelection();
            //find the first digit
            int n = 0;
            bool numberEnded = false;

            int i = startSearchIndex - 1;
            int foundStartIndex = -1;
            int foundEndingIndex = -1;
            do
            {
                //find the first number
                if (int.TryParse(Convert.ToString(aCallString[i]), out n) == true)
                {
                    //we have the first number, we need to add it to the string
                    foundEndingIndex = i;
                    do
                    {
                        if (int.TryParse(Convert.ToString(aCallString[i - 1]), out n) == true || aCallString[i - 1] == '.' || aCallString[i - 1] == ',')
                        {
                            --i;
                        }
                        else
                        {
                            numberEnded = true;
                            foundStartIndex = i;
                        }

                    } while (numberEnded == false && i > 0);

                }

                --i;

            } while (numberEnded == false && i > 0);


            string foundValue = aCallString.Substring(foundStartIndex, (foundEndingIndex + 1) - foundStartIndex);
            returnVal.setNumberValue(Convert.ToDouble(foundValue));
            returnVal.setNumberValue(Convert.ToDouble(foundValue)); returnVal.setStartIndex(foundStartIndex);
            returnVal.setEndIndex(foundEndingIndex);

            return returnVal;
        }


        public MWNumberSelection extractNextNumberVal(int prevIndex, string aCallString)
        {
            MWNumberSelection returnVal = new MWNumberSelection();
            //find the first digit
            int n = 0;
            bool numberEnded = false;
           
            int i = prevIndex + 1;
            int foundStartIndex = -1;
            int foundEndingIndex = -1;
            do
            {
                //find the first number
                if (int.TryParse(Convert.ToString(aCallString[i]), out n) == true)
                {
                    //we have the first number, we need to add it to the string
                    foundStartIndex = i;
                    do
                    {
                        if (int.TryParse(Convert.ToString(aCallString[i+1]), out n) == true || aCallString[i + 1] == '.')
                        {
                            ++i;
                        }else
                        {
                            numberEnded = true;
                            foundEndingIndex = i;
                        }
                        
                    } while (numberEnded == false && i < aCallString.Length);

                }

                ++i;

            } while (numberEnded == false && i < aCallString.Length);


            string foundValue = aCallString.Substring(foundStartIndex, (foundEndingIndex +1) - foundStartIndex);
            returnVal.setNumberValue(Convert.ToDouble(foundValue));
            returnVal.setNumberValue(Convert.ToDouble(foundValue));returnVal.setStartIndex(foundStartIndex);
            returnVal.setEndIndex(foundEndingIndex);

            return returnVal;
        }


        public bool findCurveKeyword(int startingIndex, string aCallString = "")
        {
            bool returnBool = false;
            string[] stringArray = new string[]{"Curve", "CURVE", "curve"};
            MWSelection returnSelection = new MWSelection();

            returnSelection = findNextGeneral(stringArray, startingIndex, aCallString);

            if (returnSelection.getStartIndex() != -1)
            {
                returnBool = true;
            }
            return returnBool;
        }

        public CurveLeftRightCategory findRightLeftKeyword(string aCallString)
        {
            CurveLeftRightCategory returnCat = CurveLeftRightCategory.NotFound;

            MWSelection selectionObject = findRight(0, aCallString);
            if (selectionObject.getStartIndex() >-1)
            {
                returnCat = CurveLeftRightCategory.Right;
            }
            
            if (returnCat != CurveLeftRightCategory.Right)
            {
                selectionObject = findLeft(0, aCallString);
                if (selectionObject.getStartIndex() > -1)
                {
                    returnCat = CurveLeftRightCategory.Left;
                }
            }

            return returnCat;
        }

        public MWSelection findNextGeneral(string[] stringArray, int startingIndex, string aString = "")
        {

            string stringToSearch = theString; //thestring is a property of the class

            if (aString.Length != 0)
            {
                stringToSearch = aString;
            }

            MWSelection returnSelectionObject = new MWSelection();

            MWSelection[] selectionObjectArray = new MWSelection[stringArray.Length];

            for (int i = 0; i < stringArray.Length; ++i)
            {
                MWSelection tempSelection = new MWSelection();
                tempSelection = findNextString(stringToSearch, startingIndex, stringArray[i]);
                selectionObjectArray.SetValue(tempSelection, i);
                
            }

            int firstStart = 0;
            returnSelectionObject = selectionObjectArray[0];

            for (int j = 0; j < stringArray.Length; ++j)
            {
                if (selectionObjectArray[j].getStartIndex() >= 0
                    && (selectionObjectArray[j].getStartIndex() < firstStart || firstStart == 0))
                {
                    firstStart = selectionObjectArray[j].getStartIndex();
                    returnSelectionObject = selectionObjectArray[j];
                }
            }


            return returnSelectionObject;

        }

        public MWSelection findNextString(string theString, int startingIndex, string searchString)
        {

            MWSelection selectionObject = new MWSelection();
            int i = startingIndex;
            if (startingIndex == -1)
            {
                i = 0;
            }
            while (i < (theString.Length - searchString.Length +1) && selectionObject.getEndIndex() < 0)
            {
                if (theString[i] == searchString[0]) //possbile hit
                {
                    //test if the word matches
                    string testword = theString.Substring(i, searchString.Length);
                    if (testword == searchString)
                    {
                        //we have a matching word
                        selectionObject.setStartIndex(i);
                        selectionObject.setEndIndex(i + searchString.Length - 1);
                    }
               
                }
                ++i;
            }//end while

            return selectionObject;


        }

    }

}
