using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class B002_LogicalFilter : IExternalCommand
    {
        /// <summary>
        /// filter out windows or doors within the user selected elements, and then print their properties 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;
            //get selected elements from users

            //decalre two filters
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            LogicalOrFilter orfilter = new LogicalOrFilter(filter1, filter2); //and combine them by LogicalORFilter

            FilteredElementCollector collector = new FilteredElementCollector(doc, selectedElements);
            //the collector object can take one argument (just doc) or two arguments (doc and the selected elements)
            IList<Element> elemList = collector.WherePasses(orfilter).WhereElementIsNotElementType().ToElements();
            //get the elements that passed our filters

            int counter = elemList.Count;
            StringBuilder output = new StringBuilder();
            foreach (Element elem in elemList)
            {
                output.AppendLine(elem.Category.Name + ", " + elem.Name);
            }

            MessageBox.Show("Total " + counter + " elements are windows or doors, they are:\r\n" + output.ToString());
            return Result.Succeeded;
        }
    }
}
