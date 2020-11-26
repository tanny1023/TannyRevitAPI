using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class Tanny01_DividingUVmesh : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            IList<Reference> referenceCollection = uidoc.Selection.PickObjects(ObjectType.Face);
            MessageBox.Show("You have selected total " + referenceCollection.Count.ToString() + " faces.");
            Transaction trans = new Transaction(doc);
            trans.Start("建立網格");
            foreach (Reference face in referenceCollection)
            {
                DividedSurface ds = DividedSurface.Create(doc, face);
                SpacingRule srU = ds.USpacingRule;
                srU.SetLayoutFixedDistance(1, SpacingRuleJustification.Center, 0, 0);
                SpacingRule srV = ds.VSpacingRule;
                srV.SetLayoutFixedDistance(1, SpacingRuleJustification.Center, 0, 0);
                Element host = ds.Host;
                Face dividefaces = host.GetGeometryObjectFromReference(ds.HostReference) as Face;
                GridNode node = new GridNode();
                UV uv = ds.GetGridNodeUV(node);
                XYZ pt = dividefaces.Evaluate(uv);
                MessageBox.Show(pt.ToString());
            }

            trans.Commit();
            return Result.Succeeded;
        }
    }
}
