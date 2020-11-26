using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using System.IO;
using System.Windows.Forms;

namespace revitAPI2017
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class C002_AddParameters : IExternalCommand
    {
        /// <summary>
        /// This example add a new parameter "Note" to the Wall elements
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {            
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document; 
            //this is the first time we need to generate a transaction object to chaneg the model
            Transaction trans = new Transaction(doc);
            trans.Start("instance binding");
            //create new parameter file
            FileStream fileStream = File.Create("D:\\ShareParameter.txt");
            fileStream.Close();
            //bind the file to BIM model
            doc.Application.SharedParametersFilename = "D:\\ShareParameter.txt";
            DefinitionFile defFile = doc.Application.OpenSharedParameterFile(); 
            bool bindResult=SetNewParameterToTypeWall(uidoc.Application, defFile); 
            //call the method to add new parameter
            if (bindResult == true)
                MessageBox.Show("Successfully add the new parameter \"Note\" by instance binding!");
            trans.Commit();            
            return Result.Succeeded;
        }
        /// <summary>
        ///This method is provided by the official website, here is the link
        ///https://www.revitapidocs.com/2019/f5066ef5-fa12-4cd2-ad0c-ca72ab21e479.htm
        /// </summary>
        /// <param name="app"></param>
        /// <param name="myDefinitionFile"></param>
        /// <returns></returns>
        public bool SetNewParameterToTypeWall(UIApplication app, DefinitionFile myDefinitionFile)
        {
            // Create a new group in the shared parameters file
            DefinitionGroups myGroups = myDefinitionFile.Groups;
            DefinitionGroup myGroup = myGroups.Create("Revit API Course");
            // Create a type definition
            ExternalDefinitionCreationOptions option = new ExternalDefinitionCreationOptions("Note", ParameterType.Text);
            Definition myDefinition_CompanyName = myGroup.Definitions.Create(option);
            // Create a category set and insert category of wall to it
            CategorySet myCategories = app.Application.Create.NewCategorySet();
            // Use BuiltInCategory to get category of wall
            Category myCategory = Category.GetCategory(app.ActiveUIDocument.Document, BuiltInCategory.OST_Walls);
            myCategories.Insert(myCategory);//add wall into the group. Of course, you can add multiple categories
            //Create an object of InstanceBinding according to the Categories,
            //The next line: I also provide the TypeBinding example here, but hide the code by comments
            //TypeBinding typeBinding = app.Application.Create.NewTypeBinding(myCategories);
            InstanceBinding instanceBinding = app.Application.Create.NewInstanceBinding(myCategories);
            // Get the BingdingMap of current document.
            BindingMap bindingMap = app.ActiveUIDocument.Document.ParameterBindings;
            //Bind the definitions to the document
            bool typeBindOK = bindingMap.Insert(myDefinition_CompanyName, instanceBinding,
              BuiltInParameterGroup.PG_TEXT);
            //The next two lines: I also provide the TypeBinding example here, but hide the code by comments
            //bool typeBindOK=bindingMap.Insert(myDefinition_CompanyName, typeBinding,
            //BuiltInParameterGroup.PG_TEXT);
            return typeBindOK;
        }

    };
}
