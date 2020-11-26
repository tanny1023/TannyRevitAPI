using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class F003_FindAdjecentElement : IExternalCommand
    {
        /// <summary>
        /// This example find the adjacent elements for each selected element 
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

            //for each selected element, list its adjacent elements
            foreach (ElementId elementId in ids)
            {
                //for a specific element, get the boundingbox
                Element elem = doc.GetElement(elementId);
                BoundingBoxXYZ bbx = elem.get_BoundingBox(null);
                Outline outline = new Outline(bbx.Min, bbx.Max);//get the diagonal of the bounding box
                BoundingBoxIntersectsFilter filterIntersection = new BoundingBoxIntersectsFilter(outline);
                //this filter helps to find all the bounding boxes (as well as the correponding elements) that intersect with the current bounding box
                IList<Element> Intersects =
                 new FilteredElementCollector(doc).WherePasses(filterIntersection)
                   .WhereElementIsNotElementType().ToElements();

                StringBuilder sb = new StringBuilder("This element is a " + elem.Category.Name + ", ").Append("The element id is:" + elem.Id + ", ")
                .Append("the element name is: " + elem.Name + ", ").AppendLine("The adjacent elements are：");
                foreach (Element eleFiltered in Intersects)
                {
                    try
                    {
                        sb.AppendLine(eleFiltered.Category.Name + "\t id:" + eleFiltered.Id + "\t name:" + eleFiltered.Name);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"例外狀況:{ex}");
                    }
                }
                MessageBox.Show(sb.ToString()); //print all of the adjacent elements
            }
            return Result.Succeeded;
        }
    };
}
