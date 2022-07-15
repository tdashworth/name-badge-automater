

using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;

namespace NameBadgeAutomater
{
  public class PowerPointTemplateService
  {
    private static readonly int BADGES_PER_PAGE = 8;
    private static readonly string BASE_FIRSTNAME_TEMPLATE = "first_{0}";
    private static readonly string BASE_LASTNAME_TEMPLATE = "last_{0}";
    private static readonly List<Regex> FIRSTNAME_REGEXS = Enumerable
      .Range(1, BADGES_PER_PAGE)
      .Select(i => new Regex(string.Format(BASE_FIRSTNAME_TEMPLATE, i), RegexOptions.Compiled))
      .ToList();
    private static readonly List<Regex> LASTNAME_REGEXS = Enumerable
      .Range(1, BADGES_PER_PAGE)
      .Select(i => new Regex(string.Format(BASE_LASTNAME_TEMPLATE, i), RegexOptions.Compiled))
      .ToList();

    public List<string> ValidateTemplate(byte[] fileBytes)
    {
      var validationErrors = new List<string>();

      try
      {
        var presentationDocument = PresentationDocument.Open(new MemoryStream(fileBytes), true);

        if (presentationDocument?.PresentationPart is null)
        {
          validationErrors.Add("This file seems corrupt and cannot be read. Error: No Presentation XML part.");
          return validationErrors; // Short circuit since all other validation will fail.
        }

        if (presentationDocument.PresentationPart.SlideParts.Count() == 0)
        {
          validationErrors.Add("This file has no slide. One slide is expected.");
          return validationErrors; // Short circuit since all other validation will fail.
        }

        if (presentationDocument.PresentationPart.SlideParts.Count() > 1)
        {
          validationErrors.Add("This file has more than one slide. One slide expected.");
        }

        using var streamReader = new StreamReader(presentationDocument.PresentationPart.SlideParts.First().GetStream());
        var slideContent = streamReader.ReadToEnd();

        foreach (var regex in FIRSTNAME_REGEXS)
        {
          var matches = regex.Matches(slideContent);
          if (matches.Count() == 0)
          {
            validationErrors.Add($"Could not find expected template '{regex.ToString()}.'");
          }
          if (matches.Count() > 1)
          {
            validationErrors.Add($"Multiple '{regex.ToString()} templates were found when only one was expected.'");
          }
        }
        foreach (var regex in LASTNAME_REGEXS)
        {
          var matches = regex.Matches(slideContent);
          if (matches.Count() == 0)
          {
            validationErrors.Add($"Could not find expected template '{regex.ToString()}.'");
          }
          if (matches.Count() > 1)
          {
            validationErrors.Add($"Multiple '{regex.ToString()} templates were found when only one was expected.'");
          }
        }
      }
      catch (Exception ex)
      {
        validationErrors.Add($"This file seems corrupt and cannot be read. Error: {ex}");
      }

      return validationErrors;
    }

    public byte[] GenerateFromTemplate(byte[] fileBytes, List<Person> people, bool emptyExtras = true)
    {
      var presentationDocument = PresentationDocument.Open(new MemoryStream(fileBytes), true);
      var presentationPart = presentationDocument.PresentationPart!;

      var templatePart = presentationPart.GetSlidePartsInOrder().Last();

      foreach (var peopleGroup in people.Chunk(BADGES_PER_PAGE))
      {
        var newSlidePart = templatePart.CloneSlide();
        if (newSlidePart.Slide != null)
        {

          for (var i = 0; i < peopleGroup.Count(); i++)
          {
            var person = peopleGroup[i];
            var firstNameRegex = FIRSTNAME_REGEXS[i];
            var lastNameRegex = LASTNAME_REGEXS[i];

            newSlidePart.Slide.InnerXml = firstNameRegex.Replace(newSlidePart.Slide.InnerXml, person.FirstName);
            newSlidePart.Slide.InnerXml = lastNameRegex.Replace(newSlidePart.Slide.InnerXml, person.LastName);
          }

          if (emptyExtras)
          {
            for (var i = peopleGroup.Count(); i < BADGES_PER_PAGE; i++)
            {
              var firstNameRegex = FIRSTNAME_REGEXS[i];
              var lastNameRegex = LASTNAME_REGEXS[i];

              newSlidePart.Slide.InnerXml = firstNameRegex.Replace(newSlidePart.Slide.InnerXml, "");
              newSlidePart.Slide.InnerXml = lastNameRegex.Replace(newSlidePart.Slide.InnerXml, "");
            }
          }
        }
        presentationPart.AppendSlide(newSlidePart);
      }

      presentationPart.RemoveSlide(0);

      presentationPart.Presentation.Save();
      if (OpenXmlPackage.CanSave)
      {
        presentationDocument.Save();
      }

      using var outStream = new MemoryStream();
      presentationDocument.Clone(outStream).Close();

      return outStream.ToArray();
    }
  }
}