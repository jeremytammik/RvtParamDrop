using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RvtParamDrop
{
  class ElementIdSet : Dictionary<ElementId, int>
  {
    public void Add( ElementId id)
    {
      if (null != id
        && ElementId.InvalidElementId != id)
      {
        if (!ContainsKey(id))
        {
          Add(id, 1);
        }
        else
        {
          ++this[id];
        }
      }

    }
  }
}
