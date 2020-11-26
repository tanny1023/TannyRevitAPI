using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;


namespace RevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class myRevitApiDemo_doubleFilter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            //以上先取得使用者選取的元件範圍
            FilteredElementCollector coll = new FilteredElementCollector(doc, selectedElements);
            //上一行是過濾元件的結果集合，我們以咖啡濾紙舉例，它就像我們的咖啡壺一樣，要接住我們篩選過的結果

            // 篩選條件為窗和牆的篩選器
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            //邏輯篩選器，用以組合兩個或多個篩選器做使用，因為不可能有一個元件同時是牆又同時是窗，
            //亦即牆與窗兩種類別不可能發生交集，所以我們用聯集篩選器
            LogicalOrFilter orfilter = new LogicalOrFilter(filter1, filter2);
            IList<Element> elemList = coll.WherePasses(orfilter).WhereElementIsNotElementType().ToElements();
            //最後我們把想要的元件留在我們的「咖啡壺」上頭

            // 計算元件數量
            int counter = elemList.Count;
            // 輸出元件的品類和名稱
            string output = "";
            foreach (Element elem in elemList)
            {
                output += elem.Category.Name + ", " + elem.Name + "\t";
            }

            // 展示資訊並回傳成功
            MessageBox.Show("符合篩選條件的元件共有 " + counter + " 個，有:\n" + output);
            return Result.Succeeded;
        }
    }
}
