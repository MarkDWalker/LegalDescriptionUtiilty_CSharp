
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace CSharp_Test
{
    public partial class Form1 : Form
    {

        List<MWCadNameSpace.DeconstructedCallString> deconstructedCallList = null;
        MWCadNameSpace.MWCad cadStuff = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void callDataGrid_SelectRow(object sender, EventArgs e)
        {
            

            int index = callDataGrid.CurrentRow.Index;

            //MessageBox.Show("The current Row is -  " + index);
            int bIndex = Convert.ToInt32(callDataGrid.Rows[index].Cells[0].Value);
            int eIndex = Convert.ToInt32(callDataGrid.Rows[index].Cells[1].Value);

            rich1.SelectAll();
            rich1.SelectionBackColor = Color.White;
            rich1.DeselectAll();
            rich1.Select(bIndex, (eIndex - bIndex) + 1);
            rich1.SelectionBackColor = Color.GreenYellow;


            Point topPoint = new Point(1, 1);
            int topIndex = rich1.GetCharIndexFromPosition(topPoint);

            Point bottomPoint = new Point(1, rich1.Height - 1);
            int bottomIndex = rich1.GetCharIndexFromPosition(bottomPoint);

            if (bIndex < topIndex || bIndex > bottomIndex)
            {
                rich1.ScrollToCaret();
            }

           


        }

        private void setTab2Controls()
        {
            rich1.Height = tabPage2.Height-20;
            rich1.Width = 300;
            rich1.Left = 0;
            rich1.Top = 50;

            callDataGrid.Left = rich1.Width;
            callDataGrid.Width = (tabPage2.Width - rich1.Width);
            callDataGrid.Top = 50;
            callDataGrid.Height = tabPage2.Height-20;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            deconstructedCallList = new List<MWCadNameSpace.DeconstructedCallString>();

            this.Width = 450;
            this.Height = Screen.PrimaryScreen.Bounds.Height;
            this.Left =0;
            this.Top = 0;

            

            callDataGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }


        private void TabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

          
            setTab2Controls();
            
        }





        private void populateCallDataGrid(List<MWCadNameSpace.DeconstructedCallString> deconstructedCallList)
        {
            callDataGrid.Rows.Clear();
            callDataGrid.ColumnCount = 6;

            callDataGrid.Columns[0].Name = "Start Index";
            callDataGrid.Columns[1].Name = "End Index";
            callDataGrid.Columns[2].Name = "Bearing";
            callDataGrid.Columns[3].Name = "Dist";
            callDataGrid.Columns[4].Name = "Curve Dir";
            callDataGrid.Columns[5].Name = "Rad";

            callDataGrid.Columns[0].Width = 50;
            callDataGrid.Columns[1].Width = 50;
           

            for (int i = 0; i< deconstructedCallList.Count; ++i)
            {
                MWCadNameSpace.DeconstructedCallString dCallString = deconstructedCallList[i];
             
                if (dCallString.getCurveLeftRight().Value == "NA")
                {
                    string[] rowData = new string[] {Convert.ToString(dCallString.getStart()),
                                                        Convert.ToString(dCallString.getEnd()),
                                                        dCallString.getBearingAsString(),
                                                        Convert.ToString(dCallString.getLength()),
                                                        "-","-"};
                    callDataGrid.Rows.Add(rowData);
                }
                else //we  have a curve
                {
                    string[] rowData = new string[] {Convert.ToString(dCallString.getStart()),
                                                        Convert.ToString(dCallString.getEnd()),
                                                        dCallString.getBearingAsString(),
                                                        Convert.ToString(dCallString.getLength()),
                                                        dCallString.getCurveLeftRight().Value,
                                                        Convert.ToString(dCallString.getRadius()),
                                                        };
                    callDataGrid.Rows.Add(rowData);
                }
                //string[] rowData = 
            }

        }

        private void populateGrid3(MWCadNameSpace.SurveyBoundary boundary)
        {

            Grid3.ColumnCount = 2;
            Grid3.Columns[0].Name = "Property";
            Grid3.Columns[1].Name = "Values";

            for (int i = 0; i < boundary.segmentCount(); ++i)
            {
                string[] rowData = new string[] { "SEGMENT DATA  --", Convert.ToString(i) + "--" };
                Grid3.Rows.Add(rowData);

                string[] rowDatax = new string[] { "gStart.x", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGStart().getPnt3d().getX()) };
                Grid3.Rows.Add(rowDatax);
                string[] rowDatay = new string[] { "gStart.y", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGStart().getPnt3d().getY()) };
                Grid3.Rows.Add(rowDatay);
                string[] rowDataz = new string[] { "gStart.bulge", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGStart().getBuldge()) };
                Grid3.Rows.Add(rowDataz);


                string[] rowDataa = new string[] { "gEnd.x", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGEnd().getPnt3d().getX()) };
                Grid3.Rows.Add(rowDataa);
                string[] rowDatab = new string[] { "gEnd.y", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGEnd().getPnt3d().getY()) };
                Grid3.Rows.Add(rowDatab);
                string[] rowDatac = new string[] { "gEnd.bulge", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).getGEnd().getBuldge()) };
                Grid3.Rows.Add(rowDatac);

                string[] rowData0 = new string[] { "dx", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).dx()) };
                Grid3.Rows.Add(rowData0);
                string[] rowData1 = new string[] { "dy", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).dy()) };
                Grid3.Rows.Add(rowData1);
                string[] rowData2 = new string[] { "lEnd.x", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).lEnd().getPnt3d().getX()) };
                Grid3.Rows.Add(rowData2);
                string[] rowData3 = new string[] { "lEnd.y", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).lEnd().getPnt3d().getY()) };
                Grid3.Rows.Add(rowData3);

                string[] rowData4 = new string[] { "gCadAngle", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).gCadAngle()) };
                Grid3.Rows.Add(rowData4);
                string[] rowData5 = new string[] { "length", Convert.ToString(boundary.getTranslatedCorrectedSegment(i).length()) };
                Grid3.Rows.Add(rowData5);

            }
        }

        private void populateGrid2(MWCadNameSpace.SurveyBoundary boundary)
        {
            Grid2.ColumnCount = 2;
            Grid2.Columns[0].Name = "Property";
            Grid2.Columns[1].Name = "Values";

            for (int i = 0; i < boundary.segmentCount(); ++i)
            {
                string[] rowData = new string[] { "SEGMENT DATA  --", Convert.ToString(i) + "--" };
                Grid2.Rows.Add(rowData);

                string[] rowDatax = new string[] { "gStart.x", Convert.ToString(boundary.getCorrectedSegment(i).getGStart().getPnt3d().getX()) };
                Grid2.Rows.Add(rowDatax);
                string[] rowDatay = new string[] { "gStart.y", Convert.ToString(boundary.getCorrectedSegment(i).getGStart().getPnt3d().getY()) };
                Grid2.Rows.Add(rowDatay);
                string[] rowDataz = new string[] { "gStart.bulge", Convert.ToString(boundary.getCorrectedSegment(i).getGStart().getBuldge()) };
                Grid2.Rows.Add(rowDataz);


                string[] rowDataa = new string[] { "gEnd.x", Convert.ToString(boundary.getCorrectedSegment(i).getGEnd().getPnt3d().getX()) };
                Grid2.Rows.Add(rowDataa);
                string[] rowDatab = new string[] { "gEnd.y", Convert.ToString(boundary.getCorrectedSegment(i).getGEnd().getPnt3d().getY()) };
                Grid2.Rows.Add(rowDatab);
                string[] rowDatac = new string[] { "gEnd.bulge", Convert.ToString(boundary.getCorrectedSegment(i).getGEnd().getBuldge()) };
                Grid2.Rows.Add(rowDatac);

                string[] rowData0 = new string[] { "dx", Convert.ToString(boundary.getCorrectedSegment(i).dx()) };
                Grid2.Rows.Add(rowData0);
                string[] rowData1 = new string[] { "dy", Convert.ToString(boundary.getCorrectedSegment(i).dy()) };
                Grid2.Rows.Add(rowData1);
                string[] rowData2 = new string[] { "lEnd.x", Convert.ToString(boundary.getCorrectedSegment(i).lEnd().getPnt3d().getX()) };
                Grid2.Rows.Add(rowData2);
                string[] rowData3 = new string[] { "lEnd.y", Convert.ToString(boundary.getCorrectedSegment(i).lEnd().getPnt3d().getY()) };
                Grid2.Rows.Add(rowData3);

                string[] rowData4 = new string[] { "gCadAngle", Convert.ToString(boundary.getCorrectedSegment(i).gCadAngle()) };
                Grid2.Rows.Add(rowData4);
                string[] rowData5 = new string[] { "length", Convert.ToString(boundary.getCorrectedSegment(i).length()) };
                Grid2.Rows.Add(rowData5);

            }
        }

        private void populateGrid1(MWCadNameSpace.SurveyBoundary boundary)
        {

            Grid1.ColumnCount = 2;
            Grid1.Columns[0].Name = "Property";
            Grid1.Columns[1].Name = "Values";

            for (int i = 0; i < boundary.segmentCount(); ++i)
            {
                string[] rowData = new string[] { "SEGMENT DATA  --", Convert.ToString(i) + "--" };
                Grid1.Rows.Add(rowData);

                string[] rowData0 = new string[] { "gStart.x", Convert.ToString(boundary.getOriginalSegment(i).getGStart().getPnt3d().getX()) };
                Grid1.Rows.Add(rowData0);
                string[] rowData1 = new string[] { "gStart.y", Convert.ToString(boundary.getOriginalSegment(i).getGStart().getPnt3d().getY()) };
                Grid1.Rows.Add(rowData1);
                string[] rowData2 = new string[] { "gStart.bulge", Convert.ToString(boundary.getOriginalSegment(i).getGStart().getBuldge()) };
                Grid1.Rows.Add(rowData2);


                string[] rowData3 = new string[] { "gEnd.x", Convert.ToString(boundary.getOriginalSegment(i).getGEnd().getPnt3d().getX()) };
                Grid1.Rows.Add(rowData3);
                string[] rowData4 = new string[] { "gEnd.y", Convert.ToString(boundary.getOriginalSegment(i).getGEnd().getPnt3d().getY()) };
                Grid1.Rows.Add(rowData4);
                string[] rowData5 = new string[] { "gEnd.bulge", Convert.ToString(boundary.getOriginalSegment(i).getGEnd().getBuldge()) };
                Grid1.Rows.Add(rowData5);

                string[] rowData6 = new string[] { "dx", Convert.ToString(boundary.getOriginalSegment(i).dx()) };
                Grid1.Rows.Add(rowData6);
                string[] rowData7 = new string[] { "dy", Convert.ToString(boundary.getOriginalSegment(i).dy()) };
                Grid1.Rows.Add(rowData7);

                string[] rowData8 = new string[] { "lStart.x", Convert.ToString(boundary.getOriginalSegment(i).lStart().getPnt3d().getX()) };
                Grid1.Rows.Add(rowData8);
                string[] rowData9 = new string[] { "lStart.y", Convert.ToString(boundary.getOriginalSegment(i).lStart().getPnt3d().getY()) };
                Grid1.Rows.Add(rowData9);

                string[] rowData10 = new string[] { "lEnd.x", Convert.ToString(boundary.getOriginalSegment(i).lEnd().getPnt3d().getX()) };
                Grid1.Rows.Add(rowData10);
                string[] rowData11 = new string[] { "lEnd.y", Convert.ToString(boundary.getOriginalSegment(i).lEnd().getPnt3d().getY()) };
                Grid1.Rows.Add(rowData11);

                string[] rowData12 = new string[] { "gCadAngle", Convert.ToString(boundary.getOriginalSegment(i).gCadAngle()) };
                Grid1.Rows.Add(rowData12);
                string[] rowData13 = new string[] { "length", Convert.ToString(boundary.getOriginalSegment(i).length()) };
                Grid1.Rows.Add(rowData13);

                string[] rowData14 = new string[] { "cCadAngle", Convert.ToString(boundary.getOriginalSegment(i).cCadAngle()) };
                Grid1.Rows.Add(rowData14);
                string[] rowData15 = new string[] { "length", Convert.ToString(boundary.getOriginalSegment(i).cLength()) };
                Grid1.Rows.Add(rowData15);

                string[] rowData16 = new string[] { "cdx", Convert.ToString(boundary.getOriginalSegment(i).cdx()) };
                Grid1.Rows.Add(rowData16);
                string[] rowData17 = new string[] { "cdy", Convert.ToString(boundary.getOriginalSegment(i).cdy()) };
                Grid1.Rows.Add(rowData17);

                string[] rowData18 = new string[] { "cLEnd.x", Convert.ToString(boundary.getOriginalSegment(i).cLEnd().getPnt3d().getX()) };
                Grid1.Rows.Add(rowData18);
                string[] rowData19 = new string[] { "cLEnd.y", Convert.ToString(boundary.getOriginalSegment(i).cLEnd().getPnt3d().getY()) };
                Grid1.Rows.Add(rowData19);
                string[] rowData20 = new string[] { "cLEnd.bulge", Convert.ToString(boundary.getOriginalSegment(i).cLEnd().getBuldge()) };
                Grid1.Rows.Add(rowData20);

                string[] rowData21 = new string[] { "-----------", "------------" };
                Grid1.Rows.Add(rowData21);
                string[] rowData22 = new string[] { "-----------", "------------" };
                Grid1.Rows.Add(rowData22);
            }


        }
       
        private void  Form1_MouseHover(object sender, EventArgs e)
        {
            //this.Activate();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            setTab2Controls();

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            cadStuff = new MWCadNameSpace.MWCad();
            double dpiSetting = 2;
            if (cadStuff.cadConnection() == true)
            { 
                //int cadLeft = Convert.ToInt32((Convert.ToDouble(this.Left) * dpiSetting)
                //             + (Convert.ToDouble(this.Width) * dpiSetting));
                //int cadTop = 0;
                //int cadHeight = Convert.ToInt32(Convert.ToDouble(this.Height) * dpiSetting);
                //int cadWidth = Convert.ToInt32(Convert.ToDouble(Screen.PrimaryScreen.Bounds.Width) * dpiSetting)
                //              - Convert.ToInt32(Convert.ToDouble(this.Width));

                //cadStuff.setCadScreen(cadLeft, cadTop, cadHeight, cadWidth);
                this.Text = cadStuff.getTitle();
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (cadStuff != null && cadStuff.cadConnection() == true)
            {

                List<MWCadNameSpace.VTex> vertices = cadStuff.getPlineEnityCoords();

                if (vertices.Count > 0)
                {
                    MWCadNameSpace.SurveyBoundary boundary = new MWCadNameSpace.SurveyBoundary(vertices);
                    MWCadNameSpace.pnt3d pickedPnt = cadStuff.getPickPoint(vertices[0].getPnt3d());

                    boundary.translateSegmentList(pickedPnt);


                    //cadStuff.drawPline(boundary.getTranslatedCorrectedSegmentVertices());
                    Autodesk.AutoCAD.Interop.Common.AcadLWPolyline aPline = cadStuff.drawPline(boundary.getTranslatedCorrectedSegmentVertices());
                    cadStuff.setPlineBuldges(aPline, boundary.getTranslatedCorrectedSegmentVertices());

                    Grid1.Rows.Clear();
                    populateGrid1(boundary);

                    Grid2.Rows.Clear();
                    populateGrid2(boundary);

                    Grid3.Rows.Clear();
                    populateGrid3(boundary);
                }
            }
            else
            {
                MessageBox.Show("Connection Error");
            }
        }

        private void rich1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            //change all of the text to a light color
            rich1.SelectAll();
            rich1.SelectionColor = Color.MediumPurple;
            rich1.DeselectAll();


            MWCadNameSpace.LegalDescriptionStringUtility sUtil = new MWCadNameSpace.LegalDescriptionStringUtility(rich1.Text);
            
            //get the call list
            List<MWCadNameSpace.MWSelection> callList = new List<MWCadNameSpace.MWSelection>();
            int i = 0;
            int k = 0;
            do
            {
                MWCadNameSpace.MWSelection call = sUtil.findNextPotentialCall(i);
                if (call.getStartIndex() > 0)
                {
                    
                    i = call.getEndIndex() + 1;
                    callList.Add(call);
                    k = 0;
                }
                else
                {
                    //if call is not found, advance the search start location by 1 and repeat the search
                    ++k;
                    if (callList.Count > 0)
                    {
                        i = callList[callList.Count - 1].getEndIndex() + k;
                    }
                    else
                    {
                        i = k;
                    }
                    
                    
                }
            } while (i < rich1.Text.Length);

            //find any potential curves
            List<MWCadNameSpace.MWSelection> curveLocationList = sUtil.checkForPotentialCurveData();


            //modify the call list items if there are any curve keywords
            List<MWCadNameSpace.MWSelection> modifiedCallList = new List<MWCadNameSpace.MWSelection>();
            if (curveLocationList.Count > 0)
            {
                modifiedCallList = sUtil.mergeCurveDataWithCallList(curveLocationList, callList);
            }else
            {
                modifiedCallList = callList;
            }


            //List<MWCadNameSpace.DeconstructedCallString> deconstructedCallList = new List<MWCadNameSpace.DeconstructedCallString>();

            deconstructedCallList.Clear();

            //now we need to get the information our of the courses.
            for (int j = 0; j < callList.Count; ++j)
            {
                string stringCallValue = rich1.Text.Substring(modifiedCallList[j].getStartIndex(), modifiedCallList[j].length());
                 MWCadNameSpace.DeconstructedCallString aCallString = new MWCadNameSpace.DeconstructedCallString(stringCallValue, callList[j].getStartIndex(), callList[j].getEndIndex());
                deconstructedCallList.Add(aCallString);
                Console.WriteLine("Stopper");
            }

            populateCallDataGrid(deconstructedCallList);

            
            for (int f = 0; f < callList.Count; ++f)
            {
                rich1.Select(callList[f].getStartIndex(), callList[f].length());
                rich1.SelectionColor = Color.Black;
            }

            
            

        }

        private void button5_Click(object sender, EventArgs e)
        {
            MWCadNameSpace.pnt3d origin = new MWCadNameSpace.pnt3d();
            MWCadNameSpace.pnt3d pickPnt = null;
            try
            {
                pickPnt = cadStuff.getPickPoint(origin);
            }
            catch
            {
                MessageBox.Show("Cad Connection Error");
                return;
            }
            List<MWCadNameSpace.VTex> vertices = new List<MWCadNameSpace.VTex>();

            MWCadNameSpace.pnt3d lastEntPnt = null;

            for (int i = 0; i < deconstructedCallList.Count; ++i)
            {
                MWCadNameSpace.pnt3d startPnt = pickPnt;
                if (i > 0)
                {
                    startPnt = lastEntPnt;
                }
                MWCadNameSpace.Segment aSegment = deconstructedCallList[i].extractAsSegment(startPnt);
                if (deconstructedCallList[i].getCurveLeftRight().Value == "Right" || deconstructedCallList[i].getCurveLeftRight().Value == "Left")
                {
                    aSegment.setBuldgeFromRadius(deconstructedCallList[i].getRadius(), deconstructedCallList[i].getCurveLeftRight());
                }
                vertices.Add(aSegment.getGStart());

                lastEntPnt = aSegment.getGEnd().getPnt3d();
                if (i == deconstructedCallList.Count - 1){
                    vertices.Add(aSegment.getGEnd());
                }
            }


            Autodesk.AutoCAD.Interop.Common.AcadLWPolyline thePline = cadStuff.drawPline(vertices);
            cadStuff.setPlineBuldges(thePline, vertices);

        }

        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                double dpiSetting = 2;
                if (cadStuff.cadConnection() == true)
                {
                    int cadLeft = Convert.ToInt32((Convert.ToDouble(this.Left) * dpiSetting)
                                 + (Convert.ToDouble(this.Width) * dpiSetting));
                    int cadTop = 0;
                    int cadHeight = Convert.ToInt32(Convert.ToDouble(this.Height) * dpiSetting);
                    int cadWidth = Convert.ToInt32(Convert.ToDouble(Screen.PrimaryScreen.Bounds.Width) * dpiSetting)
                                  - Convert.ToInt32(Convert.ToDouble(this.Width) * dpiSetting);

                    cadStuff.setCadScreen(cadLeft, cadTop, cadHeight, cadWidth);
                }
            }
            catch
            {
                MessageBox.Show("Connection error");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
