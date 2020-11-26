using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Tanny00_SelectFaces : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            IList<Reference> referenceCollection = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Face);
            MessageBox.Show("你共選擇了" + referenceCollection.Count.ToString() + "個面");
            return Result.Succeeded;
        }
    }
}
