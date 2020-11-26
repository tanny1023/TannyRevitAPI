using Autodesk.Revit.DB; // for using Revit API
using Autodesk.Revit.UI.Selection; //for using Revit API
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    //as there is also a Form class under Revit API, we must use full name with namespace
    public partial class F005_MaterialSelector : System.Windows.Forms.Form
    {
        public Document doc;
        public Selection selElement;
        //we must pass the BIM model (_doc) and the selected element (_selElement) 
        public F005_MaterialSelector(Document _doc, Selection _selElement)
        {
            this.doc = _doc;
            this.selElement = _selElement;
            InitializeComponent();
        }

        private void F005_MaterialSelector_Load(object sender, EventArgs e)
        {
            FilteredElementCollector collector = new FilteredElementCollector(this.doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> matList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
            //use material filter to get all the materials that have been included in the current project
            foreach (Element ele in matList) //and put all of the materials in the combo box dynamically
            {
                this.comboBoxMaterial.Items.Add(ele.Id + "\t" + ele.Name + "\t" + ele.Category.Name);
            }

        }

        private void buttonPaint_Click(object sender, EventArgs e)
        {
            Transaction trans = new Transaction(doc);
            trans.Start("automatic paint");
            //for each selected element
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele = doc.GetElement(eleId);
                //get the material name from the dropdown list
                ElementId eid = new ElementId(Convert.ToInt32(this.comboBoxMaterial.Text.Split('\t')[0]));
                try
                {
                    this.PaintElementFace(ele, eid);
                    //try to paint. Some of the element such as windows/doors cannot have attached material
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"例外狀況:{ex}");
                } //if the element is not paintable, just do nothing
            }
            trans.Commit();
        }

        public void PaintElementFace(Element elem, ElementId matId)
        {
            GeometryElement geometryElement = elem.get_Geometry(new Options());
            //get the geomertry objects from the elements 
            foreach (GeometryObject geometryObject in geometryElement)
            {
                //Solid solid = geometryObject as Solid;
                //if (solid!=null) //if the geomertry object is a solid
                if (geometryObject is Solid)
                {
                    Solid solid = geometryObject as Solid;
                    foreach (Face face in solid.Faces) //get all the faces of the solid, for each face
                    {
                        if (elem.Document.IsPainted(elem.Id, face) == false) //if not paint
                        {
                            elem.Document.Paint(elem.Id, face, matId); //paint by specified material
                        }
                    }
                }
            }
        }//end fun

    };
}
