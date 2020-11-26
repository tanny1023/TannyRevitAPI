using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;


namespace RevitAPIDemo
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class myRevitApiDemo_GetAttributes : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //用篩選器取得模型當中的所有牆
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> wallList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            StringBuilder sb = new StringBuilder(); //用一個字串建立器取得最後要秀出的成果
            foreach (Element wall in wallList)
            {
                foreach (Parameter para in wall.Parameters)
                {
                    sb.Append(this.getParameterInfomation(para, doc, ":"));
                    sb.Append("\t");
                }
                sb.AppendLine("============================"); //每讀一道牆加一條分隔線
            }
            MessageBox.Show(sb.ToString()); //列印結果
            return Result.Succeeded;
        }

        //這個函式的原型公佈在官網上，我做了一點點修改並且加上註解
        //https://knowledge.autodesk.com/search-result/caas/CloudHelp/cloudhelp/2017/ENU/Revit-API/files/GUID-D003803E-9FA0-4B2B-AB62-7DCC3A503644-htm.html
        private string getParameterInfomation(Parameter para, Document doc, string seperator)
        {
            string defName = para.Definition.Name;//這是欄位名稱，而欄位值是比較麻煩的
            //StorageType是Revit為了描述欄位變數型態而定義出來的enum
            switch (para.StorageType) //欄位值比需要區分變數類型來討論，設計得很不親切
            {
                case StorageType.Double: //如果這個欄位的型態是double
                    return defName + seperator + para.AsValueString();

                case StorageType.ElementId:
                    //如果欄位型態是ElementID識別碼，要分正整數及負整數討論
                    //因為是官網公佈應該這麼做的，因此從善如流
                    ElementId id = para.AsElementId();
                    if (id.IntegerValue >= 0)
                        return defName + seperator + doc.GetElement(id).Name;
                    else
                        return defName + seperator + id.IntegerValue.ToString();

                case StorageType.Integer: //如果欄位是整數，它還有可能是布林值的零或一，要再寫一個if-else
                    if (ParameterType.YesNo == para.Definition.ParameterType)//如果欄位是布林值
                    {
                        if (para.AsInteger() == 0)
                            return defName + seperator + "False";
                        else //para.AsInteger() == 1
                            return defName + seperator + "True";
                    }
                    else //如果欄位變數型態真的是整數，而非布林值
                    {
                        return defName + seperator + para.AsInteger().ToString();
                    }

                case StorageType.String: //如果是字串則最好處理
                    return defName + seperator + para.AsString();

                default:
                    return "未公開的參數";
            }
        }//end fun



    };
}
