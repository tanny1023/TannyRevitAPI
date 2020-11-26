using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E006_Rotation : IExternalCommand
    {
        /// <summary>
        /// This method makes the selected elements rotate on its own axis
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
            trans.Start("rotation");
            foreach (ElementId elemId in ids)
            {
                Element element = doc.GetElement(elemId);
                rotateColumn(doc, element);
            }
            trans.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        /// This method makes an column rotate on its own axis
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private void rotateColumn(Document doc, Element element)
        {
            //if the element use location point
            LocationPoint locationPoint = element.Location as LocationPoint;
            if (locationPoint != null)
            {
                XYZ pt1 = locationPoint.Point;
                XYZ pt2 = new XYZ(pt1.X, pt1.Y, pt1.Z + 10); //create an axis along z-direction
                Line axis = Line.CreateBound(pt1, pt2); 
                //rotate the element
                ElementTransformUtils.RotateElement(doc, element.Id, axis, Math.PI / 4.0);
            }
        }
    };
}
