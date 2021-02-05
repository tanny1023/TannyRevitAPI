using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class F002_IfAndError : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;
            bool checker = true;

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Category category = elem.Category;
                if (category.Name != "牆")
                {
                    checker = false;
                    elements.Insert(elem);
                }
            }

            if (checker == true)
            {
                MessageBox.Show("OK");
                return Result.Succeeded;
            }
            else
            {
                message = "不允許選擇牆以外的元件";
                return Result.Failed;
            }
        }
    }
}
