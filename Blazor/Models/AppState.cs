
using BlazorWorker.Core;
using BlazorWorker.WorkerBackgroundService;

namespace NameBadgeAutomater
{
  public class AppState
  {
    public byte[]? PowerPointFile { get; set; }
    public List<Person> People { get; set; } = new List<Person>();
    public bool HasGeneratedBadges { get; set; }
    public IWorker? Worker { get; set; }
    public IWorkerBackgroundService<PowerPointTemplateService>? Service {get; set;}
  }
}