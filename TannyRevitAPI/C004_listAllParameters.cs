using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class C004_ListParameters : IExternalCommand
    {
        /// <summary>
        /// print all the type parameters  for wall
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
                       
            //select all the walls in a BIM model by element filter
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            string modelPath = doc.PathName;
            string modelTitle = doc.Title;
            StreamWriter sw = new StreamWriter(modelPath.Replace(doc.Title+".rvt","Result.txt"));

            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> wallList = collector.WherePasses(filter).WhereElementIsElementType().ToElements();

            StringBuilder sb = new StringBuilder("this project conatins the following walls:\r\n");
            foreach (Element wall in wallList) //for each wall
            {
                sb.AppendLine("\r\ncategory :: "+wall.Name);
                sw.WriteLine("\r\ncategory :: " + wall.Name);
                foreach (Parameter para in wall.Parameters) //for each parameter
                {
                    sb.AppendLine(para.Definition.Name);
                    sw.WriteLine(para.Definition.Name);
                }
                sb.AppendLine();
            }
            sw.Flush();
            sw.Close();
            MessageBox.Show(modelTitle);
            MessageBox.Show(modelPath);
            MessageBox.Show(sb.ToString());
            return Result.Succeeded;
        }

    };
}
