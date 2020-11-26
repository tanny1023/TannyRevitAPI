using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.IO; 

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class D001_SaveText : IExternalCommand
    {
        /// <summary>
        /// This example write the base constraint, the volumn, the area, and the length of walls to a text file
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            ICollection<ElementId> ids = uidoc.Selection.GetElementIds();
            Document doc = uidoc.Document;

            string modelPath = doc.PathName;
            string modelTitle = doc.Title;
            StreamWriter sw = new StreamWriter(modelPath.Replace(doc.Title + ".rvt", "Result.txt"));
            sw.WriteLine("BaseConstraint\tVolume\tArea\tLength");

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
                    sw.WriteLine(condition + "\t" + volume + "\t" + area + "\t" + length);
                }
            }
            sw.Close();
            MessageBox.Show("Successfully write the information of walls to a text file!");
            return Result.Succeeded;
        }

        private double findVolume(Element elem)
        {
            double volume = 0.0;
            //volume = elem.LookupParameter("體積").AsDouble();
            volume = elem.LookupParameter("Volume").AsDouble();
            return volume;
        }

        private string findCondition(Element elem, Document doc)
        {
            string name = null;
            //name = doc.GetElement(elem.LookupParameter("底部約束").AsElementId()).Name;
            name = doc.GetElement(elem.LookupParameter("Base Constraint").AsElementId()).Name;
            return name;
        }

        private double findArea(Element elem)
        {
            double area = 0.0;
            //area = elem.LookupParameter("面積").AsDouble();
            area = elem.LookupParameter("Area").AsDouble();
            return area;
        }

        private double findLength(Element elem)
        {
            double length = 0.0;
            //length = elem.LookupParameter("長度").AsDouble();
            length = elem.LookupParameter("Length").AsDouble();
            return length;
        }
    }
}
