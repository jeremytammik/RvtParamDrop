#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    void ParamDropForElement(Element e)
    {
      Debug.Print(e.Name);
      ParameterSet ps = e.Parameters;
      int n = ps.Size;
      Debug.Print("Element <{0}> '{1}' has {2} parameters", e.Id,e.Name, n);
      ParameterSetIterator i = ps.ForwardIterator();
      while( i.MoveNext())
      {
        Object obj = i.Current;
        Parameter p = obj as Parameter;
        Definition def = p.Definition;
      }
    }

    void ParamDropForView(View view)
    {
      Document doc = view.Document;

      FilteredElementCollector col 
        = new FilteredElementCollector(doc, view.Id);

      int nElem = col.GetElementCount();
      Debug.Print("{0} elements visible in view", nElem);
      Dictionary<ElementId, int> typeIds = new Dictionary<ElementId, int>();

      foreach (Element e in col)
      {
        ParamDropForElement(e);

        // Collect all element types

        ElementId tid = e.GetTypeId();
        if (null != tid
          && ElementId.InvalidElementId != tid)
        {
          if (!typeIds.ContainsKey(tid))
          {
            typeIds.Add(tid, 1);
          }
          else
          {
            ++typeIds[tid];
          }
        }
      }

      foreach (ElementId id in typeIds.Keys)
      {
        Element e = doc.GetElement(id);
        ParamDropForElement(e);
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
