/*
*   FILENAME: myForm
*   PROJECT: BI_A03
*   PROGRAMMER: Jody Markic
*   FIRST VERSION: 10/28/2017
*   DESCRIPTION: This files holds the event handles and objects to the BI_A03 project.
*                This file uses a DAL object to securely access a database. This file uses a button
*                to update the a pareto diagram that reflects scrap rate along with values displayed as
*                a report.
*
*/
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Messaging;
using System.Windows.Forms.DataVisualization.Charting;
using System.Data;

namespace BI_A02
{
    //
    // Class: myForm : Form
    // Description: This method acts as event handle and api to the BI_A02
    //
    //
    public partial class myForm : Form
    {
        //local variables
        private string currentLineSelection = "";
        private string currentYoyoSelection = "";

        DataTable scrapTable = new DataTable();
        DataTable recordTable = new DataTable();

        private DAL myDAL = new DAL();
        private string[] defects = { "FINAL_COAT_FLAW", "BROKEN_SHELL", "TANGLED_STRING", "PRIMER_DEFECT", "DRIP_MARK", "INCONSISTENT_THICKNESS", "WARPING", "PITTING" };

        //enum
        enum productName { AllProjects = 0, OriginalSleeper, BlackBeauty,
            Firecracker, LemonYellow, MidnightBlue, ScreamingOrange,
            GoldGlitter, WhiteLightening }

        enum lineNumbers { AllLines = 0, Line0, Line1, Line2, Line3, Line4 }


        //
        //  METHOD      : myForm
        //  DESCRIPTION : Constructor
        //  PARAMETERS  : N/A
        //  RETURNS     : N/A
        //
        public myForm()
        {
            InitializeComponent();
            seedYoyoCombobox();
            seedLineCombobox();

            paretoDiagram.DataSource = scrapTable;

            LoadParetoDiagram();
        }

        //
        //  METHOD      : seedYoyoCombobox
        //  DESCRIPTION : seed the combobox
        //  PARAMETERS  : N/A
        //  RETURNS     : void
        //
        public void seedYoyoCombobox()
        {
            yoyoComboBox.Items.Add("All Projects");
            yoyoComboBox.Items.Add("Original Sleeper");
            yoyoComboBox.Items.Add("Black Beauty");
            yoyoComboBox.Items.Add("Firecracker");
            yoyoComboBox.Items.Add("Lemon Yellow");
            yoyoComboBox.Items.Add("Midnight Blue");
            yoyoComboBox.Items.Add("Screaming Orange");
            yoyoComboBox.Items.Add("Gold Glitter");
            yoyoComboBox.Items.Add("White Lightening");
        }

        //
        //  METHOD      : seed:LineCombobox
        //  DESCRIPTION : seed the combobox
        //  PARAMETERS  : N/A
        //  RETURNS     : void
        //
        public void seedLineCombobox()
        {
            lineComboBox.Items.Add("All Lines");
            lineComboBox.Items.Add("Line0");
            lineComboBox.Items.Add("Line1");
            lineComboBox.Items.Add("Line2");
            lineComboBox.Items.Add("Line3");
            lineComboBox.Items.Add("Line4");
        }

        //
        //  METHOD      : yoyoComboBox_SelectedIndexChanged
        //  DESCRIPTION : Event handle for selected index changing in combobox
        //                assigns a string value depending on user selection for later use
        //  PARAMETERS  : object sender, EventArgs e
        //  RETURNS     : void
        //
        private void yoyoComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (yoyoComboBox.SelectedIndex)
            {
                case (int)productName.AllProjects:
                    currentYoyoSelection = "Description";
                    break;
                case (int)productName.OriginalSleeper:
                    currentYoyoSelection = "Original Sleeper";
                    break;
                case (int)productName.BlackBeauty:
                    currentYoyoSelection = "Black Beauty";
                    break;
                case (int)productName.Firecracker:
                    currentYoyoSelection = "Firecracker";
                    break;
                case (int)productName.LemonYellow:
                    currentYoyoSelection = "Lemon Ring";
                    break;
                case (int)productName.MidnightBlue:
                    currentYoyoSelection = "Midnight Blue";
                    break;
                case (int)productName.ScreamingOrange:
                    currentYoyoSelection = "Screaming Orange";
                    break;
                case (int)productName.GoldGlitter:
                    currentYoyoSelection = "Gold Glitter";
                    break;
                case (int)productName.WhiteLightening:
                    currentYoyoSelection = "White Lighteneni";
                    break;
                default:
                    break;
            }
        }

        //
        //  METHOD      : lineComboBox_SelectedIndexChanged
        //  DESCRIPTION : Event handle for selected index changing in combobox
        //                ssigns a string value depending on user selection for later use
        //  PARAMETERS  : object sender, EventArgs e
        //  RETURNS     : void
        //
        private void lineComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (lineComboBox.SelectedIndex)
            {
                case (int)lineNumbers.AllLines:
                    currentLineSelection = "Line";
                    break;
                case (int)lineNumbers.Line0:
                    currentLineSelection = "Line0";
                    break;
                case (int)lineNumbers.Line1:
                    currentLineSelection = "Line1";
                    break;
                case (int)lineNumbers.Line2:
                    currentLineSelection = "Line2";
                    break;
                case (int)lineNumbers.Line3:
                    currentLineSelection = "Line3";
                    break;
                case (int)lineNumbers.Line4:
                    currentLineSelection = "Line4";
                    break;
                default:
                    break;
            }
        }
        //
        //  METHOD      : ClearCurrentChart
        //  DESCRIPTION : Clears existing data from chart
        //  PARAMETERS  : N/A
        //  RETURNS     : void
        //
        private void ClearCurrentChart()
        {
            paretoDiagram.Series.Clear();
            paretoDiagram.Titles.Clear();
            paretoDiagram.ChartAreas.Clear();
        }

        //
        //  METHOD      : LoadParetoDiagram
        //  DESCRIPTION : pre-load some values into the pareto diagram
        //  PARAMETERS  : N/A
        //  RETURNS     : void
        //
        private void LoadParetoDiagram()
        {
            //clear chart data
            ClearCurrentChart();

            //add a chart area
            paretoDiagram.ChartAreas.Add("MyChartArea");

            //add series
            paretoDiagram.Series.Add("Rejects");
            paretoDiagram.Series.Add("Rejection Percentage");

            //give series a chart type
            paretoDiagram.Series["Rejects"].ChartType = SeriesChartType.Column;
            paretoDiagram.Series["Rejection Percentage"].ChartType = SeriesChartType.Line;
            paretoDiagram.Series["Rejection Percentage"].YAxisType = AxisType.Secondary;

            //add title
            paretoDiagram.Titles.Add("Yoyo Pareto Diagram");

            //configure chart axises
            paretoDiagram.ChartAreas["MyChartArea"].AxisY.Title = "Freq.";
            paretoDiagram.ChartAreas["MyChartArea"].AxisY2.Title = "Cum. %";
            paretoDiagram.ChartAreas["MyChartArea"].AxisX.Title = "Defect Catergory";

            paretoDiagram.ChartAreas["MyChartArea"].AxisY2.Enabled = AxisEnabled.True;
            paretoDiagram.ChartAreas["MyChartArea"].AxisY2.Maximum = 100;
            paretoDiagram.ChartAreas["MyChartArea"].AxisY2.Minimum = 0;

        }

        //
        //  METHOD      : checkUserSelections()
        //  DESCRIPTION : Checks if a user has made selections in the combobox's provided
        //  PARAMETERS  : N/A
        //  RETURNS     : bool : result
        //
        private bool checkUserSelections()
        {
            bool result = true;

            if ((yoyoComboBox.SelectedIndex < 0) || (lineComboBox.SelectedIndex < 0))
            {
                result = false;
            }

            return result;
        }


        //
        //  METHOD      : updateRecord()
        //  DESCRIPTION : Updates report displayed in the UI
        //  PARAMETERS  : N/A
        //  RETURNS     : bool : result
        //
        private void updateRecord()
        {
            //local variables
            double total_successfully_molded = 0;
            double yield_at_mold = 0;
            double total_successfully_painted = 0;
            double yield_at_paint = 0;
            double total_successfully_assembled = 0;
            double yield_at_assembly = 0;
            double total_yield = 0;

            //retrieve cube results for report 
            double assembly = Double.Parse(recordTable.Rows[0][1].ToString());
            double inspection_1 = Double.Parse(recordTable.Rows[1][1].ToString());
            double inspection_1_scrap = Double.Parse(recordTable.Rows[2][1].ToString());
            double inspection_2 = Double.Parse(recordTable.Rows[3][1].ToString());
            double inspection_2_rework = Double.Parse(recordTable.Rows[4][1].ToString());
            double inspection_2_scrap = Double.Parse(recordTable.Rows[5][1].ToString());
            double inspection_3 = Double.Parse(recordTable.Rows[6][1].ToString());
            double inspection_3_rework = Double.Parse(recordTable.Rows[7][1].ToString());
            double inspection_3_scrap = Double.Parse(recordTable.Rows[8][1].ToString());
            double mold = Double.Parse(recordTable.Rows[9][1].ToString());
            double package = Double.Parse(recordTable.Rows[10][1].ToString());
            double paint = Double.Parse(recordTable.Rows[11][1].ToString());
            double queue_assembly = Double.Parse(recordTable.Rows[12][1].ToString());


            //calculate values used in the report based on cube results
            //assigned the calculated values to their respective Label in the UI
            TotalMolded.Text = mold.ToString();

            total_successfully_molded = mold - inspection_1_scrap;
            TotalSuccessfulMolds.Text = total_successfully_molded.ToString();

            yield_at_mold = total_successfully_molded / mold;
            yield_at_mold = yield_at_mold * 100;
            YieldAtMold.Text = yield_at_mold.ToString("#.##");

            total_successfully_painted = paint - inspection_2_scrap;
            TotalSuccessfulPaints.Text = total_successfully_painted.ToString();

            yield_at_paint = total_successfully_painted / total_successfully_molded;
            yield_at_paint = yield_at_paint * 100;
            YieldAtPaint.Text = yield_at_paint.ToString("#.##");

            total_successfully_assembled = assembly - inspection_3_scrap;
            TotalSuccessfulAssembly.Text = total_successfully_assembled.ToString();

            yield_at_assembly = total_successfully_assembled / total_successfully_painted;
            yield_at_assembly = yield_at_assembly * 100;
            YieldAtAssembly.Text = yield_at_assembly.ToString("#.##");

            TotalPartsPackaged.Text = package.ToString();

            total_yield = package / mold;
            total_yield = total_yield * 100;
            TotalYield.Text = total_yield.ToString("#.##");

        }

        //
        //  METHOD      : updatePareto()
        //  DESCRIPTION : Updates the pareto based on the results queried from the cube
        //                the majority of this method is credited to Norbert Mika, the code
        //                in this method comes from his ParetoExample project solution.
        //                this method assigns a view to a datatable acting as a datasource for the pareto diagram.
        //                It helps calculate the accumulated scrap total and displaying the columns of the pareto in a Desc order.
        //  PARAMETERS  : N/A
        //  RETURNS     : bool : result
        //
        private void updatePareto()
        {
            int Total = 0;
            foreach (DataRow dr in scrapTable.Rows)
            {
                Total += Int32.Parse(dr[1].ToString());
            }

            // paretoDiagram.ChartAreas[0].AxisY.Maximum = Total;

            paretoDiagram.Series[0].Points.Clear();
            paretoDiagram.Series[1].Points.Clear();

            DataView dv = new DataView(scrapTable);
            dv.Sort = "Sum DESC";

            int cusum = 0;
            foreach (DataRowView dr in dv)
            {
                paretoDiagram.Series[0].Points.AddXY(dr[0], dr[1]);
                cusum += Int32.Parse(dr[1].ToString());
                paretoDiagram.Series[1].Points.AddY((cusum * 100) / Total);
            }
        }

        //
        //  METHOD      : updateButton_Click
        //  DESCRIPTION : Event handles for the update button
        //  PARAMETERS  : object sender, EventArgs e
        //  RETURNS     : void
        //
        private void updateButton_Click(object sender, EventArgs e)
        {
            if (checkUserSelections())
            {
                recordTable = myDAL.GetData(currentYoyoSelection, currentLineSelection, "State");
                updateRecord();

                scrapTable = myDAL.GetData(currentYoyoSelection, currentLineSelection, "Reason");
                updatePareto();
            }
            else
            {
                MessageBox.Show("Please select a \"Line\" and \"Product\" option from the dropdown menu's.");
            }
        }

        private void myForm_Load(object sender, EventArgs e)
        {

        }
    }
}
