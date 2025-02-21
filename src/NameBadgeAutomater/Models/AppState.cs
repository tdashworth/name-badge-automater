namespace NameBadgeAutomater;

public class AppState
{
  // Upload File
  public byte[]? PowerPointFile { get; set; }
  public string FileName { get; set; } = string.Empty;
  public IEnumerable<NameTemplateResult>? DiscoveredPlaceholders { get; set; }
  public int BadgesPerPage => DiscoveredPlaceholders?.Count() ?? 0;
  public bool CompanySupported => DiscoveredPlaceholders?.Any(x => x.CompanyCount > 0) ?? false;

  // Enter Names
  public string RawNames { get; set; } = string.Empty;
  public string RawCompanies { get; set; } = string.Empty;
  public List<Person> People { get; set; } = new List<Person>();
  public bool NamesAndCompaniesCountMatch =>
    RawNames
      .Split(Environment.NewLine)
      .SelectMany(line => line.Contains(";") ? line.Split(";", StringSplitOptions.RemoveEmptyEntries) : new string[] { line })
      .Count()
    ==
    RawCompanies
      .Split(Environment.NewLine)
      .Length;

  // Generate
  public bool HasGeneratedBadges { get; set; }
}
