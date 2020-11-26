using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs06_CategorizedCounter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            StringBuilder st = new StringBuilder();
            Dictionary<string, int> categories = new Dictionary<string, int>();

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Category category = elem.Category;
                if (categories.ContainsKey(category.Name))
                    categories[category.Name]++;
                else
                    categories.Add(category.Name, 1);
            }

            foreach (string key in categories.Keys)
                st.AppendLine(key + ": " + categories[key].ToString());
            MessageBox.Show(st.ToString());
            return Result.Succeeded;
        }
    }
}
