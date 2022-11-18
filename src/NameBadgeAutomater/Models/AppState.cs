
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Forms;

namespace NameBadgeAutomater
{
  public class AppState
  {
    // Upload File
    public byte[]? PowerPointFile { get; set; }
    public string FileName { get; set; } = string.Empty;
    public IEnumerable<NameTemplateResult>? DiscoveredPlaceholders { get; set; }
    public int BadgesPerPage { get; set; }

    // Enter Names
    public string RawNames { get; set; } = string.Empty;
    public List<Person> People { get; set; } = new List<Person>();

    // Generate
    public bool HasGeneratedBadges { get; set; }
  }
}