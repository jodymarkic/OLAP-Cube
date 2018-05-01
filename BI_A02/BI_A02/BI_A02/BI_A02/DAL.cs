/*
*   FILENAME: DAL
*   PROJECT: BI_A03
*   PROGRAMMER: Jody Markic
*   FIRST VERSION:10/28/2017
*   DESCRIPTION: This files holds all the data access to the database yoyoProduction.
*                Here is where all stored procedures that Insert and retrieve values from the database
*                are held.
*
*/
using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using Microsoft.AnalysisServices.AdomdClient;


namespace BI_A02
{
    //
    // CLASS: DAL
    // DESCRIPTION: This class hold all stored procedures responsible for inserting and retrieving data from yoyoProduction
    //
    class DAL
    {
        //local variable

        private AdomdConnection myAdomdConnection = new AdomdConnection(ConfigurationManager.ConnectionStrings["DALstring"].ConnectionString);
        //private SqlConnection mySQLConnection = new System.Data.SqlClient.SqlConnection(ConfigurationManager.ConnectionStrings["DALstring"].ConnectionString);

        //
        //  METHOD      : OpenConnection
        //  DESCRIPTION : open connection to database
        //  PARAMETERS  : na
        //  RETURNS     : void
        //
        public void OpenConnection()
        {
            myAdomdConnection.Open();
        }

        //
        //  METHOD      : CloseConnection
        //  DESCRIPTION : close connection to database
        //  PARAMETERS  : na
        //  RETURNS     : void
        //
        public void CloseConnection()
        {
            myAdomdConnection.Close();
        }

        //
        //  METHOD      : setupTable
        //  DESCRIPTION : Setup a datatable that recieves the results of the cube query
        //  PARAMETERS  : DataTable table, string columnName
        //  RETURNS     : DataTable : table
        //
        public DataTable setupTable(DataTable table, string columnName)
        {
            DataColumn myDataColumn = new DataColumn(columnName, Type.GetType("System.String"));
            table.Columns.Add(myDataColumn);
            myDataColumn = new DataColumn("Sum", Type.GetType("System.Int32"));
            table.Columns.Add(myDataColumn);

            return table;
        }

        //
        //  METHOD      : evaluateReason
        //  DESCRIPTION : evaluates a string for being empty, null or whitespace
        //  PARAMETERS  : string reason
        //  RETURNS     : string : reason
        //
        public string evaluateReason(string reason)
        {
            if (String.IsNullOrEmpty(reason) || String.IsNullOrWhiteSpace(reason))
            {
                reason = null;
            }
            return reason;
        }


        //
        //  METHOD      : GetData
        //  DESCRIPTION : provide a procedure and get a return value
        //  PARAMETERS  : procedureName
        //  RETURNS     : void
        //
        public DataTable GetData(string productName, string lineName, string columnName)
        {
            DataTable bufferTable = new DataTable();
            string bufferReason;

            //setup the table
            bufferTable = setupTable(bufferTable, columnName);
            
            ///MDX Query -- generic enough for both retrieving scrap and report data from the cube
            AdomdCommand myAdomdCommand = new AdomdCommand("SELECT NON EMPTY { [Measures].[Yo Yo Data Count] }" +
                " ON COLUMNS, NON EMPTY { ( ["+ columnName + "].[" + columnName +"].[" + columnName+ "] ) }" +
                " ON ROWS FROM [Yo Yo DB] WHERE {[Line].[Line].[" + lineName + "]} * {[Product Description].[Description].[" + productName + "]}",
                myAdomdConnection);

            //open the connection
            myAdomdConnection.Open();

            //execute the query
            AdomdDataReader myAdomdDataReader = myAdomdCommand.ExecuteReader();

            //read query results with a DataReader, reading one row at a time and stacking up the results into a dataTable.
            while (myAdomdDataReader.Read())
            {
                //evaluate the cell for data
                bufferReason = evaluateReason(myAdomdDataReader[0].ToString());

                //if cell is empty skip
                if (!String.IsNullOrEmpty(bufferReason))
                {
                    //otherwise add the row to the table
                    bufferTable.Rows.Add(bufferReason, Int32.Parse(myAdomdDataReader[1].ToString()));
                }
            }

            //close the connection
            myAdomdConnection.Close();

            //return the built table
            return bufferTable;
        }
    }
}
