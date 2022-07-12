
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Components.Forms;

namespace NameBadgeAutomator
{
  public class AppState
  {
    public byte[] PowerPointFile { get; set; }
    public List<Person> People { get; set; } = new List<Person>();
  }
}