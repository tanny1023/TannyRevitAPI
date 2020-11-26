using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class F005_AutomaticPaintWinForm : IExternalCommand
    {
        /// <summary>
        /// This example use Revit API to call winform for performing automatic paint tool 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            //this is how we launch a windows form inside a Revit API application
            Application.EnableVisualStyles();
            Application.Run(new F005_MaterialSelector(doc, selElement));

            return Result.Succeeded;
        }//end fun
    };
}