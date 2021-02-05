using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Transaction(TransactionMode.Manual)]
    class Tanny14_OpenFloorAnalytycalModel
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ElementCategoryFilter filter_wall = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> floorList = collector.WherePasses(filter_wall).WhereElementIsNotElementType().ToElements();

            using (Transaction trans = new Transaction(doc, "啟用分析模型"))
            {
                trans.Start();
                foreach (Element element in floorList)
                {
                    Floor floor = element as Floor;
                    OpenFloorStructuralSignificant(floor);
                }
                trans.Commit();
            }
            MessageBox.Show("You have selected total " + floorList.Count + " elements.");
            return Result.Succeeded;
        }
        public void OpenFloorStructuralSignificant(Floor floor)
        {
            try
            {
                floor.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(1);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"啟用板的分析物件發生錯誤\n{ex}", "錯誤訊息");
                throw;
            }

        }
    }
}
