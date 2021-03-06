using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RvtParamDrop
{
  class ParamDropData
  {
    public ElementId HostElementId { get; set; }
    public string HostCategory { get; set; }
    public string HostElementName { get; set; }
    public ElementId ParameterId { get; set; }
    public string ParameterTypeId { get; set; }
    public string ParameterName { get; set; }
    public string ParameterValue { get; set; }

    /// <summary>
    /// Return CSV header line
    /// </summary>
    static public string CsvHeader
    {
      get
      {
        return "HostElementId,HostCategory,HostElementName,ParameterId,ParameterTypeId,ParameterName,ParameterValue";
      }
    }

    /// <summary>
    /// Wrap CSV output entry in double quotes
    /// </summary>
    static public string CsvQuote(string s)
    {
      return "\"" + s + "\"";
    }

    /// <summary>
    /// Return a comma separated string representing this entry
    /// </summary>
    public string CsvString
    {
      get
      {
        return HostElementId.IntegerValue.ToString()
          + "," + CsvQuote(HostCategory)
          + "," + CsvQuote(HostElementName)
          + "," + ParameterId.IntegerValue.ToString()
          + "," + ParameterTypeId
          + "," + CsvQuote(ParameterName)
          + "," + CsvQuote(ParameterValue);
      }
    }
  }
}
