using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class F004_AutomaticPaintBasic : IExternalCommand
    {
        /// <summary>
        /// This example paint all the selected element by a specified material
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
            trans.Start("automatic paint");
            //filter out all of the materails
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Materials);
            IList<Element> matList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            int materialID = 0; //we need to record the material ID
            foreach (Element ele in matList)
            {
                if (ele.Name == "00二丁掛")
                    materialID = Convert.ToInt32(ele.Id.IntegerValue); //element id is represented by an integer
            }

            //for all the selected elements, paint evevry face by the specified material
            foreach (ElementId eleId in ids)
            {
                Element ele = doc.GetElement(eleId);
                ElementId eid = new ElementId(materialID);
                PaintElementFace(ele, eid); //call the mathod for paint an element
            }
            trans.Commit();
            return Result.Succeeded;
        }

        /// <summary>
        /// This method paint an element by a specified material
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="matId"></param>
        public void PaintElementFace(Element elem, ElementId matId)
        {
            GeometryElement geometryElement = elem.get_Geometry(new Options());
            //get the geomertry objects from the elements 
            foreach (GeometryObject geometryObject in geometryElement)
            {
                Solid solid = geometryObject as Solid; 
                if (solid!=null) //if the geomertry object is a solid
                {
                    foreach (Face face in solid.Faces) //for each face on the solid
                       elem.Document.Paint(elem.Id, face, matId); //paint the face
                }
            }
        }

    };
}
