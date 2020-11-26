using System;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;
using System.Data;
using System.Reflection;
using System.Data.OleDb;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class integrateParameters : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            
            Application.EnableVisualStyles();
            Application.Run(new parameterManipulator(doc, uidoc)); //呼叫Windows Form使用者介面
            
            return Result.Succeeded;
        }

        public void setNewParametersToCategory(Document doc, UIDocument uidoc, BuiltInCategory category)
        {
            string cateName = category.ToString();
            StreamReader sr = new StreamReader("D:\\revitAPI\\"+cateName+".txt");
            string[] titles = sr.ReadLine().Split('\t');
            List<string> fieldNames = new List<string>();
            for (int i = 1; i != titles.Length; i++)
                fieldNames.Add(titles[i]);

            Transaction trans = new Transaction(doc);
            trans.Start("設定新增參數，綁定於實體之上");
            //必須先新增共用參數檔案
            FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "ShareParameter.txt");
            fileStream.Close();
            //然後將共用參數檔案綁定到Document之上
            doc.Application.SharedParametersFilename = Directory.GetCurrentDirectory() + "ShareParameter.txt";
            DefinitionFile defFile = doc.Application.OpenSharedParameterFile(); //打開共用參數檔
            this.AddNewParametersToCategories(uidoc.Application, defFile, category, fieldNames); //呼叫方法以新增參數
            while (sr.Peek() >= 0) //從文字檔中逐行讀取每個元件的對應參數並且填寫參數值到BIM模型中
            {
                string[] data = sr.ReadLine().Split('\t');
                ElementId eid = new ElementId(Convert.ToInt32(data[0]));
                for (int i = 1; i != data.Length; i++)
                {
                    string columnName = fieldNames[i - 1];
                    string columnValue = data[i];
                    this.setSingleParameter(eid, doc, columnName, columnValue);
                }
                
            }
            sr.Close();            
            trans.Commit();            
        }

        private void setSingleParameter(ElementId eid, Document doc, string colName, string colValue)
        {
            Element ele = doc.GetElement(eid);
            foreach (Parameter para in ele.Parameters)
            {
                if (para.Definition.Name.Equals(colName))
                {
                    try
                    {
                        para.Set(colValue);
                    }
                    catch (Exception ex) { }
                }//end if
            }
        }//end fun



        //這個方法也是被公開在官網上的，我只是小小修改並且翻譯了註解，參考以下網址
        //http://www.revitapidocs.com/2017/f5066ef5-fa12-4cd2-ad0c-ca72ab21e479.htm
        private void AddNewParametersToCategories(UIApplication app, DefinitionFile myDefinitionFile, BuiltInCategory category, List<string> newParameters)
        {
            //在共用參數檔案當中先建立一個新的參數群組
            DefinitionGroups myGroups = myDefinitionFile.Groups;
            DefinitionGroup myGroup = myGroups.Create("使用者定義參數群組");
            // 創立參數定義，含參數名稱及參數型別

            foreach (string paraName in newParameters)
            {
                ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions(paraName, ParameterType.Text);
                Definition myDefinition = myGroup.Definitions.Create(option);
                // 創建一個類別群組，將來在這個類別群組裡的所有類別都會被新增指定的參數
                CategorySet myCategories = app.Application.Create.NewCategorySet();
                // 創建一個類別．以牆為例
                Category myCategory = Category.GetCategory(app.ActiveUIDocument.Document, category);
                myCategories.Insert(myCategory);//把牆類別插入類別群組，當然你可以插入不只一個類別
                //以下兩者是亮點，「類別綁定」與「實體綁定」的結果不同，以本例而言我們需要的其實是實體綁定
                //TypeBinding typeBinding = app.Application.Create.NewTypeBinding(myCategories);
                InstanceBinding instanceBinding = app.Application.Create.NewInstanceBinding(myCategories);
                //取得一個叫作BingdingMap的物件，以進行後續新增參數
                BindingMap bindingMap = app.ActiveUIDocument.Document.ParameterBindings;
                // 最後將新增參數加到群組上頭，並且指定了instanceBiding的方法（也可替換為typeBinding）
                bool typeBindOK = bindingMap.Insert(myDefinition, instanceBinding,
                  BuiltInParameterGroup.PG_TEXT);
              }
        }

        
        /// <summary>
        /// 這是為了作業3增加的函式，隱藏版 2017/12/30
        /// 這個函式會找出所有的門窗類別，將它們的座標抓到access資料庫裡的備註欄位
        /// </summary>
        /// <param name="doc"></param>
        public void hw3_extractPositionStringIntoNotes(Document doc)
        {
            FilteredElementCollector coll = new FilteredElementCollector(doc);
            //上一行是過濾元件的結果集合，我們以咖啡濾紙舉例，它就像我們的咖啡壺一樣，要接住我們篩選過的結果
            // 篩選條件為窗和牆的篩選器
            ElementCategoryFilter filter1 = new ElementCategoryFilter(BuiltInCategory.OST_Windows);
            ElementCategoryFilter filter2 = new ElementCategoryFilter(BuiltInCategory.OST_Doors);
            //邏輯篩選器，用以組合兩個或多個篩選器做使用，因為不可能有一個元件同時是牆又同時是窗，
            //亦即牆與窗兩種類別不可能發生交集，所以我們用聯集篩選器
            LogicalOrFilter orfilter = new LogicalOrFilter(filter1, filter2);
            IList<Element> elemList = coll.WherePasses(orfilter).WhereElementIsNotElementType().ToElements();
            //最後我們把想要的元件留在我們的「咖啡壺」上頭

            //產生DataTable草稿
            //DataTable dt = new DataTable("WindowOrDoors");
            //dt.Columns.Add("ElementID", typeof(int));
            //dt.Columns.Add("coordinate", typeof(string));
            //dt.Columns.Add("categoryType", typeof(string));
            //dt.Columns.Add("categoryName", typeof(string));
            //dt.Columns.Add("manufacturer", typeof(string));


            //foreach (Element elem in elemList) //抓到的都是門與窗
            //{
            //    int eid = elem.Id.IntegerValue;
            //    string cateType = elem.Category.Name;
            //    string cateName = elem.Name ;
            //    dt.Rows.Add(eid, "", cateType, cateName, "");

            //}
            //dt.WriteXml("D:\\draft.xml",XmlWriteMode.WriteSchema);


            DataTable dt = new DataTable("WindowOrDoors");
            dt.ReadXml("C:\\revitAPI\\draft.xml");

            foreach (Element elem in elemList) //抓到的都是門與窗
            {
                int eid = elem.Id.IntegerValue;
                //再來填寫幾何座標資訊
                List<XYZ> positionsSpecific = this.getPosition(elem);
                string coord = "(" + positionsSpecific[0].X + ", " + positionsSpecific[0].Y + ", " + positionsSpecific[0].Z + ")";
                dt.Select("ElementID='" + eid + "'")[0]["coordinate"]=coord;
            }

            dt.WriteXml("D:\\middle.xml",XmlWriteMode.WriteSchema);

            //cn.Close();
            MessageBox.Show("將座標寫至備註欄位");
        }

        /// <summary>
        /// 20171230為作業3新增之功能
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        public void hw3_addAndSetParaToBIM(UIDocument uidoc, Document doc)
        {
            DataTable dt = new DataTable("WindowOrDoors");
            dt.ReadXml("D:\\middle.xml");
            this.hw3_setNewParametersToCategory(uidoc, doc, BuiltInCategory.OST_Windows, dt);
            this.hw3_setNewParametersToCategory(uidoc, doc, BuiltInCategory.OST_Doors, dt);
            
        }

        /// <summary>
        /// 2017/12/30 為作業3改造參數新增與賦值函式
        /// </summary>
        /// <param name="uidoc"></param>
        /// <param name="doc"></param>
        /// <param name="category"></param>
        /// <param name="dr"></param>
        private void hw3_setNewParametersToCategory(UIDocument uidoc, Document doc,  BuiltInCategory category, DataTable dt)
        {
            string cateName = category.ToString();

            Transaction trans = new Transaction(doc);
            trans.Start("設定新增參數，綁定於實體之上");
            //必須先新增共用參數檔案
            FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "ShareParameter.txt");
            fileStream.Close();
            //然後將共用參數檔案綁定到Document之上
            doc.Application.SharedParametersFilename = Directory.GetCurrentDirectory() + "ShareParameter.txt";
            DefinitionFile defFile = doc.Application.OpenSharedParameterFile(); //打開共用參數檔

            List<string> fieldNames = new List<string>();
            fieldNames.Add("製造商");

            this.AddNewParametersToCategories(uidoc.Application, defFile, category, fieldNames); //呼叫方法以新增參數

            if (category == BuiltInCategory.OST_Windows)
            {
                DataRow[] drs = dt.Select("categoryType='窗'");
                for (int i = 0; i != drs.Length; i++)
                {
                    string id = drs[i]["ElementID"].ToString();
                    string manu = drs[i]["manufacturer"].ToString();
                    ElementId eid = new ElementId(Convert.ToInt32(id));
                    this.setSingleParameter(eid, doc, fieldNames[0], manu);
                }
            }

            else if (category == BuiltInCategory.OST_Doors)
            {
                DataRow[] drs = dt.Select("categoryType='門'");
                for (int i = 0; i != drs.Length; i++)
                {
                    string id = drs[i]["ElementID"].ToString();
                    string manu = drs[i]["manufacturer"].ToString();
                    ElementId eid = new ElementId(Convert.ToInt32(id));
                    this.setSingleParameter(eid, doc, fieldNames[0], manu);
                }
            }
            else { }

            trans.Commit();
        }



        public DataTable getDataTableBySpecificCategory(Document doc, BuiltInCategory category)
        {
            //用篩選器取得模型當中的所有牆
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(category);
            IList<Element> elementList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            DataTable dt = new DataTable("ElementParameters");
            dt.Columns.Add("ElementID", typeof(int));
            dt.Columns.Add("UniqueID", typeof(string));
            if (elementList.Count > 0)
            {
                //先從集合裡取一個元素來建立DataTable的樣板
                IEnumerator<Element> it = elementList.GetEnumerator();
                it.MoveNext(); //指到第一個元素
                Element templateElem = it.Current;

                List<XYZ> positions = this.getPosition(templateElem);
                if (positions.Count == 2)
                {
                    dt.Columns.Add("起點x座標", typeof(double));
                    dt.Columns.Add("起點y座標", typeof(double));
                    dt.Columns.Add("起點z座標", typeof(double));
                    dt.Columns.Add("終點x座標", typeof(double));
                    dt.Columns.Add("終點y座標", typeof(double));
                    dt.Columns.Add("終點z座標", typeof(double));
                }
                else if (positions.Count == 1)
                {
                    dt.Columns.Add("中心點x座標", typeof(double));
                    dt.Columns.Add("中心點y座標", typeof(double));
                    dt.Columns.Add("中心點z座標", typeof(double));
                }
                else { }

                foreach (Parameter para in templateElem.Parameters)
                {
                    Tuple<string, string, Type> currentItem = this.getParameterInfomation(para, doc);
                    //我意外從程式執行結果發現Parameters欄位還會發生重複的，原因不明，但只好這樣排除
                    if (dt.Columns.Contains(currentItem.Item1) == false)
                        dt.Columns.Add(currentItem.Item1, currentItem.Item3);
                }//end foreach

                //接下來正式上陣，填滿這個DataTable
                foreach (Element elem in elementList)
                {
                    int eid = elem.Id.IntegerValue;
                    string uid = elem.UniqueId;
                    DataRow dtRow = dt.NewRow();
                    //首先填寫辨識碼
                    dtRow["ElementID"] = eid;
                    dtRow["UniqueID"]=uid;
                    //再來填寫幾何座標資訊
                    List<XYZ> positionsSpecific = this.getPosition(elem);
                    if (positionsSpecific.Count == 2)
                    {
                        dtRow["起點x座標"] = positionsSpecific[0].X; dtRow["起點y座標"] = positionsSpecific[0].Y;
                        dtRow["起點z座標"] = positionsSpecific[0].Z;  dtRow["終點x座標"] = positionsSpecific[1].X;
                        dtRow["終點y座標"] = positionsSpecific[1].Y;  dtRow["終點z座標"] = positionsSpecific[1].Z;
                    }
                    else if (positionsSpecific.Count == 1)
                    {
                        dtRow["中心點x座標"]= positionsSpecific[0].X;
                        dtRow["中心點y座標"] = positionsSpecific[0].Y;
                        dtRow["中心點z座標"] = positionsSpecific[0].Z;
                    }
                    else { }
                    //最後針對一個元件的所有參數進行擷取
                    foreach (Parameter para in elem.Parameters) 
                    {
                        try
                        {
                            Tuple<string, string, Type> currentParameter = this.getParameterInfomation(para, doc);
                            //擷取參數的動作很麻煩，是官網提供的函式，做了一些改造並且用Tuple的結構傳回
                            dtRow[currentParameter.Item1] = currentParameter.Item2; //然後塞進DataTable的對應欄位中
                            //上述一行顯示出DataTable的強大，因為左值與右值理論上要變數型別相等，但現在右值一律是字串，
                            //而它只要「字串格式」能夠符合左值的要求（整數、布林或是浮點數）就會自動轉型置入
                        }
                        catch (Exception ex) { }
                    }
                    dt.Rows.Add(dtRow);//將新增資料列放入DataTable中
                }
                dt.WriteXml("D:\\revitAPI\\parameters_" + category.ToString() + ".xml", XmlWriteMode.WriteSchema);
                return dt;
            }
            else
            {
                MessageBox.Show("這個篩選器沒有得到任何結果");
                return dt;
            }
        }//end fun





        //這個函式的原型公佈在官網上，我做了一點點修改並且加上註解
        //https://knowledge.autodesk.com/search-result/caas/CloudHelp/cloudhelp/2017/ENU/Revit-API/files/GUID-D003803E-9FA0-4B2B-AB62-7DCC3A503644-htm.html
        private Tuple<string, string, Type> getParameterInfomation(Parameter para, Document doc)
        {
            string defName = para.Definition.Name;//這是欄位名稱，而欄位值是比較麻煩的
            //StorageType是Revit為了描述欄位變數型態而定義出來的enum
            switch (para.StorageType) //欄位值比需要區分變數類型來討論，設計得很不親切
            {
                case StorageType.Double: //如果這個欄位的型態是double
                    return new Tuple<string, string, Type>(defName, para.AsString(), typeof(string));      
                case StorageType.ElementId:
                    //如果欄位型態是ElementID識別碼，要分正整數及負整數討論
                    //因為是官網公佈應該這麼做的，因此從善如流
                    ElementId id = para.AsElementId();
                    if (id.IntegerValue >= 0)
                        return new Tuple<string, string, Type>(defName, doc.GetElement(id).Name, typeof(int));
                    else
                        return new Tuple<string, string, Type>(defName, id.IntegerValue.ToString(), typeof(int));
                case StorageType.Integer: //如果欄位是整數，它還有可能是布林值的零或一，要再寫一個if-else
                    if (ParameterType.YesNo == para.Definition.ParameterType)//如果欄位是布林值
                    {
                        if (para.AsInteger() == 0)
                            return new Tuple<string, string, Type>(defName, "False", typeof(bool));
                        else //para.AsInteger() == 1
                            return new Tuple<string, string, Type>(defName, "True", typeof(bool));
                    }
                    else //如果欄位變數型態真的是整數，而非布林值
                    {
                        return new Tuple<string, string, Type>(defName, para.AsInteger().ToString(), typeof(int));
                    }
                case StorageType.String: //如果是字串則最好處理
                    return new Tuple<string, string, Type>(defName, para.AsString(), typeof(string));

                default:
                    return new Tuple<string, string, Type>("未公開的參數", "無法定義的值", typeof(string)); 
                    //return "未公開的參數";
            }
        }//end fun

        /// <summary>
        /// 2017/12/30為了作業3小改一下，加入門與窗
        /// </summary>
        /// <param name="elem"></param>
        /// <returns></returns>
        private List<XYZ> getPosition(Element elem)
        {

            List<XYZ> positionArr = new List<XYZ>();

            if (elem.Category.Name == "牆" || elem.Category.Name == "結構構架")
            {
                LocationCurve locationCurve = elem.Location as LocationCurve;
                Line locationLine = locationCurve.Curve as Line;
                //以上是類別的轉型示範
                positionArr.Add(locationLine.GetEndPoint(0));
                positionArr.Add(locationLine.GetEndPoint(1));

            }

            else if (elem.Category.Name == "結構柱"|| elem.Category.Name == "門" || elem.Category.Name == "窗") //判斷類別，如果元件是柱的話才執行以下內容
            {
                LocationPoint locationPoint = elem.Location as LocationPoint;
                XYZ Point = locationPoint.Point as XYZ;
                //以上是類別的轉型示範
                positionArr.Add(locationPoint.Point);
            }
            else { }
            return positionArr;

        }//end fun


    }
}
