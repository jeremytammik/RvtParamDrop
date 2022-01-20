#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;

#endregion

namespace RvtParamDrop
{
  [Transaction(TransactionMode.ReadOnly)]
  public class Command : IExternalCommand
  {

    public void GetParams(Document doc)
    {

      //UIDocument uidoc = this.ActiveUIDocument;
      //Document doc = this.Document;


      List<Element> sharedParams = new List<Element>();

      FilteredElementCollector collector
          = new FilteredElementCollector(doc)
          .WhereElementIsNotElementType();

      // Filter elements for shared parameters only
      collector.OfClass(typeof(SharedParameterElement));

      String paramList = "";
      foreach (Element e in collector)
      {
        SharedParameterElement param = e as SharedParameterElement;
        Definition def = param.GetDefinition();
        paramList += def.Name + "\r\n";
      }
      TaskDialog.Show("Result", paramList);
      System.IO.File.WriteAllText(@"C:\Users\parrela\desktop\Params.txt", paramList);
    }

    void ParamDropForView(View view)
    {
      Document doc = view.Document;

      FilteredElementCollector els
        = new FilteredElementCollector(doc, view.Id);

      foreach (Element e in els)
      {
        Debug.Print(e.Name);
      }

    }

    public Result Execute(
      ExternalCommandData commandData,
      ref string message,
      ElementSet elements)
    {
      UIApplication uiapp = commandData.Application;
      UIDocument uidoc = uiapp.ActiveUIDocument;
      Application app = uiapp.Application;
      Document doc = uidoc.Document;
      View view = doc.ActiveView;

      ParamDropForView(view);

      return Result.Succeeded;
    }
  }
}
