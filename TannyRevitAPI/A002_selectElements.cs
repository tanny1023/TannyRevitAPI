using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class A002_SelectElement : IExternalCommand
    {
        /// <summary>
        /// Select by element and count the total number
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            //get all the selected elements
            int totalCount = selectedElements.Count;
            MessageBox.Show("You have selected total " + totalCount.ToString() + " elements.");

            return Result.Succeeded;
        }//end fun

    } //end class
}
