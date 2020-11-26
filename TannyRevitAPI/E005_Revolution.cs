using System; 
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E005_Revolution : IExternalCommand
    {
        /// <summary>
        /// This example make the selected elements revolute around z-axis
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
            trans.Start("revolution");
            foreach (ElementId elemId in ids)
            {
                Element element = doc.GetElement(elemId);
                revolution(doc, element);
            }
            trans.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        /// This method make an element revolute around z-axis
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="element"></param>
        private void revolution(Document doc, Element element)
        {
            // use two points to determine a line as the rotation axis
            XYZ axisPoint1 = new XYZ(0, 0, 0);
            XYZ axisPoint2 = new XYZ(0, 0, 10);
            Line axis = Line.CreateBound(axisPoint1, axisPoint2);
            // rotate the axis
            ElementTransformUtils.RotateElement(doc, element.Id, axis, Math.PI / 4.0);
        }
    };
}
