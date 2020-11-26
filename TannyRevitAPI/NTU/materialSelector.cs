using System;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace revitAPI2017
{
    public partial class materialSelector : System.Windows.Forms.Form
    {
        public Document doc;
        public Autodesk.Revit.UI.Selection.Selection selElement;

        public materialSelector(Document _doc, Autodesk.Revit.UI.Selection.Selection _selElement)
        {
            InitializeComponent();
            this.doc = _doc;
            this.selElement = _selElement; 
            //我們會做使用者選取模型的前置動作，可是是在貼材質的時候才用上，開始的材質篩選還不會用到

        }

        private void materialSelector_Load(object sender, EventArgs e)
        {

            FilteredElementCollector collector = new FilteredElementCollector(this.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            //材料篩選器的用法和類別篩選器大致相同，並且，因為材料是不可見的，所以不做Selection的前置動作
            IList<Element> matList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            foreach (Element ele in matList) //取得所有材料之後將它塞進下拉式選單之中
            {
                comboBox1.Items.Add(ele.Id + "\t" + ele.Name + "\t" + ele.Category.Name);
                //但是記得在Id及Name之間要加上適當的分隔符號，因為Name是給人看的，Id是給電腦用的，
                //稍後我們要再用Split()字串處理函式把Id拆出來
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Transaction trans = new Transaction(doc);
            trans.Start("自動貼材質");
            //從Selection當中取得ElementId集合，再取得Element，這些和之前學過的都一樣
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele = doc.GetElement(eleId);
                //下一行很重要！從下拉式選單裡再分離出使用者所選的材質Id
                ElementId eid = new ElementId(Convert.ToInt32(this.comboBox1.Text.Split('\t')[0]));
                try
                {
                    this.PaintElementFace(ele, eid);
                    //然後試著塗色，為什麼要「試」，因為有一些元件像門窗是不能貼材質的，不知它們會不會拋出例外
                }
                catch (Exception ex) { } //但是我們面對例外就讓它do nothing就好 
            }
            trans.Commit(); //最後認列交易

        }//end fun

        /// <summary>
        /// 這個函式是精華片段，講解貼材質的方法
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="matId"></param>
        public void PaintElementFace(Element elem, ElementId matId)
        {
            Document docSingleElement = elem.Document; 
            //我們第一次碰上：把單一的元件放到Document之中，之前Document總是代表整個模型
            GeometryElement geometryElement = elem.get_Geometry(new Options());
            //GeomertyElement物件會存放一個Element的所有幾何性質，在這裡我們要用上它的表面資訊
            //它會以「量體」（Solid）及「表面」（Face）的形式藏在GeomertryObject之中，
            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is Solid) //GeomertyObject是父類別，如果它實際指向了Solid子類別
                {
                    Solid solid = geometryObject as Solid; //則先將它強制轉型為Solid子類別，以使用擴充成員
                    foreach (Face face in solid.Faces) //Solid的Faces屬性可以撈出所有的量體表面
                    {
                        if (docSingleElement.IsPainted(elem.Id, face) == false) //如果這個表面還沒有上漆的話
                        {
                            docSingleElement.Paint(elem.Id, face, matId); //就把它貼上指定材質
                        }
                    }
                }
            }
        }//end fun



    }
}
