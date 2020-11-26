using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class F002_WallGeomertry : IExternalCommand
    {
        /// <summary>
        /// This example get all the faces and edges from a selected wall
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

            //if the selected element is a wall, look up all of the faces and edges
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                    getFacesAndEdges(wall);
            }

            return Result.Succeeded;
        }
   
        /// <summary>
        /// get all the faces and edges of a wall
        /// </summary>
        /// <param name="wall"></param>
        private void getFacesAndEdges(Wall wall)
        {
            // set the related parameters for the level of information,
            //we will use default value here
            Options opt = new Options();
            //This method will help us get all the geomertry object from a wall, including Solid, Face, Line, etc.
            GeometryElement geomElem = wall.get_Geometry(opt);

            foreach (GeometryObject geomObj in geomElem)
            {
                //for each geomertry object, get solids 
                Solid geomSolid = geomObj as Solid;
                if (geomSolid != null)
                {
                    //each solid will contain faces and edges, we will count for the number and the area
                    int faces = 0;
                    int edges = 0;
                    double totalArea = 0;
                    foreach (Face geomFace in geomSolid.Faces)
                    {
                        faces++;
                        totalArea += geomFace.Area * 0.3048 * 0.3048; // use SI UNIT
                    }
                    foreach (Edge geomEdge in geomSolid.Edges)
                        edges++;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("this wall conatins ").Append(edges).Append(" edges and ").Append(faces);
                    sb.Append(" faces, the total area is ").Append(totalArea.ToString("f2")).AppendLine("square meter");
                    // Note: ToString("f2") will erase the numbers after the third decimal place
                    MessageBox.Show(sb.ToString());
                }
            }
        }

    }//end class

}

