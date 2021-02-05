using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Tanny02_ColumnDim : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiApp = commandData.Application;
            Document doc = uiApp.ActiveUIDocument.Document;
            Selection sel = uiApp.ActiveUIDocument.Selection;
            //选择需要标注尺寸的图元
            Reference obj = sel.PickObject(ObjectType.Element);
            //取得其中一个图元 获取其位置
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
            //寫入revit
            Transaction trans = new Transaction(doc);
            trans.Start("建立尺寸");
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
                            CreateNewDimensionAlongLine(doc, solid, instanceTransform);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("geoInstance=null");
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
                                XYZ p1 = instanceTransform.OfPoint(edge.Evaluate(0)) + -50 * offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)
                                XYZ p2 = instanceTransform.OfPoint(edge.Evaluate(1)) + -50 * offsetVec; // 將點座標依照轉換為全域坐標系(Local transform to global transform)

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

