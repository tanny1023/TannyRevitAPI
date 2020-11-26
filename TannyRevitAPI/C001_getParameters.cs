using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace TannyRevitAPI
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]

    public class C001_GetParameters : IExternalCommand
    {
        /// <summary>
        /// print all the parameters for all of the walls in a BIM model, 
        /// must define a method to discuss different types of parameters
        /// </summary>
        /// <param name="commandData"></param>
        /// <param name="message"></param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //select all the walls in a BIM model by element filter
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            ElementCategoryFilter filter = new ElementCategoryFilter(BuiltInCategory.OST_Walls);
            IList<Element> wallList = collector.WherePasses(filter).WhereElementIsNotElementType().ToElements();

            StringBuilder sb = new StringBuilder(); 
            foreach (Element wall in wallList) //for each wall
            {
                sb.AppendLine(wall.Name+"::"+wall.Id);
                foreach (Parameter para in wall.Parameters) //for each parameter
                {
                    sb.Append(this.getParameterInfomation(para, doc, ":")); //print parameter information
                    sb.AppendLine();
                }
                sb.AppendLine(); 
            }
            MessageBox.Show(sb.ToString());  
            return Result.Succeeded;
        }

        /// <summary>
        /// this method is to judge the data type of parameter, 
        /// and make correct conversions of parameter for return the parameter name and value as a string
        /// </summary>
        /// <param name="para"></param>
        /// <param name="doc"></param>
        /// <param name="seperator"></param>
        /// <returns></returns>
         private string getParameterInfomation(Parameter para, Document doc, string seperator)
        {
            string defName = para.Definition.Name;//get parameter name, which is the first half of the return value
            //switch the StorageType, which is a Enum for defining the data type
            switch (para.StorageType)
            {
                case StorageType.Double: //if the parameter is a double, use AsValueString() to return the value
                    return defName + seperator + para.AsValueString();

                case StorageType.ElementId:
                    //if the parameter is a element id, its IntegerValue would be either a positive integer or -1
                    //-1 means an invalid element id
                    ElementId id = para.AsElementId();
                    if (id.IntegerValue >= 0) //if IntegerValue is not -1, we can get the parameter by Element.Name
                        return defName + seperator + doc.GetElement(id).Name;
                    else //if IntegerValue is -1, print -1 because it is an invalid ElementId
                        return defName + seperator + id.IntegerValue.ToString();

                case StorageType.Integer: //if the parameter is an integer, it would be a boolean or an integer
                    if (ParameterType.YesNo == para.Definition.ParameterType)//if it is a boolean (0 or 1)
                    {
                        //we must use true or false to represent the parameter instead of 0 or 1
                        if (para.AsInteger() == 0) 
                            return defName + seperator + "False"; 
                        else //para.AsInteger() == 1
                            return defName + seperator + "True";
                    }
                    else //if it is an integer, just use AsInteger().ToString() to get the string value
                    {
                        return defName + seperator + para.AsInteger().ToString();
                    }

                case StorageType.String: //if it is a string, we can return it by AsString()
                    return defName + seperator + para.AsString();

                default:
                    return "unknown parameter";
            }
        }//end fun
    }
}
