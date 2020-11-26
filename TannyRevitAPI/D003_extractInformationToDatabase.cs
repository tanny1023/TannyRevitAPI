using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class D003_SaveDatabase : IExternalCommand
    {
        /// <summary>
        /// This example write the base constraint, the volumn, the area, and the length of walls to a Access database, 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;
            string modelPath = doc.PathName;
            string modelTitle = doc.Title;

            string dbPath = modelPath.Replace(doc.Title + ".rvt", "info.accdb");
            string cnStr = "Provider=Microsoft.ACE.OLEDB.12.0; Data Source = " + dbPath;
            OleDbConnection cn = new OleDbConnection(cnStr);
            cn.Open();

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                {
                    string condition = findCondition(wall, doc);
                    double volume = findVolume(wall);
                    double area = findArea(wall);
                    double length = findLength(wall);
                    string sqlCMD1 = "INSERT INTO Info (BaseConstraint, Volume, Area, Length) VALUES ('"+condition+"', '"+volume+"','"+area+"','"+length+"');";
                    OleDbCommand cmd = new OleDbCommand(sqlCMD1, cn);
                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Successfully write the information of walls to a database!");

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM Info ");
            string sqlCmd = sb.ToString();

            DataSet ds = new DataSet();
            OleDbDataAdapter dataAdaptor = new OleDbDataAdapter(sqlCmd, cn);
            dataAdaptor.Fill(ds, "joined");
            DataTable dt = ds.Tables["joined"];

            StringBuilder sbR = new StringBuilder();

            for (int k = 0; k != dt.Columns.Count; k++)
                sbR.Append(dt.Columns[k].ColumnName + "\t");
            sbR.AppendLine();

            for (int i = 0; i != dt.Rows.Count; i++)
            {
                for (int j = 0; j != dt.Columns.Count; j++)
                {
                    sbR.Append(dt.Rows[i][j] + "\t");
                }
                sbR.AppendLine();
            }//end for i

            MessageBox.Show(sbR.ToString());
            cn.Close();
            return Result.Succeeded;
        }//end fun

        private double findVolume(Element elem)
        {
            double volume = 0.0;
            //volume = elem.LookupParameter("體積").AsDouble();
            volume = elem.LookupParameter("Volume").AsDouble();
            return volume;
        }

        private string findCondition(Element elem, Document doc)
        {
            string name = null;
            //name = doc.GetElement(elem.LookupParameter("底部約束").AsElementId()).Name;
            name = doc.GetElement(elem.LookupParameter("Base Constraint").AsElementId()).Name;
            return name;
        }

        private double findArea(Element elem)
        {
            double area = 0.0;
            //area = elem.LookupParameter("面積").AsDouble();
            area = elem.LookupParameter("Area").AsDouble();
            return area;
        }

        private double findLength(Element elem)
        {
            double length = 0.0;
            //length = elem.LookupParameter("長度").AsDouble();
            length = elem.LookupParameter("Length").AsDouble();
            return length;
        }
    };
}
