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
    public partial class parameterManipulator : System.Windows.Forms.Form
    {

        private Document doc;
        private UIDocument uidoc;
        public parameterManipulator(Document _doc, UIDocument _uidoc)
        {
            InitializeComponent();
            this.doc = _doc;
            this.uidoc = _uidoc;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            integrateParameters myIntegratePara = new integrateParameters();
            myIntegratePara.getDataTableBySpecificCategory(doc, BuiltInCategory.OST_Walls);
            myIntegratePara.getDataTableBySpecificCategory(doc, BuiltInCategory.OST_StructuralColumns);
            myIntegratePara.getDataTableBySpecificCategory(doc, BuiltInCategory.OST_StructuralFraming);
            MessageBox.Show("成功匯出參數表格！");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            integrateParameters myIntegratePara = new integrateParameters();
            myIntegratePara.setNewParametersToCategory(this.doc, this.uidoc, BuiltInCategory.OST_Walls);
            myIntegratePara.setNewParametersToCategory(this.doc, this.uidoc, BuiltInCategory.OST_StructuralColumns);
            myIntegratePara.setNewParametersToCategory(this.doc, this.uidoc, BuiltInCategory.OST_StructuralFraming);
            MessageBox.Show("成功匯入新增參數並設定數值！");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            integrateParameters myIntegratePara = new integrateParameters();
            myIntegratePara.hw3_extractPositionStringIntoNotes(this.doc);
            myIntegratePara.hw3_addAndSetParaToBIM(this.uidoc, this.doc);
            MessageBox.Show("成功設定及新增門窗製造商參數至BIM模型！");
        }
    }
}
