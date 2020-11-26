using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class F001_ErrorMessage : IExternalCommand
    {
        /// <summary>
        /// This example highlight the selected walls and deliver a failed message 
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            message = "Please check the highlighted elements";
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                    elements.Insert(elem);
            }
            return Result.Failed;
        }
    }
}
