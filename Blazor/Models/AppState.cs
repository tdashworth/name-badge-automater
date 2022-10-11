
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Forms;

namespace NameBadgeAutomater
{
  public class AppState
  {
    public int BadgesPerPage { get; set; }
    public byte[]? PowerPointFile { get; set; }
    public List<Person> People { get; set; } = new List<Person>();
    public bool HasGeneratedBadges { get; set; }
  }
}