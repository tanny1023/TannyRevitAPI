using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Tanny00_FilterColumnandBeam : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralColumns);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_StructuralFraming);
            LogicalOrFilter filter = new LogicalOrFilter(filter1, filter2);
            IList<Element> beamList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();
            StringBuilder result = new StringBuilder();
            foreach (Element ele in beamList)
            {
                result.AppendLine(ele.Category.Name + "\t" + ele.Name + "\t" + ele.Id);
            }
            MessageBox.Show(result.ToString());
            return Result.Succeeded;
        }
    }
}
