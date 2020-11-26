using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using Sino_UnderStructure_II;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Tanny13_GetFloorSideFace : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;
            foreach (ElementId id in ids)
            {
                Element elem = doc.GetElement(id);
                Floor floor = elem as Floor;
                List<Face> faceList = ElementMethod.GetSideFace(floor);
                PopupBox.Error($"faceList={faceList.Count}");
            }
            return Result.Succeeded;
        }
       
    }
}
