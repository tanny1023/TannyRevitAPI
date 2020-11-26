using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs13_UsingLogicalFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            FilteredElementCollector coll = new FilteredElementCollector(doc);

            // 篩選條件為窗和柱的篩選器
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_Columns);

            // 邏輯篩選器，用以組合兩個或多個篩選器做使用
            LogicalOrFilter orfilter = new LogicalOrFilter(filter1, filter2);

            IList<Element> elemList =
     coll.WherePasses(orfilter).WhereElementIsNotElementType().ToElements();

            // 計算元件數量
            int counter = elemList.Count;

            // 輸出柱元件的品類、族群、與類型
            string output = null;
            foreach (Element elem in elemList)
            {
                FamilyInstance familyInstance = elem as FamilyInstance;
                FamilySymbol familySymbol = familyInstance.Symbol;
                Family family = familySymbol.Family;

                output += "品類：" + elem.Category.Name + "，族群：" + family.Name + "，類型：" + elem.Name + "\n";
            }

            // 展示資訊並回傳成功
            MessageBox.Show("符合篩選條件的元件共有 " + counter + " 個，有:\n" + output);
            return Result.Succeeded;
        }
    }
}
