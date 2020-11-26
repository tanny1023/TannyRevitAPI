using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;


namespace RevitAPIDemo
{
     [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class myRevitApiDemo_SetAttributes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selElement = uidoc.Selection;
            //以上「選取特定元件」的操作皆同，不再贅述

            Transaction trans = new Transaction(doc);
            trans.Start("設定參數");
            //在Revit API當中，如果會修改模型本身的操作，必須寫在Transaction當中
            //讓它位於Start及Commit的程式區段之間

            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element elem = doc.GetElement(eleId);

                foreach (Parameter para in elem.Parameters)//針對每個元件的所有參數
                { 
                   if(para.Definition.Name=="備註")
                       para.Set("test");
                }//end inner foreach
            }//end outer foreach

            trans.Commit();//結束修改模型操作
            MessageBox.Show("已將指定字串寫入屬性當中");//提示API操作已結束
            return Result.Succeeded;
        }

    };
}
