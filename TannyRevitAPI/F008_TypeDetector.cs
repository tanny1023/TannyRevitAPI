using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class F008_TypeDetector : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();

            foreach (ElementId eid in ids)
            {
                Element elem = doc.GetElement(eid);
                CeilingAndFloor slab = elem as CeilingAndFloor;
                Floor floor = elem as Floor;
                if (slab != null)
                    MessageBox.Show("slab is ceiling and floor!");
                if (floor != null)
                {
                    StringBuilder sb = new StringBuilder();
                    MessageBox.Show("slab is floor!");
                    SlabShapeEditor sbe = floor.SlabShapeEditor;
                    SlabShapeVertexArray sbArray = sbe.SlabShapeVertices;
                    foreach (SlabShapeVertex vertex in sbArray)
                    {
                        sb.AppendLine(vertex.Position.ToString());
                    }

                    MessageBox.Show(sb.ToString());
                }
            }
            return Result.Succeeded;
        }
    }
}
