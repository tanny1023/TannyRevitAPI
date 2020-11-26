using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    
    public class A004_SelectPoint : IExternalCommand
    {
        /// <summary>
        /// This example let users pick up four points and then print their coordinates on the screen
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
  
            XYZ point1 = uidoc.Selection.PickPoint();
            XYZ point2 = uidoc.Selection.PickPoint();
            XYZ point3 = uidoc.Selection.PickPoint();
            XYZ point4 = uidoc.Selection.PickPoint();

            //the ToString() method of XYZ objects can output the coordiante directly 
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(point1.ToString());
            sb.AppendLine(point2.ToString());
            sb.AppendLine(point3.ToString());
            sb.AppendLine(point4.ToString());

            MessageBox.Show("You have selected these points:\r\n"+sb.ToString());

            return Result.Succeeded;
        }
    }
}
