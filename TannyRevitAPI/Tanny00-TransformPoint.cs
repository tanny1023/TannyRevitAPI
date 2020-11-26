using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class TransformPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;

            XYZ point = uidoc.Selection.PickPoint();
            Transform transform = Transform.Identity;
            transform.BasisX = new XYZ(2, 2, 2);
            transform.BasisY = new XYZ(3, 3, 3);
            transform.BasisZ = new XYZ(4, 4, 4);
            transform.Origin = new XYZ(1, 2, 3);
            XYZ b0 = transform.OfPoint(point);
            XYZ b1 = transform.get_Basis(1);
            XYZ b2 = transform.get_Basis(2);
            XYZ b3 = transform.Origin;
            MessageBox.Show($"XYZ={point.ToString()}");
            MessageBox.Show($"XYZ=\n{b0.ToString()}\n{b1.ToString()}\n{b2.ToString()}\n{b3.ToString()}");

            return Result.Succeeded;
        }
    }
}
