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

    public class addParameters : IExternalCommand
    {

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //開啟模型並選取其中一部份
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Autodesk.Revit.UI.Selection.Selection selElement = uidoc.Selection;
            Autodesk.Revit.DB.Document doc = uidoc.Document; 

            Transaction trans = new Transaction(doc);
            trans.Start("設定新增參數，綁定於實體之上");
            //必須先新增共用參數檔案
            FileStream fileStream = File.Create(Directory.GetCurrentDirectory() + "ShareParameter.txt");
            fileStream.Close();
            //然後將共用參數檔案綁定到Document之上
            doc.Application.SharedParametersFilename = Directory.GetCurrentDirectory() + "ShareParameter.txt";
            DefinitionFile defFile = doc.Application.OpenSharedParameterFile(); //打開共用參數檔
            bool bindResult=SetNewParameterToTypeWall(uidoc.Application, defFile); //呼叫方法以新增參數
            if (bindResult == true)
                MessageBox.Show("成功新增共享參數「牆上塗鴉」");

            trans.Commit();
            
            return Result.Succeeded;
        }

        //這個方法也是被公開在官網上的，我只是小小修改並且翻譯了註解，參考以下網址
        //http://www.revitapidocs.com/2017/f5066ef5-fa12-4cd2-ad0c-ca72ab21e479.htm
        public bool SetNewParameterToTypeWall(UIApplication app, DefinitionFile myDefinitionFile)
        {
            //在共用參數檔案當中先建立一個新的參數群組
            DefinitionGroups myGroups = myDefinitionFile.Groups;
            DefinitionGroup myGroup = myGroups.Create("新參數群組");
            // 創立參數定義，含參數名稱及參數型別
            ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions("牆上塗鴉", ParameterType.Text);
            Definition myDefinition_CompanyName = myGroup.Definitions.Create(option);
            // 創建一個類別群組，將來在這個類別群組裡的所有類別都會被新增指定的參數
            CategorySet myCategories = app.Application.Create.NewCategorySet();
            // 創建一個類別．以牆為例
            Category myCategory = Category.GetCategory(app.ActiveUIDocument.Document, BuiltInCategory.OST_Walls);
            myCategories.Insert(myCategory);//把牆類別插入類別群組，當然你可以插入不只一個類別
            //以下兩者是亮點，「類別綁定」與「實體綁定」的結果不同，以本例而言我們需要的其實是實體綁定
            //TypeBinding typeBinding = app.Application.Create.NewTypeBinding(myCategories);
            InstanceBinding instanceBinding = app.Application.Create.NewInstanceBinding(myCategories);
            //取得一個叫作BingdingMap的物件，以進行後續新增參數
            BindingMap bindingMap = app.ActiveUIDocument.Document.ParameterBindings;
            // 最後將新增參數加到群組上頭，並且指定了instanceBiding的方法（也可替換為typeBinding）
            bool typeBindOK = bindingMap.Insert(myDefinition_CompanyName, instanceBinding,
              BuiltInParameterGroup.PG_TEXT);
            //bool typeBindOK=bindingMap.Insert(myDefinition_CompanyName, typeBinding,
              //BuiltInParameterGroup.PG_TEXT);
            return typeBindOK;
        }

    };
}
