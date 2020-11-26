using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;


namespace RevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class myRevitApiDemo_calculateAndSaveWallInformation : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection selElement = uidoc.Selection;
            
            //準備檔案寫入
            StreamWriter sw= new StreamWriter(Directory.GetCurrentDirectory() + "//output.txt");
            sw.WriteLine("牆底部約束\t牆長度\t牆投影面積\t牆體積");
            //宣告三個變數，記錄牆的總長度及總體積
            double totalVolume = 0;
            double totalArea = 0;
            double totalLength = 0;

            //針對已經被使用者選取的所有元件
            foreach (ElementId eleId in selElement.GetElementIds())
            {
                Element elem = doc.GetElement(eleId);

                if (elem.GetType().Name=="Wall") //如果目前正在關注的元件類型是「牆」的話
                {                    
                    //用獨立之函式（方法）取得以下四項資訊
                    string condition = this.getWallCondition(elem, doc);//傳回底部約束
                    double volume = this.getWallVolume(elem);//傳回總體積
                    double area = this.getWallArea(elem);//傳回總面積
                    double length = this.getWallLength(elem);//傳回總長度
                    //將上述四項資訊加總於三個總量之中
                    totalVolume += volume;
                    totalArea += area;
                    totalLength += length;
                    //將上述四項資訊記錄於文字檔中
                    sw.WriteLine(condition + "\t" + length + "\t" + area + "\t" + volume);
                }
            }
            //記錄總量
            sw.WriteLine("\n總和:\t" + totalLength + "\t" + totalArea + "\t" + totalVolume);
            sw.Flush();
            sw.Close();//關閉寫入串流

            MessageBox.Show("牆的參數資訊寫入完成！");
            return Result.Succeeded;
        }

       /// <summary>
       /// 傳回牆體積，直接從元件屬性裡撈出來
       /// </summary>
       /// <param name="elem"></param>
       /// <returns></returns>
        private double getWallVolume(Element elem)
        {
            double volume = 0.0;
            foreach (Parameter para in elem.Parameters)
            {
                if (para.Definition.Name == "體積")
                    volume = para.AsDouble();
            }
            return volume;
        }
        /// <summary>
        /// 傳回底部約束，直接從元件屬性裡撈出來
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        private string getWallCondition(Element elem, Document doc)
        {
            string name = "default";
            foreach (Parameter para in elem.Parameters)
            {
                if (para.Definition.Name == "底部約束")
                {
                    ElementId id = para.AsElementId();
                    return doc.GetElement(id).Name;
                }
            }
            return name;
        }

        /// <summary>
        /// 傳回牆面積，直接從元件屬性裡撈出來
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private double getWallArea(Element elem)
        {
            double area = 0.0;

            foreach (Parameter para in elem.Parameters)
            {
                if (para.Definition.Name == "面積")
                    area = para.AsDouble();
            }
            return area;
        }

        /// <summary>
        /// 傳回牆長度，直接從元件屬性裡撈出來
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private double getWallLength(Element elem)
        {
            double length = 0.0;
            foreach (Parameter para in elem.Parameters)
            {
                if (para.Definition.Name == "長度")
                    length = para.AsDouble();
            }
            return length;
        }
    }//end class

}//end namespace
