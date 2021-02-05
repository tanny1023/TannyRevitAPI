using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Data;
using System.Windows;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Tanny12_WriteDataTable : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            List<double> xRays = new List<double> { 1, 2, 3, 4, 5 };
            List<double> yRays = new List<double> { 11, 12, 13, 14, 15 };

            //--儲存檔案
            GetMeshtable(xRays, yRays).WriteXml(@"D:\Hsintien_tsai\Result.xml", XmlWriteMode.WriteSchema); ;
            MessageBox.Show("OK0250");
            return Result.Succeeded;
        }

        //--建立MeshTable
        public DataTable GetMeshtable(List<double> xRays, List<double> yRays)
        {
            DataTable meshtable = new DataTable("MeshPoints");
            DataRow row;

            // 建立欄位
            meshtable.Columns.Add("Ycoordinate", typeof(double));
            for (int j = 0; j < yRays.Count; j++)
            {
                meshtable.Columns.Add($"x={xRays[j]}", typeof(string));
            }

            // 新增資料到DataTable
            for (int i = 0; i < yRays.Count; i++)
            {
                row = meshtable.NewRow();
                row["Ycoordinate"] = yRays[i];
                for (int j = 0; j < yRays.Count; j++)
                {
                    XYZ PT = new XYZ(xRays[j], yRays[i], 0);
                    if (PT.X.Equals(2) && PT.Y.Equals(12))
                        row[$"x={xRays[j]}"] = null;
                    else
                        row[$"x={xRays[j]}"] = PT.ToString();
                }
                meshtable.Rows.Add(row);
            }
            return meshtable;
        }
    }
}
