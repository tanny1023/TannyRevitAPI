using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class B001_ElementbyFilter : IExternalCommand
    {
        /// <summary>
        /// filter out all of the columns within a BIM model and print their properties 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            //ICollection<ElementId> ids = uidoc.Selection.GetElementIds(); //we don't select any element in this example
            //in other word, this application is applied to the whole BIM model
            Document doc = uidoc.Document;

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            //Filter out all the columns
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            IList<Element> columnList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
            // WherePasses(filter): use filter object to only select columns
            // WhereElementIsNotElementType(): we only focus on real column objects (element type itself is also an element)
            // ToElements(): Convert the results to Element objects 
            StringBuilder output =new StringBuilder( "All the structural columns in this BIM model are:\r\n");
            foreach (Element elem in columnList)
            {
                //print the category, the family and the name of the columns
                FamilyInstance familyInstance = elem as FamilyInstance;
                FamilySymbol familySymbol = familyInstance.Symbol;
                Family family = familySymbol.Family;
                string elemName = "Category:" + elem.Category.Name + ", Family:" + family.Name + ", Name:" + elem.Name + "\r\n";
                output.Append(elemName);
            }
            MessageBox.Show(output.ToString());
            return Result.Succeeded;
        }
    }
}
