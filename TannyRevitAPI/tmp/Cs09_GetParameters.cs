using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Text;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs09_GetParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            StringBuilder st = new StringBuilder();

            Element selectedElem = null;
            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                selectedElem = elem;
                break;
            }

            foreach (Parameter para in selectedElem.Parameters)
                st.AppendLine(getParameterInformation(para, doc));

            MessageBox.Show(st.ToString(), "Revit", MessageBoxButtons.OK);
            return Result.Succeeded;
        }
        private string getParameterInformation(Parameter para, Document document)
        {
            string defName = para.Definition.Name;
            switch (para.StorageType)
            {
                case StorageType.Double:
                    return defName + ":" + para.AsValueString();

                case StorageType.ElementId:
                    ElementId id = para.AsElementId();
                    if (id.IntegerValue >= 0)
                        return defName + ":" + document.GetElement(id).Name;
                    // 在2014以前取得元件的方法為get_Element()，而在2014方法更新為GetElement()
                    else
                        return defName + ":" + id.IntegerValue.ToString();

                case StorageType.Integer:
                    if (ParameterType.YesNo == para.Definition.ParameterType)
                    {
                        if (para.AsInteger() == 0)
                            return defName + ":" + "False";
                        else
                            return defName + ":" + "True";
                    }
                    else
                    {
                        return defName + ":" + para.AsInteger().ToString();
                    }

                case StorageType.String:
                    return defName + ":" + para.AsString();

                default:
                    return "未公開的參數";
            }
        }
    }
}