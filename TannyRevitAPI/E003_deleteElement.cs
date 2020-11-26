using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E003_DeleteElement : IExternalCommand
    {
        /// <summary>
        /// this example delete the selected elements
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            Transaction trans = new Transaction(doc);
            trans.Start("delete element");

            // delete all the selected elements
            List<ElementId> deletedTotal = new List<ElementId>();
            foreach (ElementId elemId in ids)
            {
               ICollection<ElementId> deletedItems= doc.Delete(elemId);
                foreach (ElementId id in deletedItems)
                {
                    deletedTotal.Add(id);
                    Element elem = doc.GetElement(id);
                }
            }

            trans.Commit();
            MessageBox.Show("You have deleted " + deletedTotal.Count + " elements, including the invisible elements. ");

            return Result.Succeeded;
        }
    }
}
