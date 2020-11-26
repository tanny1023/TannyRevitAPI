using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPIDemo
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    class TANNY03_WallDIm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Reference obj = sel.PickObject(ObjectType.Element);
            Wall wall = doc.GetElement(obj.ElementId) as Wall;
            Transaction trans = new Transaction(doc);
            trans.Start("建立尺寸");
            if (wall != null)
            {
                Options opt = new Options();
                opt.ComputeReferences = true; //打開計算幾何應用
                opt.DetailLevel = ViewDetailLevel.Medium;//詳細程度
                GeometryElement geoEle = wall.get_Geometry(opt);
                foreach (GeometryObject geoObj in geoEle)
                {
                    Solid solid = geoObj as Solid;
                    Transform instanceTransform = Transform.Identity;
                    CreateNewDimensionAlongLine(doc, solid, instanceTransform);
                }
            }
            trans.Commit();
            return Result.Succeeded;
        }
        //--建立尺寸
        public void CreateNewDimensionAlongLine(Document doc, Solid solid, Transform instanceTransform)
        {
            try
            {
                foreach (Face f in solid.Faces)
                {
                    bool result = f.ComputeNormal(new UV(0.5, 0.5)).IsAlmostEqualTo(XYZ.BasisZ.Negate());
                    if (result) //get bottom face
                    {
                        //Transform instanceTransform = Transform.Identity;
                        foreach (EdgeArray edgeArr in f.EdgeLoops)
                        {
                            for (int i = 0; i < 2; i++)
                            {
                                Edge edge = edgeArr.get_Item(i);
                                ReferenceArray refArray = new ReferenceArray();
                                refArray.Append(edge.GetEndPointReference(0));
                                refArray.Append(edge.GetEndPointReference(1));
                                XYZ offsetVec = instanceTransform.OfVector(edge.Evaluate(0.5)).Normalize(); // 取得邊的法向量(normal vector)
                                XYZ p1 = instanceTransform.OfPoint(edge.Evaluate(0)) + (-250)*offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)
                                XYZ p2 = instanceTransform.OfPoint(edge.Evaluate(1)) + (-250)*offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)
                                Line line = Line.CreateBound(p1, p2);
                                doc.Create.NewDimension(doc.ActiveView, line, refArray);
                            }
                        }
                    }
                }
            }

            catch
            {

                MessageBox.Show("錯誤");
            }
        }
    }
}

