using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class E002_CreateSlabByPoint : IExternalCommand
    {
        /// <summary>
        /// Let user to select four points for determining the edge of a rectengular slab
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Transaction tx = new Transaction(doc);
            tx.Start("Create Sloped Slab");
  
            XYZ point1 = uidoc.Selection.PickPoint();
            XYZ point2 = uidoc.Selection.PickPoint();
            XYZ point3 = uidoc.Selection.PickPoint();
            XYZ point4 = uidoc.Selection.PickPoint();

            Line line1 = Line.CreateBound(point1, point2);
            Line line2 = Line.CreateBound(point2, point3);
            Line line3 = Line.CreateBound(point3, point4);
            Line line4 = Line.CreateBound(point4, point1);

            CurveArray curveArray = new CurveArray();
            curveArray.Append(line1);
            curveArray.Append(line2);
            curveArray.Append(line3);
            curveArray.Append(line4);

            Level level = new FilteredElementCollector(doc).OfClass(typeof(Level)).Where<Element>(e => e.Name.Equals("2F")).FirstOrDefault<Element>() as Level;
            Floor floor = doc.Create.NewSlab(curveArray, level, curveArray.get_Item(1) as Line, 0, true);

            tx.Commit();
            return Result.Succeeded;
        }
    }
}
