using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Forms;

namespace SampleCode
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class Cs15_SharedParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            Transaction trans = new Transaction(doc);
            trans.Start("綁定類型");

            // 首先必須建立用來存放客製化參數定義的共用參數文檔，你可以設定路徑及檔案名稱，
            // 如果沒有設路徑的話，預設會建立在開啟的revit專案檔所在目錄下
            System.IO.FileStream fileStream = System.IO.File.Create("SharedParamFile.txt");
            fileStream.Close();

            // 尋找檔案路徑目錄，這邊檔案路徑是存放在預設的revit專案檔所在目錄下
            string path = System.Environment.CurrentDirectory;
            // 設定共用參數文檔的路徑
            doc.Application.SharedParametersFilename = path + @"\SharedParamFile.txt";

            // 開啟共用參數文檔
            DefinitionFile definFile = doc.Application.OpenSharedParameterFile();

            // 定義參數並綁定到牆類型中
            bool result = setNewParameterToTypeWall(uidoc.Application, definFile);

            trans.Commit();
            if (result == true)
                return Result.Succeeded;
            else
            {
                message = "客製化的參數綁定元件失敗";
                return Result.Failed;
            }
        }
        // 定義參數與綁定類型的Function
        private bool setNewParameterToTypeWall(UIApplication app, DefinitionFile myDefinitionFile)
        {
            // 共用參數文檔中包含了Group，代表參數所屬的群組，
            // 而Group內又包含了Definition，也就是你想增加的參數，
            // 欲新增Definition首先必須新增一個Group
            DefinitionGroups myGroups = myDefinitionFile.Groups;
            DefinitionGroup myGroup = myGroups.Create("參數群組");

            // 創建Definition的定義以及其儲存類型
            ExternalDefinitionCreationOptions options
                          = new ExternalDefinitionCreationOptions("聯絡人", ParameterType.Text);
            Definition myDefinition = myGroup.Definitions.Create(options);

            // 創建品類集並插入牆品類到裡面
            CategorySet myCategories = app.Application.Create.NewCategorySet();

            // 使用BuiltInCategory來取得牆品類
            Category myCategory =
     app.ActiveUIDocument.Document.Settings.Categories.get_Item(BuiltInCategory.OST_Walls);
            myCategories.Insert(myCategory);

            // 根據品類創建類型綁定的物件
            TypeBinding typeBinding = app.Application.Create.NewTypeBinding(myCategories);

            // 取得當前文件中所有的類型綁定
            BindingMap bindingMap = app.ActiveUIDocument.Document.ParameterBindings;

            // 將客製化的參數定義綁定到文件中
            bool typeBindOK = bindingMap.Insert(myDefinition, typeBinding,
     BuiltInParameterGroup.PG_TEXT);

            return typeBindOK;
        }
    }
}
