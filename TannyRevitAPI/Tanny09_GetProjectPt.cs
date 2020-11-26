using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Transaction(TransactionMode.Manual)]
    class Tanny09_GetProjectPt : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            XYZ A = new XYZ(2, 1, 0);
            XYZ B = new XYZ(7, 0, 0);
            XYZ C = new XYZ(8, 5, 0);
            List<XYZ> wallLine = new List<XYZ> { A, C };
            XYZ D = GetProjectPt(B, wallLine);
            MessageBox.Show($"投影點座標D={D.ToString()}");
            return Result.Succeeded;

        }
        //--判斷點與線的關係，點在線段首尾兩端之外表示r <= 0 || r >= 1
        public double GetPtLineRelation(XYZ startPt, XYZ endPt, XYZ checkPt)
        {
            double cross = (endPt.X - startPt.X) * (checkPt.X - startPt.X) + (endPt.Y - startPt.Y) * (checkPt.Y - startPt.Y);
            double length = ((endPt.X - startPt.X) * (endPt.X - startPt.X)) + ((endPt.Y - startPt.Y) * (endPt.Y - startPt.Y));
            double r = cross / length;
            return r;
        }
        //--回傳XY投影點座標
        public XYZ GetProjectPt(XYZ checkPt, List<XYZ> wallLine)
        {
            XYZ startPt = wallLine[0];
            XYZ endPt = wallLine[1];
            double r = GetPtLineRelation(startPt, endPt, checkPt);
            double px = startPt.X + (endPt.X - startPt.X) * r;
            double py = startPt.Y + (endPt.Y - startPt.Y) * r;
            XYZ projectPt = new XYZ(px, py, checkPt.Z);
            return projectPt;
        }

    }
}
