using System.Globalization;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;

namespace NameBadgeAutomater;

public class PowerPointTemplateService
{
  private static readonly Regex TEMPLATE_REGEX = new Regex("(first|last)_\\d+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

  public IEnumerable<NameTemplateResult> DiscoverTemplates(byte[] fileBytes)
  {
    PresentationDocument presentationDocument = null!;

    try
    {
      presentationDocument = PresentationDocument.Open(new MemoryStream(fileBytes), true);
    }
    catch (Exception ex)
    {
      throw new Exception($"This file seems corrupt and cannot be read. Error: {ex.Message}");
    }

    if (presentationDocument?.PresentationPart is null)
    {
      throw new Exception("This file seems corrupt and cannot be read. Error: Specified part does not exist in the package.");
    }

    if (presentationDocument.PresentationPart.SlideParts.Count() == 0)
    {
      throw new Exception("This file has no slide. One slide is expected.");
    }

    if (presentationDocument.PresentationPart.SlideParts.Count() > 1)
    {
      throw new Exception("This file has more than one slide. One slide expected.");
    }

    // Validate Slide
    using var streamReader = new StreamReader(presentationDocument.PresentationPart.SlideParts.First().GetStream());
    var slideContent = streamReader.ReadToEnd();

    var matchedTemplateTexts = TEMPLATE_REGEX
                                .Matches(slideContent)
                                .Select(x => x.Value)
                                .ToList();

    var groupedTemplates = matchedTemplateTexts
                            .GroupBy(x => x)
                            .ToDictionary(x => x.Key, x => x.Count());

    var indexedTemplates = groupedTemplates
                            .GroupBy(x => x.Key.Split("_").Last())
                            .Select(x => new NameTemplateResult
                            {
                              Index = int.Parse(x.Key),
                              FirstCount = x.FirstOrDefault(xx => xx.Key.StartsWith("first", true, CultureInfo.CurrentUICulture)).Value,
                              LastCount = x.FirstOrDefault(xx => xx.Key.StartsWith("last", true, CultureInfo.CurrentUICulture)).Value,
                            })
                            .ToList();

    if (!indexedTemplates.Any()) return Enumerable.Empty<NameTemplateResult>();  

    // Add missing indexes.
    for (int index = 1; index <= indexedTemplates.Max(x => x.Index); index++)
    {
      if (indexedTemplates.SingleOrDefault(x => x.Index == index) is not null) continue;

      indexedTemplates.Add(new NameTemplateResult { Index = index });
    }

    return indexedTemplates;
  }

  public bool IsTemplateValid(IEnumerable<NameTemplateResult> discoveredTemplates) =>
    discoveredTemplates.Count() > 0 &&
    discoveredTemplates.All(x => x.FirstCount > 0 && x.LastCount > 0);

  public byte[] GenerateFromTemplate(byte[] fileBytes, List<Person> people, int badgesPerPage, bool blankUnusedBadges = true)
  {
    // Open document
    var presentationDocument = PresentationDocument.Open(new MemoryStream(fileBytes), true);
    var presentationPart = presentationDocument.PresentationPart!;

    // Get template slide (there should only be one slide)
    var templatePart = presentationPart.GetSlidePartsInOrder().Last();

    // Generate new slides 
    people
      .Chunk(badgesPerPage)
      .Select(peopleGroup => GenerateNewSlideWithNames(templatePart, peopleGroup, badgesPerPage, blankUnusedBadges))
      .ToList()
      .ForEach(newSlide => presentationPart.AppendSlide(newSlide));

    // Remove template slide
    presentationPart.RemoveSlide(0);

    // Save document
    using var outStream = new MemoryStream();
    presentationDocument.Clone(outStream).Close();

    return outStream.ToArray();
  }

  private static SlidePart GenerateNewSlideWithNames(SlidePart templatePart, Person[] peopleGroup, int badgesPerPage, bool blankUnusedBadges)
  {
    var newSlidePart = templatePart.CloneSlide();

    for (var i = 0; i < peopleGroup.Count(); i++)
    {
      var person = peopleGroup[i];

      newSlidePart.Slide.InnerXml = newSlidePart.Slide.InnerXml
        .Replace("FIRST_" + (i + 1).ToString(), person.FirstName.ToUpperInvariant())
        .Replace("LAST_" + (i + 1).ToString(), person.LastName.ToUpperInvariant())
        .Replace("first_" + (i + 1).ToString(), person.FirstName, true, CultureInfo.CurrentUICulture)
        .Replace("last_" + (i + 1).ToString(), person.LastName, true, CultureInfo.CurrentUICulture);
    }

    if (blankUnusedBadges)
    {
      for (var i = peopleGroup.Count(); i < badgesPerPage; i++)
      {
        newSlidePart.Slide.InnerXml = newSlidePart.Slide.InnerXml
          .Replace("first_" + (i + 1), string.Empty, true, CultureInfo.CurrentUICulture)
          .Replace("last_" + (i + 1), string.Empty, true, CultureInfo.CurrentUICulture);
      }
    }

    return newSlidePart;
  }
}