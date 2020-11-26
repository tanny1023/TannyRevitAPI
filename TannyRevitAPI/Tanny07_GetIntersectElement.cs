using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Transaction(TransactionMode.Manual)]
    class Tanny07_GetIntersectElement : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ICollection<ElementId> selectedElements = uidoc.Selection.GetElementIds();
            MessageBox.Show("You have selected total " + selectedElements.Count + " elements.");

            Element ele;
            IList<Element> wallList=null;

            foreach (ElementId id in selectedElements)
            {
                ele = doc.GetElement(id);
                wallList= GetBoundingBoxIntersectsElements(ele, "floor");
                elements.Insert(ele);
                MessageBox.Show("數回傳有碰到ele的牆數量 " + wallList.Count + " elements.");
            }
            foreach (Element element in wallList)
            {
                elements.Insert(element);
            }
            if (elements.Size == 0)
                return Result.Succeeded;
            else
                message = "亮顯的牆有碰到ele";
                return Result.Failed;
        }
        //回傳有碰到ele的牆或版
        public IList<Element> GetElementIntersectsElements(Element ele, String elementType)
        {
            Document doc = ele.Document;
            //--去除ele本身過濾器
            List<ElementId> excludedElementID = new List<ElementId>();
            excludedElementID.Add(ele.Id);
            ExclusionFilter excludedFilter = new ExclusionFilter(excludedElementID);
            //--圖元相交過濾器
            ElementIntersectsElementFilter intersectFilter = new ElementIntersectsElementFilter(ele);
            //--建立圖元過濾器
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            switch (elementType)
            {
                case "floor":
                    collector.OfClass(typeof(Floor)).WhereElementIsNotElementType().WherePasses(excludedFilter).WherePasses(intersectFilter);
                    break;
                case "wall":
                    collector.OfClass(typeof(Wall)).WhereElementIsNotElementType().WherePasses(intersectFilter);
                    break;
                default:
                    MessageBox.Show("依照設定參數回傳有碰到ele的牆或版錯誤", "錯誤訊息");
                    break;
            }

            IList<Element> intersectElement = collector.ToElements();
            return intersectElement;
        }
        //回傳有碰到ele的BoundingBox的牆或版
        public IList<Element> GetBoundingBoxIntersectsElements(Element ele, String elementType)
        {
            Document doc = ele.Document;
            //--去除ele本身過濾器
            List<ElementId> excludedElementID = new List<ElementId>();
            excludedElementID.Add(ele.Id);
            ExclusionFilter excludedFilter = new ExclusionFilter(excludedElementID);
            //--邊界框相交過濾器
            BoundingBoxXYZ boundingBoxOfEle = ele.get_BoundingBox(doc.ActiveView);
            Outline outline = ExtendBoundingBox(boundingBoxOfEle);
            BoundingBoxIntersectsFilter intersectFilter = new BoundingBoxIntersectsFilter(outline);

            //--建立圖元過濾器
            FilteredElementCollector collector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            switch (elementType)
            {
                case "floor":
                    collector.OfClass(typeof(Floor)).WhereElementIsNotElementType().WherePasses(excludedFilter).WherePasses(intersectFilter);
                    break;
                case "wall":
                    collector.OfClass(typeof(Wall)).WhereElementIsNotElementType().WherePasses(intersectFilter);
                    break;
                default:
                    MessageBox.Show("依照設定參數回傳有碰到ele的牆或版錯誤", "錯誤訊息");
                    break;
            }

            IList<Element> intersectElement = collector.ToElements();
            return intersectElement;
        }
        //擴大BoundingBox
        public Outline ExtendBoundingBox(BoundingBoxXYZ boundingBox)
        {
            Transform trans = boundingBox.Transform;
            XYZ boundingBoxMin = boundingBox.Min;
            XYZ boundingBoxMax = boundingBox.Max;
            XYZ victorBoundingBox = boundingBoxMax - boundingBoxMin;
            victorBoundingBox = victorBoundingBox.Normalize();

            boundingBoxMin -= victorBoundingBox;
            boundingBoxMax += victorBoundingBox;
            //定界框
            Outline outLine = new Outline(trans.OfPoint(boundingBoxMin), trans.OfPoint(boundingBoxMax));
            return outLine;
        }
    }
}

