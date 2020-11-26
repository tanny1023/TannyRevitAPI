
//引進了RevitAPI.dll及RevitUIAPI.dll以後，要加入以下兩行
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
//為了用WindowsForm在畫面上輸出執行內容，需要加入以下一行
using System.Windows.Forms;

namespace RevitAPIDemo
{
   [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    //宣告一個類別，繼承IExternalCommand介面，這個類別的名稱可以由自己決定，因為會顯示在Revit上頭，
    //名稱一定要能反應它的功能，不然將來會看不懂（不要取demo01,class01這類的「菜市場名」）
    public class myRevitApiDemo_HelloWorld : IExternalCommand 
    {
        //繼承IExternalCommand介面的類別，一定要有Execute這個方法，參數類型和方法名稱都不能隨意改動
       public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
       {
           //在此開始編寫程式
           MessageBox.Show("Hello, world!");
           //編寫到此為止
           return Result.Succeeded;
       }//end fun
    } //end class

}//end namespace

