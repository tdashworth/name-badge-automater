using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

public static class OpenXmlUtils
{
  public static IEnumerable<SlidePart> GetSlidePartsInOrder(this PresentationPart presentationPart)
  {
    SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

    return slideIdList.ChildElements
        .Cast<SlideId>()
        .Select(x => presentationPart.GetPartById(x.RelationshipId))
        .Cast<SlidePart>();
  }

  public static SlidePart CloneSlide(this SlidePart templatePart)
  {
    // find the presentationPart: makes the API more fluent
    var presentationPart = templatePart.GetParentParts()
        .OfType<PresentationPart>()
        .Single();

    // clone slide contents
    Slide currentSlide = (Slide)templatePart.Slide.CloneNode(true);
    var slidePartClone = presentationPart.AddNewPart<SlidePart>();
    currentSlide.Save(slidePartClone);

    // copy layout part
    slidePartClone.AddPart(templatePart.SlideLayoutPart);

    foreach (var image in templatePart.ImageParts)
    {
      slidePartClone.AddPart(image, templatePart.GetIdOfPart(image));
    }

    return slidePartClone;
  }

  public static void AppendSlide(this PresentationPart presentationPart, SlidePart newSlidePart)
  {
    SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

    // find the highest id
    uint maxSlideId = slideIdList.ChildElements
        .Cast<SlideId>()
        .Max(x => x.Id.Value);

    // Insert the new slide into the slide list after the previous slide.
    var id = maxSlideId + 1;

    SlideId newSlideId = new SlideId();
    slideIdList.Append(newSlideId);
    newSlideId.Id = id;
    newSlideId.RelationshipId = presentationPart.GetIdOfPart(newSlidePart);
  }

  public static void RemoveSlide(this PresentationPart presentationPart, int index)
  {
    SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

    var slideId = slideIdList.ChildElements[index] as SlideId;
    slideIdList.RemoveChild(slideId);

    if (presentationPart.Presentation.CustomShowList != null)
    {
      // Iterate through the list of custom shows.
      foreach (var customShow in presentationPart.Presentation.CustomShowList.Elements<CustomShow>())
      {
        if (customShow.SlideList != null)
        {
          // Declare a link list of slide list entries.
          LinkedList<SlideListEntry> slideListEntries = new LinkedList<SlideListEntry>();
          foreach (SlideListEntry slideListEntry in customShow.SlideList.Elements())
          {
            // Find the slide reference to remove from the custom show.
            if (slideListEntry.Id != null && slideListEntry.Id == slideId.RelationshipId)
            {
              slideListEntries.AddLast(slideListEntry);
            }
          }

          // Remove all references to the slide from the custom show.
          foreach (SlideListEntry slideListEntry in slideListEntries)
          {
            customShow.SlideList.RemoveChild(slideListEntry);
          }
        }
      }
    }

    // Remove the slide part.
    SlidePart slidePart = presentationPart.GetPartById(slideId.RelationshipId) as SlidePart;
    presentationPart.DeletePart(slidePart);
  }
}