using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RvtParamDrop
{
  class ParamDropData
  {
    public ElementId HostElementId { get; set; }
    public ElementId ParameterId { get; set; }
    public string ParameterTypeId { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }

  }
}
