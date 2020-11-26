using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Text;
using System.Collections.Generic;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E004_MoveElement : IExternalCommand
    {
        /// <summary>
        /// this example move elements that define their coordinates by LocationPoint and LocationCurve
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

            StringBuilder sb = new StringBuilder();
            Transaction trans = new Transaction(doc);
            trans.Start("move selected elements");

            // for each elements in the selection, call the move function
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                XYZ translationVector = new XYZ(10, 10, 0);
                sb.AppendLine(moveElement(doc, elem, translationVector));
            }

            trans.Commit();
            MessageBox.Show(sb.ToString());
            return Result.Succeeded;
        }

        /// <summary>
        /// move one element, which may be a wall or a family instance
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elem"></param>
        /// <param name="translationVector"></param>
        /// <returns></returns>
        private string moveElement(Document doc, Element  elem, XYZ translationVector)
        {
            StringBuilder sb = new StringBuilder(); //record the translation information
            //declare two Location objects, as they are multual exclusive, one of them will be null finally 
            LocationPoint ptLocation = null;
            LocationCurve lineLocation = null;
            
            //the visible element will be either a wall or a familyinstance, these two condition are also mutual exclusive
            FamilyInstance fm = elem as FamilyInstance;
            if (fm != null)
            {
                ptLocation = fm.Location as LocationPoint;
                lineLocation = fm.Location as LocationCurve;
            }

            Wall wall = elem as Wall;
            if (wall != null)
            {
                lineLocation = wall.Location as LocationCurve;
            }

            if (ptLocation != null)
            {
                XYZ oldPlace = ptLocation.Point;
                //move an element, we provide two method, either of them works
                //ElementTransformUtils.MoveElement(doc, elem.Id, translationVector);
                ptLocation.Move(translationVector);
                //record the final coordinate                   
                XYZ newPlace = ptLocation.Point;
                //return the info as a string
                sb.AppendLine("the coordinate before translation:");
                sb.AppendLine(oldPlace.ToString());
                sb.AppendLine("the coordinate after translation:");
                sb.AppendLine(newPlace.ToString());

            }
            else //(lineLocation != null)
            {
                lineLocation.Move(translationVector);
                Line line = lineLocation.Curve as Line;
                XYZ oldPlace1 = line.GetEndPoint(0);
                XYZ oldPlace2 = line.GetEndPoint(1);
                //move an element, we provide two method, either of them works
                //ElementTransformUtils.MoveElement(doc, elem.Id, translationVector);
                lineLocation.Move(translationVector);
                //record the final coordinate
                XYZ newPlace1 = line.GetEndPoint(0);
                XYZ newPlace2 = line.GetEndPoint(1);
                //return the info as a string
                sb.AppendLine("the coordinate of start point before translation:");
                sb.AppendLine(oldPlace1.ToString());
                sb.AppendLine("the coordinate of start point before translation:");
                sb.AppendLine(oldPlace2.ToString());
                sb.AppendLine("the coordinate of end point before translation:");
                sb.AppendLine(newPlace1.ToString());
                sb.AppendLine("the coordinate of end point before translation:");
                sb.AppendLine(newPlace2.ToString());
            }
            return sb.ToString();
        }

    };
}
