using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Transaction(TransactionMode.Manual)]
    class Tanny08_CreateWall : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            // Build a location line for the wall creation
            XYZ p1 = new XYZ(0, 0, 0);
            XYZ p2 = new XYZ(0, 10, 0);
            double height = 100;
            string typeName = "RC 牆 15cm";
            ElementId levelId = GetLevel(doc, "GL");
            Curve curve = Line.CreateBound(p1, p2);
            ElementId wallTypeId = GetWallType(doc, typeName).Id;
            using (Transaction trans = new Transaction(doc,"建立牆"))
            {
                trans.Start();
                CreateWall(doc, curve, wallTypeId, levelId, height);
                trans.Commit();
            }
            MessageBox.Show("牆建立完成");
            return Result.Succeeded;
        }
        public void CreateWall(Document doc, Curve curve, ElementId wallTypeId, ElementId levelId, double height)
        {
            double offset = 0;
            bool flip = true;
            bool structural = true;
            Wall.Create(doc, curve, wallTypeId, levelId, height, offset, flip, structural);
        }
        public WallType GetWallType(Document doc, string typeName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            List<WallType> allWallType = collector.OfClass(typeof(WallType)).Cast<WallType>().Where(x => x.Kind == WallKind.Basic).ToList();
            WallType wallType = allWallType.Find(w => w.Name.ToString().Equals(typeName));
            return wallType;
        }
        public ElementId GetLevel(Document doc, string levelName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_Levels));
            var query = from l in collector
                        where l.Name == levelName
                        select l;
            List<Element> levels = query.ToList();
            ElementId levelId= levels[0].Id;
            return levelId;
        }
    }
}
