using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.IO; //要存取File相關資訊需要引用此參考
using System.Text; //要使用字元編碼(Encoding)需要引用此參考

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Cs10_SaveFile : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            System.IO.FileStream file = new FileStream("FileOut.txt", FileMode.Create, FileAccess.ReadWrite);
            System.IO.StreamWriter sw = new System.IO.StreamWriter(file, Encoding.Unicode);
            sw.WriteLine("底部約束\t體積\t面積\t長度");

            double totalVolume = 0;
            double totalArea = 0;
            double totalLength = 0;

            foreach (ElementId elemId in ids)
            {
                Element elem = doc.GetElement(elemId);
                Wall wall = elem as Wall;
                if (wall != null)
                {
                    string condition = findCondition(wall, doc);
                    double volume = findVolume(wall);
                    double area = findArea(wall);
                    double length = findLength(wall);
                    totalVolume += volume;
                    totalArea += area;
                    totalLength += length;
                    sw.WriteLine(condition + "\t" + volume + "\t" + area + "\t" + length);
                }
            }
            sw.WriteLine("\n總和:\t" + totalVolume + "\t" + totalArea + "\t" + totalLength);
            sw.Close();
            MessageBox.Show("牆的參數資訊寫入完成！");
            return Result.Succeeded;
        }
        private double findVolume(Element elem)
        {
            double volume = 0.0;
            volume = elem.LookupParameter("體積").AsDouble();
            return volume;
        }

        private string findCondition(Element elem, Document doc)
        {
            string name = null;
            name = doc.GetElement(elem.LookupParameter("底部約束").AsElementId()).Name;
            return name;
        }

        private double findArea(Element elem)
        {
            double area = 0.0;
            area = elem.LookupParameter("面積").AsDouble();
            return area;
        }

        private double findLength(Element elem)
        {
            double length = 0.0;
            length = elem.LookupParameter("長度").AsDouble();
            return length;
        }
    }
}
