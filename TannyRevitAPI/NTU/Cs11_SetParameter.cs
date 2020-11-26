using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs11_SetParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            Transaction trans = new Transaction(doc);
            trans.Start("設定參數");

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                setMark(elem, "Hello World");
            }

            trans.Commit();
            MessageBox.Show("選擇的元件已被標註");
            return Result.Succeeded;
        }

        private void setMark(Element elem, string s)
        { elem.LookupParameter("備註").Set(s); }
    }
}
