using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class printIDbyConsole : IExternalCommand
    {
        //以下呼叫兩個Win32API裡的現成函式，分別是開啟主控台與關閉主控台
        [DllImport("kernel32.dll", SetLastError = true)] 
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole(); //呼叫主控台

        [DllImport("kernel32.dll")]
        public static extern void FreeConsole();//關閉主控台

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AllocConsole();//開啟主控台
            Console.WriteLine("以下列印出所有選取元素的ID..");
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            //UIDocument代表「現在正開啟的模型」
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            //Selection代表「使用者用滑鼠左鍵按住拖曳所選取的元件」
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            //Document類別可以用來存放模型的全部或是一部份，從uidoc存取Document屬性，
            //就會把整個模型放進Document類別當中

            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele = doc.GetElement(eleId); //以ElementId在document當中找到對應的Element元件
                //Document docSingle = ele.Document; //單一的元素也有Document屬性，但在這裡我們還不會用上
                Console.WriteLine(eleId.ToString()+"\t"+ele.UniqueId+"\t"+ele.Name);
            }

            Console.Read(); //如果我們想要在螢幕上看到主控台的執行成果，要用Console.Read()叫它等一下，否則它會「一閃即逝」
            FreeConsole(); //和Revit配合時，一定要用程式碼將主控台關掉，如果直接把主控台視窗按掉的話，會連Revit一起關掉
            return Result.Succeeded;
        }
    };
}
