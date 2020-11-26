using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class setParameters : IExternalCommand
    {
        //以下呼叫兩個Win32API裡的現成函式，分別是開啟主控台與關閉主控台
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("Kernel32")]
        public static extern void FreeConsole();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            AllocConsole();//開啟主控台

            Console.WriteLine("針對特定的ID及參數名稱插入參數值..");
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            Autodesk.Revit.DB.Document doc = uidoc.Document; 
            //Revit 2017一定要用Document來抓Element，不再有SelElementSet類別

            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("238494", "紀乃文到此一遊，北面牆");
            dic.Add("238695", "紀乃文到此一遊，東面牆");
            dic.Add("238855", "紀乃文到此一遊，西面牆");
            dic.Add("238993", "紀乃文到此一遊，南面牆");

            Transaction trans2 = new Transaction(doc);
            trans2.Start("設定參數");//裡面這個字串不能省不然會報錯


            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element ele = doc.GetElement(eleId);
                Console.WriteLine(eleId.ToString() + "\t" + ele.Name);
                this.setMark(dic, eleId, doc);

            }
            trans2.Commit();

            Console.Read();
            FreeConsole(); //和Revit配合時，一定要用程式碼將主控台關掉，如果直接把主控台視窗按掉的話，會連Revit一起關掉
            return Result.Succeeded;
        }

        public void setMark(Dictionary<string, string> dic, ElementId eleId, Document doc)
        {
            Element ele = doc.GetElement(eleId);

            foreach (Parameter para in ele.Parameters)
            {
                //string defName = "牆上塗鴉";
                if (para.Definition.Name == "牆上塗鴉")
                {
                    try
                    {
                        para.Set(dic[eleId.ToString()]);
                        Console.WriteLine(ele.UniqueId + "\t" + dic[eleId.ToString()]);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }

                }//outer if
            }

        }//end fun
    };
}
    