using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Transaction(TransactionMode.Manual)]
    class TANNY05_EqualXYZ : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            List<XYZ> pointList = new List<XYZ>();
            pointList.Add(new XYZ(1, 0, 0));
            pointList.Add(new XYZ(2, 0, 0));
            pointList.Add(new XYZ(3, 0, 0));
            ////--建立一個字串的陣列用來搜尋
            //List<string> list_string = new List<string>();
            //foreach (XYZ point in pointList)
            //{
            //    list_string.Add(point.ToString());
            //}
            //點編號
            XYZ p1 = new XYZ(2, 0, 0);

            int index = -1;
            //index = list_string.IndexOf(p1.ToString());
            index = pointList.FindIndex(p => p.IsAlmostEqualTo(p1));
            
            MessageBox.Show($"index={index.ToString()}", "TEST");

            return Result.Succeeded;

        }
    }
}
