using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Tanny00_SelectFaces : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Reference r = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Face);
            ElementId eid = r.ElementId;
            Element ele = doc.GetElement(eid);
            GeometryObject topFace = ele.GetGeometryObjectFromReference(r);
            Face face = topFace as Face;
            //--面的法線方向
            PlanarFace planarFace = face as PlanarFace;
            XYZ normalvector = planarFace.FaceNormal;
            MessageBox.Show("面的法線方向=" + normalvector.ToString());
            return Result.Succeeded;
        }
    }
}
