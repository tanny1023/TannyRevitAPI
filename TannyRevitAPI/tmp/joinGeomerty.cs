using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    class joinGeomertry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //以下都很好理解
            UIDocument uidoc = commandData.Application.ActiveUIDocument; //取得目前模型
            Document doc = uidoc.Document;
            //ICollection<ElementId> elemids = uidoc.Selection.GetElementIds();
            //取得所有元件id

            Application.EnableVisualStyles();
            Application.Run(new jointElementPanel(doc, uidoc));

            return Result.Succeeded;

        }

        public static void joinGeoByTwoCategories(Document doc, BuiltInCategory firstCategory, BuiltInCategory secondCategory)
        {
            FilteredElementCollector coll = new FilteredElementCollector(doc);
            // 篩選條件牆的篩選器
            ElementCategoryFilter filterFirstCategory = new ElementCategoryFilter(firstCategory);
            IList<Element> FirstCategoryElements = coll.WherePasses(filterFirstCategory)
                .WhereElementIsNotElementType().ToElements();                      
            //因為元件接合要做寫入改動，因此要將它放入交易
            //針對所有元件作自動接合
            foreach (Element firstElement in FirstCategoryElements)
            {
                BoundingBoxXYZ bbx = firstElement.get_BoundingBox(null);
                //從第一元件取得「邊界長方體」（包覆元件邊界的長方體，如果元件本身是長方體，就會完全包覆）
                //OutLine是一條線，此處等於直接拿包覆長方體的對角線來定義它
                Outline outline = new Outline(bbx.Min, bbx.Max);//Min及Max各是一個點，都能存取XYZ座標
                BoundingBoxIntersectsFilter filterIntersection = new BoundingBoxIntersectsFilter(outline);
                //這個過濾器會取得「所有和這個BoundingBox相交的BoundingBox，並且傳回其所代表之元件」
                ElementCategoryFilter filterSecondCategory = new ElementCategoryFilter(secondCategory); 
                //然後在相交元件當中，我只想先處理第一類別與第二類別的相交，所以需要再一個篩選器
                LogicalAndFilter andfilter = new LogicalAndFilter(filterIntersection, filterSecondCategory); //用交集篩選器將它們組合起來

                IList<Element> adjacents = new FilteredElementCollector(doc).WherePasses(andfilter).
                    WhereElementIsNotElementType().ToElements();
                //以上一行選取所有和第一元件相鄰，而且屬於第二類別的元件，把整個doc放進Collector後再濾出通過篩選器的元件
                foreach (Element secondElement in adjacents)
                {
                    //MessageBox.Show(secondElement.Id + "\n" + secondElement.Category.Name); //debug
                    Transaction trans = new Transaction(doc);
                    try
                    {
                        trans.Start("join"); //開始交易（接合）
                        if (JoinGeometryUtils.AreElementsJoined(doc, firstElement, secondElement) == true) //如果兩個元件已接合
                            JoinGeometryUtils.UnjoinGeometry(doc, firstElement, secondElement); //先解除接合，因為我假設它不是我要的結果
                        //以上那個動作有點近似我們在Revit裡頭除了「接合」、「取消接合」以外，也可以「改變接合順序」
                        JoinGeometryUtils.JoinGeometry(doc, firstElement, secondElement); //再重新接合，但原本就處於未接合的元件也會被接合
                    }
                    catch (Exception ex)
                    {
                        //MessageBox.Show(ex.ToString());
                    }
                    finally //這裡刻意讓finally陳述句登場，只是強調無論如何都要做交易的commit
                    {
                        trans.Commit();
                    }
                }//end inner foreach
            } //end outer foreach
            
        }//end fun


    };
}
