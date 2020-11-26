using System.Windows.Forms;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class A003_SelectEdge : IExternalCommand
    {
        /// <summary>
        /// select edges and count the total number
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            IList<Reference> referenceCollection = uidoc.Selection.PickObjects(ObjectType.Face);
            MessageBox.Show("You have selected total "+referenceCollection.Count.ToString()+" faces.");
     
            return Result.Succeeded;
        }
    }
}