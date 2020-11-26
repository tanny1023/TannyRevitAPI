using System; // 要使用Exception需引用此參考
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs14_DeleteElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            // 開始一個transaction，每個改變模型的動作都需在transaction中進行
            Transaction trans = new Transaction(doc);
            trans.Start("刪除元件");

            // 刪除選取的元件
            foreach (ElementId elemId in ids)
            {
                deleteElement(doc, elemId);
            }

            trans.Commit();
            return Result.Succeeded;
        }

        // 刪除元件的Function
        private void deleteElement(Autodesk.Revit.DB.Document document, ElementId elemId)
        {

            // 將指定元件以及所有與該元件相關聯的元件刪除，並將刪除後所有的元件存到到容器中
            ICollection<Autodesk.Revit.DB.ElementId> deletedIdSet = document.Delete(elemId);

            // 可利用上述容器來查看刪除的數量，若數量為0，則刪除失敗，提供錯誤訊息
            if (deletedIdSet.Count == 0)
            {
                throw new Exception("選取的元件刪除失敗");
            }
        }
    }
}
