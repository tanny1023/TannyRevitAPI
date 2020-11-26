using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs03_HelloWorld : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MessageBox.Show("Hello World");
            return Result.Succeeded;
        }
    }
}