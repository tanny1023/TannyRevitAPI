using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Text;
using System.Windows.Forms;

namespace RevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class myRevitApiDemo_GetPosition : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc=uidoc.Document;
            Selection selElement = uidoc.Selection;

            StringBuilder stTemp = new StringBuilder("顯示各種建築元件座標:");
            //stringbuilder是一個「建構中的字串」，可以在字串後頭再加上新的內容
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element elem = doc.GetElement(eleId);
                stTemp.AppendLine("這個建築元件是："+elem.Category.Name);
                //上面一行也有助於「偷看」元件的確實名稱，名稱有時令人意外，如「樑」是「結構構架」
                if (elem.Category.Name == "牆") //判斷類別，如果元件是牆的話才執行以下內容
                {
                    LocationCurve locationCurve = elem.Location as LocationCurve;
                    Line locationLine = locationCurve.Curve as Line;
                    //以上是類別的轉型示範
                    string position = getLinePosition(locationLine);//在此要呼叫另一個函式
                    stTemp.AppendLine(position);//把成果加入暫存字串當中
                }

                else if (elem.Category.Name == "結構柱") //判斷類別，如果元件是柱的話才執行以下內容
                {
                    LocationPoint locationPoint = elem.Location as LocationPoint;
                    XYZ Point = locationPoint.Point as XYZ;
                    //以上是類別的轉型示範
                    string position = getPointPosition(Point);//在此要呼叫另一個函式
                    stTemp.AppendLine(position);//把成果加入暫存字串當中
                }

                else if (elem.Category.Name == "結構構架") //判斷類別，如果元件是梁的話才執行以下內容
                {
                    LocationCurve locationCurve = elem.Location as LocationCurve;
                    Line locationLine = locationCurve.Curve as Line;
                    //以上是類別的轉型示範
                    string position = getLinePosition(locationLine);//在此要呼叫另一個函式
                    stTemp.AppendLine(position);//把成果加入暫存字串當中
                }

                else if (elem.Category.Name == "門") //判斷類別，如果元件是梁的話才執行以下內容
                {
                    LocationPoint locationPoint = elem.Location as LocationPoint;
                    XYZ Point = locationPoint.Point as XYZ;
                    //以上是類別的轉型示範
                    string position = getPointPosition(Point);//在此要呼叫另一個函式
                    stTemp.AppendLine(position);//把成果加入暫存字串當中
                }

                else if (elem.Category.Name == "窗") //判斷類別，如果元件是梁的話才執行以下內容
                {
                    LocationPoint locationPoint = elem.Location as LocationPoint;
                    XYZ Point = locationPoint.Point as XYZ;
                    //以上是類別的轉型示範
                    string position = getPointPosition(Point);//在此要呼叫另一個函式
                    stTemp.AppendLine(position);//把成果加入暫存字串當中
                }


                else
                { }

            }
            MessageBox.Show(stTemp.ToString());//最後把成果一次輸出
            return Result.Succeeded;
        }
        /// <summary>
        /// 這個函式輸入一條線物件，然後抓出線的兩端之XY座標，以字串的方式傳回來
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string getLinePosition(Line line)
        {
            XYZ startPoint = line.GetEndPoint(0);//牆心線起點
            XYZ endPoint = line.GetEndPoint(1);//牆心線終點
            // 在2014以前取得點位座標的方法為get_EndPoint()，而在2014方法更新為GetEndPoint()
            string x1 = startPoint.X.ToString();
            string y1 = startPoint.Y.ToString();
            string z1 = startPoint.Z.ToString();

            string x2 = endPoint.X.ToString();
            string y2 = endPoint.Y.ToString();
            string z2 = endPoint.Z.ToString();

            string position = "起點( " + x1 + ", " + y1 + ", " + z1+ ")，終點( " + x2 + ", " + y2 +", " + z2 + ")";
            return position;
        }

        private string getPointPosition(XYZ point)
        {
            string position = "點( " + point.X + ", " + point.Y+ ", " + point.Z + ")";
            return position;
        }
    }
}
