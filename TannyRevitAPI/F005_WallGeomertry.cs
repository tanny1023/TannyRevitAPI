using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class F005_WallGeomertry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            // 在使用者選取的元件中擷取出牆，並檢索其幾何形狀資訊
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                    getFacesAndEdges(wall);
            }

            return Result.Succeeded;
        }
        private void getFacesAndEdges(Wall wall)
        {
            string info = "";

            // 建立檢索幾何選項的設定，你可以設定幾何的精細程度等選項，若沒有指定則使用預設選項
            Autodesk.Revit.DB.Options opt = new Options();

            // 使用牆類別的Geometry屬性檢索Geometry.Element物件，這個物件包含GeometryObject屬性中
            // 所有的幾何物件，如Solid、Line，等等。
            Autodesk.Revit.DB.GeometryElement geomElem = wall.get_Geometry(opt);

            foreach (GeometryObject geomObj in geomElem)
            {
                // 迭代GeometryObject屬性以取得Solid物件，而Solid物件又包含了Face及Edge物件，
                // 代表組成牆幾何形狀的邊以及面
                Solid geomSolid = geomObj as Solid;
                if (geomSolid != null)
                {
                    // 計算Face以及Edge數量，以及組成牆幾何形狀的Face總面積
                    int faces = 0;
                    int edges = 0;
                    double totalArea = 0;
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        faces++;
                        totalArea += geomFace.Area * 0.3048 * 0.3048; // 英制單位轉公制單位
                    }
                    foreach (Edge geomEdge in geomSolid.Edges)
                        edges++;

                    info += "選取的牆總共由 " + edges + " 個邊，\n";
                    info += "以及 " + faces + " 個面所組成，\n";
                    info += "所有面的總面積為 " + totalArea.ToString("f2") + " 平方公尺。";
                    // ToString("f2")代表以小數點後兩位的方式顯示
                }
            }
            TaskDialog.Show("Revit", info);
        }
    }
}
