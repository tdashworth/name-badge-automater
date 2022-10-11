

using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Packaging;

namespace NameBadgeAutomater
{
  public class PowerPointTemplateService
  {
    private static readonly Regex BASE_FIRSTNAME_TEMPLATE = new Regex("first_\\d", RegexOptions.Compiled);
    private static readonly Regex BASE_LASTNAME_TEMPLATE = new Regex("last_\\d", RegexOptions.Compiled);

    public (int, List<string>) ValidateTemplate(byte[] fileBytes)
    {
      var validationErrors = new List<string>();
      var badgesPerPage = 0;

      try
      {
        // Validate File
        var presentationDocument = PresentationDocument.Open(new MemoryStream(fileBytes), true);
        if (presentationDocument?.PresentationPart is null)
        {
          validationErrors.Add("This file seems corrupt and cannot be read. Error: No Presentation XML part.");
          return (0, validationErrors); // Short circuit since all other validation will fail.
        }

        if (presentationDocument.PresentationPart.SlideParts.Count() == 0)
        {
          validationErrors.Add("This file has no slide. One slide is expected.");
          return (0, validationErrors); // Short circuit since all other validation will fail.
        }

        if (presentationDocument.PresentationPart.SlideParts.Count() > 1)
        {
          validationErrors.Add("This file has more than one slide. One slide expected.");
        }

        // Validate Slide
        using var streamReader = new StreamReader(presentationDocument.PresentationPart.SlideParts.First().GetStream());
        var slideContent = streamReader.ReadToEnd();

        var firstNameMatches = BASE_FIRSTNAME_TEMPLATE.Matches(slideContent);
        var lastNameMatches = BASE_LASTNAME_TEMPLATE.Matches(slideContent);

        if (firstNameMatches.Count != lastNameMatches.Count)
        {
          validationErrors.Add("Number of first_x and last_x templates do not match.");
        }

        var firstNameMatchedTexts = firstNameMatches.Select(x => x.Value);
        var lastNameMatchedTexts = lastNameMatches.Select(x => x.Value);

        for (int index = 1; index <= firstNameMatches.Count; index++)
        {
          var firstNameExpectedText = BASE_FIRSTNAME_TEMPLATE.ToString().Replace("\\d", index.ToString());
          ValidateExpectedTemplateOnSlide(firstNameMatchedTexts, firstNameExpectedText, validationErrors);

          var lastNameExpectedText = BASE_LASTNAME_TEMPLATE.ToString().Replace("\\d", index.ToString());
          ValidateExpectedTemplateOnSlide(lastNameMatchedTexts, lastNameExpectedText, validationErrors);

          badgesPerPage++;
        }

        if (badgesPerPage == 0)
        {
          validationErrors.Add("No templates were found. At least one pair of template is expected.");
        }
      }
      catch (Exception ex)
      {
        validationErrors.Add($"This file seems corrupt and cannot be read. Error: {ex}");
      }

      return (badgesPerPage, validationErrors);
    }

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

    private static void ValidateExpectedTemplateOnSlide(IEnumerable<string> foundTemplates, string expectedTemplate, List<string> validationErrors)
    {
      var countFound = foundTemplates.Count(x => string.Equals(x, expectedTemplate));
      if (countFound == 0)
      {
        validationErrors.Add($"Could not find expected template '{expectedTemplate}.'");
      }
      if (countFound > 1)
      {
        validationErrors.Add($"Multiple '{expectedTemplate} templates were found when only one was expected.'");
      }
    }

    private static SlidePart GenerateNewSlideWithNames(SlidePart templatePart, Person[] peopleGroup, int badgesPerPage, bool blankUnusedBadges)
    {
      var newSlidePart = templatePart.CloneSlide();

      for (var i = 0; i < peopleGroup.Count(); i++)
      {
        var person = peopleGroup[i];

        newSlidePart.Slide.InnerXml = newSlidePart.Slide.InnerXml
          .Replace(BASE_FIRSTNAME_TEMPLATE.ToString().Replace("\\d", (i+1).ToString()), person.FirstName)
          .Replace(BASE_LASTNAME_TEMPLATE.ToString().Replace("\\d", (i+1).ToString()), person.LastName);
      }

      if (blankUnusedBadges)
      {
        for (var i = peopleGroup.Count(); i < badgesPerPage; i++)
        {
          newSlidePart.Slide.InnerXml = newSlidePart.Slide.InnerXml
          .Replace(BASE_FIRSTNAME_TEMPLATE.ToString().Replace("\\d", (i+1).ToString()), string.Empty)
          .Replace(BASE_LASTNAME_TEMPLATE.ToString().Replace("\\d", (i+1).ToString()), string.Empty);
        }
      }

      return newSlidePart;
    }
  }
}