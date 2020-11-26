using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text;
using System.Collections.Generic;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs16_GetPosition : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            StringBuilder st = new StringBuilder();
            st.AppendLine("位於以下座標的牆已被選取:");
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                LocationCurve locationCurve = elem.Location as LocationCurve;
                Line locationLine = locationCurve.Curve as Line;
                string position = getPosition(locationLine);
                st.AppendLine(position);
            }

            MessageBox.Show(st.ToString());
            return Result.Succeeded;
        }

        private string getPosition(Line line)
        {
            XYZ startPoint = line.GetEndPoint(0);
            // 在2014以前取得點位座標的方法為get_EndPoint()，而在2014方法更新為GetEndPoint()
            string x = startPoint.X.ToString();
            string y = startPoint.Y.ToString();
            string position = "( " + x + ", " + y + ")";

            return position;
        }
    }
}
