using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using Autodesk.Revit.UI.Selection;
using System.Text;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class E007_FindSlabLocation : IExternalCommand
    {
        /// <summary>
        /// This example get the locations of each edges of a slab
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction TemporaryTransaction1 = new Transaction(doc);
            TemporaryTransaction1.Start("get slab curve");
            //let user select a single slab, use the pick object method
            Reference r = uidoc.Selection.PickObject(ObjectType.Element);
            ElementId eid = r.ElementId; //get the element id
            //important: when we delete an element, it will return a collection to represent what we have delete
            //if we delete a slab, this collection will be the element ids of the model lines (slab edges)
            ICollection<ElementId> eids = doc.Delete(eid);
            TemporaryTransaction1.RollBack();
            //we delete the slab only for getting all the model lines, therefore we cancel the transaction   

            StringBuilder sb = new StringBuilder();
            //add those model lines to the list
            foreach (ElementId MlineID in eids)
            {
                Element elem = doc.GetElement(MlineID);
                ModelLine mLine = elem as ModelLine;
                if (mLine!=null)
                {
                    sb.AppendLine(mLine.Name);
                    LocationCurve curve = mLine.Location as LocationCurve;
                    Line line = curve.Curve as Line;
                    XYZ p1 = line.GetEndPoint(0);
                    XYZ p2 = line.GetEndPoint(1);
                    sb.AppendLine(p1.ToString());
                    sb.AppendLine(p2.ToString());
                }
            }
            MessageBox.Show(sb.ToString());
            return Result.Succeeded;
        }
    };
}