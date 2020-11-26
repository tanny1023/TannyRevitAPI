using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class paintSelectedItems : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {

            //Console.WriteLine("針對特定的ID及參數名稱插入參數值..");
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            //如果有上一行就表示元件有經過選擇，否則就是視同全選
            Autodesk.Revit.DB.Document doc = uidoc.Document;
            //Revit 2017一定要用Document來抓Element，不再有SelElementSet類別

            Application.EnableVisualStyles();
            Application.Run(new materialSelector(doc, selElement));

            return Result.Succeeded;
        }//end fun

        //以下是舊版主控台範例程式碼，所以把它封存起來
        #region prototype
        public void GetMaterial(Document document, Wall wall)
        {
            foreach (Parameter parameter in wall.Parameters)
            {
                Definition definition = parameter.Definition;
                // material is stored as element id
                if (parameter.StorageType == StorageType.ElementId)
                {
                    if (definition.ParameterGroup == BuiltInParameterGroup.PG_MATERIALS &&
                            definition.ParameterType == ParameterType.Material)
                    {
                        Autodesk.Revit.DB.Material material = null;
                        Autodesk.Revit.DB.ElementId materialId = parameter.AsElementId();
                        if (-1 == materialId.IntegerValue)
                        {
                            //Invalid ElementId, assume the material is "By Category"
                            if (null != wall.Category)
                            {
                                material = wall.Category.Material;
                            }
                        }
                        else
                        {
                            material = document.GetElement(materialId) as Material;
                        }

                        Console.WriteLine("Revit", "Element material: " + material.Name + "\t" + material.Id);
                        //TaskDialog.Show("Revit", "Element material: " + material.Name);
                        break;
                    }
                }
            }
        }//end fun


        public void PaintElementFace(Element elem, ElementId matId)
        {
            Document doc = elem.Document;
            GeometryElement geometryElement = elem.get_Geometry(new Options());
            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is Solid)
                {
                    Solid solid = geometryObject as Solid;
                    foreach (Face face in solid.Faces)
                    {
                        if (doc.IsPainted(elem.Id, face) == false)
                        {
                            doc.Paint(elem.Id, face, matId);
                        }
                    }
                }
            }
        }//end fun



        public void PaintWallFaces(Wall wall, ElementId matId)
        {
            Document doc = wall.Document;
            GeometryElement geometryElement = wall.get_Geometry(new Options());
            foreach (GeometryObject geometryObject in geometryElement)
            {
                if (geometryObject is Solid)
                {
                    Solid solid = geometryObject as Solid;
                    foreach (Face face in solid.Faces)
                    {
                        if (doc.IsPainted(wall.Id, face) == false)
                        {
                            doc.Paint(wall.Id, face, matId);
                        }
                    }
                }
            }
        }//end fun
        #endregion

    };
}