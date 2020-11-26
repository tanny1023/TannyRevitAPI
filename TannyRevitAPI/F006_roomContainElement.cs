using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI.Selection;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class F006_Room : IExternalCommand
    {
        /// <summary>
        /// This example print the name of the rooms where the selected elements is located at
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Selection selElement = uidoc.Selection;
            Document doc = uidoc.Document;

            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele= doc.GetElement(eleId);
                // Get the group's center point
                XYZ origin = GetElementCenter(ele);
                // Get the room that the picked group is located in
                Room room = GetRoomOfGroup(doc, origin);
                MessageBox.Show("The item you have selected is in the room: " + room.Name);
            }
            return Result.Succeeded;
        }

        /// <summary>
        /// This method get the element center of a bounding box, so it can represent the centroid of an element
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        public XYZ GetElementCenter(Element elem)
        {
            BoundingBoxXYZ bounding = elem.get_BoundingBox(null);
            XYZ center = (bounding.Max + bounding.Min) * 0.5;
            return center;
        }

        /// <summary>
        /// This method get the room where a point is located at
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public Room GetRoomOfGroup(Document doc, XYZ point)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfCategory(BuiltInCategory.OST_Rooms); //find all the rooms in a BIM model
            Room room = null;
            foreach (Element elem in collector)
            {
                room = elem as Room;
                if (room != null)
                {
                    // Decide if this point is in the picked room 
                    if (room.IsPointInRoom(point))
                    {
                        break;
                    }
                }
            }
            return room;
        }

    }//end class

}
