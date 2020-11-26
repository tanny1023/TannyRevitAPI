using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E001_GetCoordinate : IExternalCommand
    {
        /// <summary>
        /// show the coordinates for walls, columns, beams, windows, and doors
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc=uidoc.Document;
            Selection selElement = uidoc.Selection;

            StringBuilder strTemp = new StringBuilder();
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element elem = doc.GetElement(eleId);
                strTemp.AppendLine("This element is："+elem.Category.Name);//get the category name
                //if the element is a wall
                if (elem.Category.Name == "牆" || elem.Category.Name == "Walls" ||
                    elem.Category.Name == "結構柱" || elem.Category.Name == "Structural Columns" ||
                    elem.Category.Name == "結構構架" || elem.Category.Name == "Structural Framing" ||
                    elem.Category.Name == "門" || elem.Category.Name == "Doors" ||
                    elem.Category.Name == "窗" || elem.Category.Name == "Windows")
                    {
                        strTemp.AppendLine(getCoordinateString(elem));
                    }
            }
            MessageBox.Show(strTemp.ToString());//show the results
            return Result.Succeeded;
        }

        /// <summary>
        /// if the element location is based on LocationPoint or LocationCurve, return its coordinates
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private string getCoordinateString(Element elem)
        {
            string position = "default";
            LocationPoint locationPoint = elem.Location as LocationPoint;
            if (locationPoint != null)
            {
                XYZ point = locationPoint.Point as XYZ;
                position = "Point: "+point.ToString();
            }

            LocationCurve locationCurve = elem.Location as LocationCurve;
            if (locationCurve != null)
            {
                Line locationLine = locationCurve.Curve as Line;
                //convert the location to a line
                XYZ startPoint = locationLine.GetEndPoint(0);//get the first end point
                XYZ endPoint = locationLine.GetEndPoint(1);//get the second end point
                //link the coordinates of two end points as the return value
                position = "Start Point: "+startPoint.ToString()+", End Point: "+endPoint.ToString();               
            }
            return position;
        }

    };
}
