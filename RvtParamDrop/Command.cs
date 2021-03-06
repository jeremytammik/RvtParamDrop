#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
#endregion

namespace RvtParamDrop
{
  [Transaction(TransactionMode.ReadOnly)]
  public class Command : IExternalCommand
  {
    /// <summary>
    /// Output file path
    /// </summary>
    string _filepath = "C:/tmp/RvtParamDrop.csv";

    /// <summary>
    /// Data collector for exported data mapping
    /// ElementId --> ParameterId --> parameter data
    /// </summary>
    Dictionary<ElementId, Dictionary<ElementId, ParamDropData>> _data = null;

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

    void ParamDropForParam(Element host, Parameter p)
    {
      ElementId eid = host.Id;

      if (!_data.ContainsKey(eid))
      {
        _data.Add(eid, new Dictionary<ElementId, ParamDropData>());
      }

      ElementId paramid = p.Id;

      if (_data[eid].ContainsKey(paramid))
      {
        return;
      }

      Document doc = host.Document;
      Definition def = p.Definition;
      Category cat = host.Category;

      ParamDropData d = new ParamDropData()
      {
        HostElementId = eid,
        HostCategory = (null == cat) ? "" : cat.Name,
        HostElementName = host.Name,
        ParameterId = paramid,
        ParameterName = def.Name,
        ParameterValue = p.AsValueString(),
        ParameterTypeId = def.GetDataType().TypeId
      };

      _data[eid].Add(paramid, d);

      // Recurse to process all referenced elements

      StorageType st = p.StorageType;
      if (StorageType.ElementId == st)
      {
        ElementId id = p.AsElementId();
        if (null != id
          && ElementId.InvalidElementId != id
          && 0 < id.IntegerValue)
        {
          Element e = doc.GetElement(id);
          if (null != e)
          {
            ParamDropForElement(e);
          }
        }
      }
    }

    void ParamDropForElement(Element e)
    {
      if (!_data.ContainsKey(e.Id))
      {
        ParameterSet ps = e.Parameters;
        int n = ps.Size;
        Debug.Print("Element <{0}> '{1}' has {2} parameters", e.Id, e.Name, n);
        Dictionary<ElementId, int> refIds = new Dictionary<ElementId, int>();
        ParameterSetIterator i = ps.ForwardIterator();
        while (i.MoveNext())
        {
          Object obj = i.Current;
          Parameter p = obj as Parameter;
          ParamDropForParam(e, p);
        }
      }
    }

    void ParamDropForView(View view)
    {
      Document doc = view.Document;

      FilteredElementCollector col 
        = new FilteredElementCollector(doc, view.Id);

      int nElem = col.GetElementCount();
      Debug.Print("{0} elements visible in view", nElem);
      ElementIdSet typeIds = new ElementIdSet();

      foreach (Element e in col)
      {
        ParamDropForElement(e);

        // Collect all element types
        // Probably superfluous, since they are already 
        // collected by the ParamDropForElement recursion.

        ElementId tid = e.GetTypeId();
        typeIds.Add(tid);
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

      _data = new Dictionary<ElementId, Dictionary<ElementId, ParamDropData>>();

      ParamDropForView(view);

      // Dump data

      using (StreamWriter s = new StreamWriter(_filepath))
      {
        s.WriteLine(ParamDropData.CsvHeader);

        List<int> eids = new List<int>(_data.Keys
          .Select<ElementId, int>(id => id.IntegerValue));

        eids.Sort();
        foreach (int id in eids)
        {
          ElementId eid = new ElementId(id);

          List<int> pids = new List<int>(_data[eid].Keys
            .Select<ElementId, int>(id2 => id2.IntegerValue));

          pids.Sort();
          foreach (int id2 in pids)
          {
            ElementId pid = new ElementId(id2);
            s.WriteLine(_data[eid][pid].CsvString);
          }
        }
        s.Close();
      }
      return Result.Succeeded;
    }
  }
}
