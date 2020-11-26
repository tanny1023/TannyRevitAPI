using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class testConsole : IExternalCommand
    {
       
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        public static extern void FreeConsole();

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            AllocConsole();
            Console.WriteLine("Hello, world!");
            Console.Read();
            FreeConsole();
            return Result.Succeeded;
        }

        private void demoMessageBox()
        {
            for (int i = 0; i != 1000; i++)
            {
                //這是錯誤示範！
                MessageBox.Show("Hello, world!");
            }

            string x = "";
            for (int i = 0; i != 1000; i++)
            {
                x += "Hello, world!";
                x += "\n"; //換行符號
            }
            MessageBox.Show(x);
        }//end fun

    };


}
