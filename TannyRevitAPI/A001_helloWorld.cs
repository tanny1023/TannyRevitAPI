using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
  
    public class A001_HelloWorld : IExternalCommand
    {
        /// <summary>
        /// This clss shows how to use message box to show the hello world message
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            MessageBox.Show("Hello, World");
            return Result.Succeeded;
        }
    }
}
