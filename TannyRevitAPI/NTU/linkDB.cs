using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Text;
using System.Data;

namespace revitAPI2019
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class showData : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string dbPath = "D:\\toy.accdb";
            string cnStr = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dbPath;
            // string cnStr = "Provider=Microsoft.Jet.OLEDB.4.0; Data Source=" +dbPath;
            OleDbConnection cn = new OleDbConnection(cnStr);
            cn.Open();
            //string sqlCmd = "SELECT * FROM activity";
            //OleDbCommand cmd = new OleDbCommand(sqlCmd, cn);
            //OleDbDataReader dr = cmd.ExecuteReader();

            string sqlCMD1 = "INSERT INTO main (ID, Name) VALUES (100, 'John');";
            OleDbCommand cmd = new OleDbCommand(sqlCMD1, cn);
            cmd.ExecuteNonQuery();

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM main ");


            string sqlCmd = sb.ToString();

            DataSet ds = new DataSet();
            OleDbDataAdapter dataAdaptor = new OleDbDataAdapter(sqlCmd, cn);
            dataAdaptor.Fill(ds, "joined");
            DataTable dt = ds.Tables["joined"];

            StringBuilder sbR = new StringBuilder();

            for (int k = 0; k != dt.Columns.Count; k++)
                sbR.Append(dt.Columns[k].ColumnName + "\t");
            sbR.Append("\n");

            for (int i = 0; i != dt.Rows.Count; i++)
            {
                for (int j = 0; j != dt.Columns.Count; j++)
                {
                    sbR.Append(dt.Rows[i][j] + "\t");
                }
                sbR.Append("\n");
            }//end for i

            MessageBox.Show(sbR.ToString());
            dt.WriteXml("D:\\tableExample.xml", XmlWriteMode.WriteSchema);
            return Result.Succeeded;
        }//end fun


    };
}
