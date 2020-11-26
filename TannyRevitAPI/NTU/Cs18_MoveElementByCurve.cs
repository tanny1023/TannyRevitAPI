using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs18_MoveElementByCurve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            // 宣告一個checker來確認是否移動成功
            bool checker = false;

            // Transaction狀態開始
            Transaction trans = new Transaction(doc);
            trans.Start("移動元件");

            // 移動所有被選擇的牆元件
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                    checker = moveWall(wall, 10, 50);
            }

            //Transaction狀態結束，確認移動
            trans.Commit();
            //若牆有被移動，則回傳成功訊息
            if (checker == true)
                return Result.Succeeded;
            //若牆沒有被移動，則回傳失敗訊息
            else
            {
                message = "移動元件失敗";
                return Result.Failed;
            }
        }
        // 移動牆的Function
        private bool moveWall(Wall wall, double translationX, double translationY)
        {
            // 抓取牆線
            LocationCurve wallLine = wall.Location as LocationCurve;
            XYZ translationVector = new XYZ(translationX, translationY, 0);

            // 移動並回傳成功訊息
            return wallLine.Move(translationVector);
        }
    }
}
