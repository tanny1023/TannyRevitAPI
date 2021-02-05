using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Windows;

namespace TannyRevitAPI
{
    [Transaction(TransactionMode.Manual)]
    class Tanny10_WallJoined : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Autodesk.Revit.DB.View target_view = doc.ActiveView;
            FilteredElementCollector elementInView = new FilteredElementCollector(doc, target_view.Id);
            ElementCategoryFilter filter_wall = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> Walls = elementInView.WherePasses(filter_wall).WhereElementIsNotElementType().ToElements();
            using (Transaction trans = new Transaction(doc, "連接牆"))
            {
                trans.Start();
                foreach (Element ele in Walls)
                {
                    Wall wall = ele as Wall;
                    //--允許牆接合
                    WallUtils.AllowWallJoinAtEnd(wall, 0);
                    WallUtils.AllowWallJoinAtEnd(wall, 1);
                    LocationCurve curve = wall.Location as LocationCurve;
                    for (int i = 0; i < 2; i++)
                    {
                        curve.set_JoinType(i, JoinType.Miter);
                    }
                }
                trans.Commit();
            }
            MessageBox.Show("連接牆完成");
            return Result.Succeeded;
        }
    }
}
