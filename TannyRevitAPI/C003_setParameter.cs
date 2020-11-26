using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Windows.Forms;

namespace revitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class C003_SetParameters : IExternalCommand
    {
        /// <summary>
        /// This example set values to the parameter we created in the previous unit
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            Autodesk.Revit.DB.Document doc = uidoc.Document;

            //this is the first time to use transaction object, becasue we want to modfify the BIM model
            Transaction trans2 = new Transaction(doc);
            trans2.Start("set values for parameters"); //you must give a name to the transaction, cannot be ignored

            int k = 1;
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele = doc.GetElement(eleId);
                this.setMark(k, eleId, doc);
                k++;
            }
            trans2.Commit();
            MessageBox.Show("set values to parameters successfully!");
            return Result.Succeeded;
        }

        /// <summary>
        /// This method set a specific message to the value of the parameter "Note"
        /// </summary>
        /// <param name="k"></param>
        /// <param name="eleId"></param>
        /// <param name="doc"></param>
        public void setMark(int k, ElementId eleId, Document doc)
        {
            string message = "This is the " + k + "th item we have selected.";
            Element ele = doc.GetElement(eleId);

            foreach (Parameter para in ele.Parameters)
            {
                if (para.Definition.Name == "Note")
                {
                    try
                    {
                        para.Set(message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"例外狀況:{ex}");
                    }

                }//outer if
            }//end foreach

        }//end fun
    }
}
