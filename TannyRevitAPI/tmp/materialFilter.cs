using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows.Forms;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class materialFilter : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            
            Document doc = commandData.Application.ActiveUIDocument.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> matList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            string result = "";
            foreach (Element ele in matList)
            {
                result+=ele.Category.Name+"\t"+ele.Name+"\t"+ele.Id+"\n";
            }
            MessageBox.Show(result);

           return Result.Succeeded;
        }
    };
}
