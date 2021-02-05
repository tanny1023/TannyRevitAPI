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
    class Tanny14_OpenFloorAnalytycalModel : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            ElementCategoryFilter filter_wall = new ElementCategoryFilter(BuiltInCategory.OST_Floors);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            IList<Element> floorList = collector.WherePasses(filter_wall).WhereElementIsNotElementType().ToElements();
            using (Transaction trans = new Transaction(doc, "啟用分析模型"))
            {
                trans.Start();
                foreach (Element element in floorList)
                {
                    Floor floor = element as Floor;
                    if (floor != null)
                        OpenFloorStructuralSignificant(floor);
                }
                trans.Commit();

            }

            foreach (Element element in floorList)
            {
                Floor floor = element as Floor;
                if (floor != null)
                {

                    IList<CurveLoop> list = GetExternalBoundary(floor);
                    MessageBox.Show($"list={list.Count}");
                }
            }

            return Result.Succeeded;
        }
        //--開啟板的分析模型
        public void OpenFloorStructuralSignificant(Floor floor)
        {
            try
            {
                if (floor != null)
                {
                    floor.get_Parameter(BuiltInParameter.FLOOR_PARAM_IS_STRUCTURAL).Set(1);
                    floor.get_Parameter(BuiltInParameter.STRUCTURAL_ANALYTICAL_MODEL).Set(1);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"啟用板的分析物件發生錯誤\n{ex}", "錯誤訊息");
                throw;
            }

        }
        //--取出分析模型的外側邊界
        public IList<CurveLoop> GetExternalBoundary(Floor floor)
        {
            //--得到分析模型的表面
            AnalyticalModelSurface analyticalModelSurface = floor.GetAnalyticalModel() as AnalyticalModelSurface;
            //--得到分析模型表面的IList<CurveLoop>
            IList<CurveLoop> analyticalFloorLoops = analyticalModelSurface.GetLoops(AnalyticalLoopType.External);
            return analyticalFloorLoops;
        }
        //--取出分析模型的內側邊界
        public IList<CurveLoop> GetInternalBoundary(Floor floor)
        {
            //--得到分析模型的表面
            AnalyticalModelSurface analyticalModelSurface = floor.GetAnalyticalModel() as AnalyticalModelSurface;
            //--得到分析模型表面的IList<CurveLoop>
            IList<CurveLoop> analyticalFloorLoops = analyticalModelSurface.GetLoops(AnalyticalLoopType.Internal);
            return analyticalFloorLoops;
        }
    }
}
