using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text;
using System.Collections.Generic;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs17_MoveElementByPoint : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // 抓取Revit模型中被選取的元件
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            // 建立StringBuilder
            StringBuilder sb = new StringBuilder();

            // 開始一個transaction，每個改變模型的任何動作都需在transaction中進行
            Transaction trans = new Transaction(doc);
            trans.Start("移動元件");

            // 對於被選擇的每個元件，若其是FamilyInstance則呼叫移動的Function
            foreach (ElementId elemId in ids)
            {
                Element element = doc.GetElement(elemId);
                FamilyInstance c = element as FamilyInstance;
                if (c != null)
                    sb.AppendLine(moveColumn(doc, c));
            }

            // 確認資訊改變,transaction結束
            trans.Commit();
            // 展示資訊
            MessageBox.Show(sb.ToString());
            return Result.Succeeded;
        }
        // 移動柱的Function
        private string moveColumn(Document doc, FamilyInstance column)
        {
            // 抓取柱的位置並記錄起始位置
            LocationPoint columnLocation = column.Location as LocationPoint;
            XYZ oldPlace = columnLocation.Point;

            // 給予位移值 
            XYZ translationVector = new XYZ(10, 20, 30);
            // 此處雖然有給Z方向的移動量，但因為柱子底部必須與樓板做連接，因此柱子並不會做Z方向的移動

            // 將Document(doc)內的column移動
            ElementTransformUtils.MoveElement(doc, column.Id, translationVector);

            // 抓取柱的位置並記錄最後位置
            columnLocation = column.Location as LocationPoint;
            XYZ newPlace = columnLocation.Point;

            // 回傳資訊
            string info = "元件移動前的座標為:\n " +
                      "(" + oldPlace.X + ", " + oldPlace.Y + ", " + oldPlace.Z + ")" +
                      "\n元件移動後的座標為:\n" +
                      "(" + newPlace.X + ", " + newPlace.Y + ", " + newPlace.Z + ")";
            return info;
        }
    }
}
