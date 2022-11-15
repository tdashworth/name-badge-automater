
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Forms;

namespace NameBadgeAutomater
{
  public class AppState
  {
    public byte[]? PowerPointFile { get; set; }
    public int BadgesPerPage { get; set; }
    public string RawNames { get; set; } = string.Empty;
    public List<Person> People { get; set; } = new List<Person>();
    public bool HasGeneratedBadges { get; set; }
  }
}