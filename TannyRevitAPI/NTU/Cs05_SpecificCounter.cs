using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;  // 使用StringBuilder需要引用此參考

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs05_SpecificCounter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            StringBuilder st = new StringBuilder();
            int wallCounter = 0;
            int familyCounter = 0;

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null) wallCounter++;
                FamilyInstance familyInstance = elem as FamilyInstance;
                if (familyInstance != null) familyCounter++;
            }

            st.AppendLine("有 " + wallCounter + " 面牆被選取");
            st.AppendLine("有 " + familyCounter + " 個族群實作元件被選取");

            MessageBox.Show(st.ToString());
            return Result.Succeeded;
        }
    }
}
