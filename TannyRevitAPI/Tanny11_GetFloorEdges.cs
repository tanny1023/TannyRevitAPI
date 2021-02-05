using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Tanny11_GetFloorEdges : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            using (Transaction TemporartTransaction1 = new Transaction(doc))
            {
                TemporartTransaction1.Start("得到樓板邊線");
                Reference r = uidoc.Selection.PickObject(ObjectType.Element);
                ElementId eid = r.ElementId;
                ICollection<ElementId> eids = doc.Delete(eid);
                StringBuilder sb = new StringBuilder();
                foreach (ElementId id in eids)
                {
                    Element ele = doc.GetElement(id);
                    ModelLine mLine = ele as ModelLine;
                    if (mLine != null)
                    {
                        sb.AppendLine(mLine.Name);
                        LocationCurve curve = mLine.Location as LocationCurve;
                        Line line = curve.Curve as Line;
                        XYZ p1 = line.GetEndPoint(0);
                        XYZ p2 = line.GetEndPoint(1);
                        sb.AppendLine(p1.ToString());
                        sb.AppendLine(p2.ToString());
                    }
                }
                MessageBox.Show(sb.ToString());
                TemporartTransaction1.RollBack();
                //TemporartTransaction1.Commit();
                //TemporartTransaction1.GetStatus();
            }
            return Result.Succeeded;
        }
    }
}
