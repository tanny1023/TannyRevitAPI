using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class B003_MaterialFilter : IExternalCommand
    {
        /// <summary>
        /// find all the materials (whether they have been applied to the BIM model or not) in the project
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            //we do not ask users to select some element because materials are invisible elements!

            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> matList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            StringBuilder result = new StringBuilder();
            foreach (Element ele in matList)
            {
                result.AppendLine(ele.Category.Name + "\t" + ele.Name + "\t" + ele.Id);
            }
            MessageBox.Show("All the materials exist in this projects are:\r\n" + result.ToString());

            return Result.Succeeded;
        }
    };
}
