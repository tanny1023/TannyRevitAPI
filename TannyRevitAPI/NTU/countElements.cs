
//引進了RevitAPI.dll及RevitUIAPI.dll以後，要加入以下兩行
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
//為了用WindowsForm在畫面上輸出執行內容，需要加入以下一行
using System.Windows.Forms;
//使用數量計算的API需要用到選取範圍的命名空間
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace RevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class myRevitApiDemo_Counter : IExternalCommand
    {
        //這個API計算已經被選取的元件共有多少個，不分類別
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument myUidoc = commandData.Application.ActiveUIDocument;
            //UIDocument這個物件會儲存使用者「正開啟的模型」
            ICollection<ElementId> selectedElements = myUidoc.Selection.GetElementIds();
            //SelElementSet允許使用者做一個「框選」的前置動作，我們通常不會盲目的關心「整個模型的元件數量」，
            //而會只框選某個樓層的某一部份，因此針對這個範圍再做數量計算，會是較合理的操作
            //如果使用者真的在關心整個模型的話，「全選」也是一個可以輕易做到的事

            int totalCount = selectedElements.Count;
            //既然已知我們框選的範圍的所有元件會被放在SelElementSet，我們只要取出它的「數量」屬性就能做數量計算了
            //但這就是一個基礎的數量計算，無法區分梁柱牆板

            MessageBox.Show("被選取的總共有 " + totalCount.ToString() + " 個元件");

            return Result.Succeeded;
        }//end fun

    } //end class
}
