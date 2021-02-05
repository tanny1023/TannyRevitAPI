using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Transaction(TransactionMode.Manual)]
    class TANNY04_GridDim : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //選擇網格線
            IList<Reference> grids = uidoc.Selection.PickObjects(ObjectType.Element);
            // 選擇柱子
            IList<Reference> columns = uidoc.Selection.PickObjects(ObjectType.Element);
            //寫入revit
            Transaction trans = new Transaction(doc);
            trans.Start("建立尺寸");
            foreach (Reference grid in grids)
            {
                Grid g = doc.GetElement(grid) as Grid;
                Curve gCurve = g.Curve;
                Transform t = Transform.CreateRotation(XYZ.BasisZ, Math.PI / 2);
                Curve gdCurve = gCurve.CreateTransformed(t);
                foreach (Reference obj in columns)
                {
                    ReferenceArray refArray = new ReferenceArray();
                    refArray.Append(grid);
                    FamilyInstance e = doc.GetElement(obj.ElementId) as FamilyInstance;
                    //讀取柱的實體(Solid)資訊，找出底部的面(Face)
                    Options opt = new Options();
                    opt.ComputeReferences = true; //打開計算幾何應用
                    opt.DetailLevel = ViewDetailLevel.Medium;//詳細程度
                    GeometryElement geoEle = e.get_Geometry(opt);
                    if (geoEle == null)
                    {
                        MessageBox.Show("geoEle=NULL");
                    }
                    //從GeometryElement 獲得GemotryObject再轉化為Solid
                    foreach (GeometryObject geoObj in geoEle)
                    {

                        GeometryInstance geoInstance = geoObj as GeometryInstance;
                        if (geoInstance != null)
                        {
                            Transform instanceTransform = geoInstance.Transform;
                            foreach (GeometryObject o in geoInstance.SymbolGeometry)
                            {
                                Solid solid = o as Solid;
                                if (solid == null)
                                {
                                    MessageBox.Show("solid=null");
                                }
                                else
                                {
                                    //--建立尺寸
                                    CreateNewDimensionAlongLine(doc, solid, instanceTransform,ref refArray);
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("geoInstance=null");
                        }
                    }

                }
            }

            trans.Commit();
            return Result.Succeeded;
        }
        //--建立尺寸
        public void CreateNewDimensionAlongLine(Document doc, Solid solid, Transform instanceTransform,ref ReferenceArray refArray)
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
                            for (int i = 0; i < edgeArr.Size; i++)
                            {
                                Edge edge = edgeArr.get_Item(i);
                                
                                refArray.Append(edge.GetEndPointReference(0));
                                refArray.Append(edge.GetEndPointReference(1));
                                XYZ offsetVec = instanceTransform.OfVector(edge.Evaluate(0.5)).Normalize(); // 取得邊的法向量(normal vector)
                                XYZ p1 = instanceTransform.OfPoint(edge.Evaluate(0)) + 1000 * offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)
                                XYZ p2 = instanceTransform.OfPoint(edge.Evaluate(1)) + 1000 * offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)

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
