using Autodesk.Revit.Creation;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace revitAPI2017
{
    public partial class jointElementPanel : System.Windows.Forms.Form
    {
        private Autodesk.Revit.DB.Document doc;
        private UIDocument uidoc;


        public jointElementPanel(Autodesk.Revit.DB.Document _doc, UIDocument _uidoc)
        {
            InitializeComponent();
            this.doc = _doc;
            this.uidoc = _uidoc;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            ICollection<ElementId>elementIds= selElement.GetElementIds();

            foreach (ElementId elementId in elementIds)
            {
                Element elem = doc.GetElement(elementId);
                BoundingBoxXYZ bbx = elem.get_BoundingBox(null);
                //從元件取得「邊界長方體」（包覆元件邊界的長方體，如果元件本身是長方體，就會完全包覆）
                //OutLine是一條線，此處等於直接拿包覆長方體的對角線來定義它
                Outline outline = new Outline(bbx.Min, bbx.Max);//Min及Max各是一個點，都能存取XYZ座標
                BoundingBoxIntersectsFilter filterIntersection = new BoundingBoxIntersectsFilter(outline);
                //這個過濾器會取得「所有和這個BoundingBox相交的BoundingBox，並且傳回其所代表之元件」
                IList<Element> Intersects = 
                    new FilteredElementCollector(doc).WherePasses(filterIntersection).WhereElementIsNotElementType().ToElements();

                StringBuilder sb = new StringBuilder("這個元件是"+elem.Category.Name+":"+elem.Id+":"+elem.Name+"，");
                sb.AppendLine("和它相鄰的元件有：");

                foreach (Element eleFiltered in Intersects)
                {
                    try //這邊需要用try包起來，因為我發現有些Element似乎是沒有Category.Name的，這會發生空參考錯誤
                    {
                        sb.AppendLine(eleFiltered.Category.Name + ":" + eleFiltered.Id + ":" + eleFiltered.Name);
                        //印出相鄰元件資訊
                    }
                    catch (Exception ex)
                    { }
                }
                sb.AppendLine("=======");
                MessageBox.Show(sb.ToString());
            }//end foreach
            
        }//end fun

        private void button2_Click(object sender, EventArgs e)
        {
            joinGeomertry.joinGeoByTwoCategories(doc, BuiltInCategory.OST_Walls, BuiltInCategory.OST_StructuralFraming);
            joinGeomertry.joinGeoByTwoCategories(doc, BuiltInCategory.OST_Floors, BuiltInCategory.OST_Walls);
            MessageBox.Show("完成元件結合！");
        }
    }
}
