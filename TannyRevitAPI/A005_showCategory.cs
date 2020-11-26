using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class A005_ShowCategoryByDictionary : IExternalCommand
    {
        /// <summary>
        /// to print the category and the number of instances for the selected items
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            StringBuilder st = new StringBuilder();
            Dictionary<string, int> categories = new Dictionary<string, int>();

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId); //get the element by id
                Category category = elem.Category; //get the category by element
                if (categories.ContainsKey(category.Name))
                    categories[category.Name]++; //count the number of instances for a category
                else
                    categories.Add(category.Name, 1); //for the first time, set the initial number of category 
            }

            //show the total number of each category
            foreach (string key in categories.Keys)
                st.AppendLine(key + ": " + categories[key].ToString());
            MessageBox.Show(st.ToString());
            return Result.Succeeded;
        }
    }
}
