using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Transaction(TransactionMode.Manual)]
    class Tanny06_AnalyticalWallCurve : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ElementCategoryFilter filter_wall = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> wallList = collector.WherePasses(filter_wall).WhereElementIsNotElementType().ToElements();

            using (Transaction trans = new Transaction(doc, "啟用分析模型"))
            {
                trans.Start();
                foreach (Element element in wallList)
                {
                    Wall wall = element as Wall;
                    OpenWallStructuralSignificant(wall);
                }
                trans.Commit();
            }
            MessageBox.Show("You have selected total " + wallList.Count + " elements.");
            return Result.Succeeded;
        }
        //--啟用牆的分析模型
        public void OpenWallStructuralSignificant(Wall wall)
        {
            try
            {
                wall.get_Parameter(BuiltInParameter.WALL_STRUCTURAL_SIGNIFICANT).Set(1);
                wall.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(1);
                //--修正分析牆對齊方式
                AnalyticalModelSurface analyticalModelSurface = wall.GetAnalyticalModel() as AnalyticalModelSurface;
                if (analyticalModelSurface!=null)
                {
                    //--對齊方式改為投影
                    analyticalModelSurface.AlignmentMethod = AnalyticalAlignmentMethod.Projection;
                    //--Z方向投影改為元素的中心
                    analyticalModelSurface.ProjectionZ = SurfaceElementProjectionZ.CenterOfElement;
                    //--修正分析牆延伸方法投影
                    analyticalModelSurface.TopExtensionMethod = AnalyticalAlignmentMethod.Projection;
                    analyticalModelSurface.BottomExtensionMethod = AnalyticalAlignmentMethod.Projection; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"啟用牆的分析物件發生錯誤\n{ex}", "錯誤訊息");
                throw;
            }
        }
    }
}

