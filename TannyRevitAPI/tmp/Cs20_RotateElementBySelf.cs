using System;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs20_RotateElementBySelf : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            // 宣告一個checker來確認是否旋轉成功
            bool checker = false;

            Transaction trans = new Transaction(doc);
            trans.Start("旋轉元件");

            // 旋轉被選取的每個元件
            foreach (ElementId elemId in ids)
            {
                Element element = doc.GetElement(elemId);
                checker = rotateZ_BySelf(element);
            }
            trans.Commit();

            // 若每個元件都被旋轉，則回傳成功
            if (checker == true)
                return Result.Succeeded;
            // 若失敗，則回傳失敗訊息
            else
            {
                message = "旋轉元件失敗";
                return Result.Failed;
            }
        }
        // 對自身旋轉的Function
        private bool rotateZ_BySelf(Element element)
        {
            // 抓取自身的定位線
            LocationCurve locationCurve = element.Location as LocationCurve;
            bool successful = false;

            if (locationCurve != null)
            {
                // 抓取定位線的起始點，並對起始點設一個平行Z軸的旋轉軸線
                Curve curve = locationCurve.Curve;
                XYZ axisPoint1 = curve.GetEndPoint(0);
                XYZ axisPoint2 = new XYZ(axisPoint1.X, axisPoint1.Y, axisPoint1.Z + 10);
                Line axis = Line.CreateBound(axisPoint1, axisPoint2);

                // 以此旋轉軸線為中心旋轉，並回傳訊息
                successful = locationCurve.Rotate(axis, Math.PI / 4.0);
            }
            // 回傳訊息
            return successful;
        }
    }
}
