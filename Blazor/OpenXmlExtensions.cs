using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;

public static class OpenXmlExtensions
{
  public static IEnumerable<SlidePart> GetSlidePartsInOrder(this PresentationPart? presentationPart)
  {
    if (presentationPart is null) throw new Exception("Invalid document provided: no presentation part.");

    var slideIdList = presentationPart.Presentation?.SlideIdList;
    if (slideIdList is null) throw new Exception("Invalid document provided: No slide ids list.");

    return slideIdList.ChildElements
        .Cast<SlideId>()
        .Select(x => presentationPart.GetPartById(x.RelationshipId!))
        .Cast<SlidePart>();
  }

  public static SlidePart CloneSlide(this SlidePart templatePart)
  {
    if (templatePart is null) throw new Exception("Invalid document: no slide part.");
    if (templatePart.SlideLayoutPart is null) throw new Exception("Invalid document: no slide layout part.");

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
    if (presentationPart is null) throw new Exception("Invalid document: no presentation part.");
    if (newSlidePart is null) throw new Exception("Invalid document: no slide part to append.");

    var slideIdList = presentationPart.Presentation?.SlideIdList;
    if (slideIdList is null) throw new Exception("Invalid document provided: No slide ids list.");

    // find the highest id
    uint maxSlideId = slideIdList.ChildElements
        .Cast<SlideId>()
        .Max(x => x.Id!.Value);

    // Insert the new slide into the slide list after the previous slide.
    var id = maxSlideId + 1;

    SlideId newSlideId = new SlideId();
    slideIdList.Append(newSlideId);
    newSlideId.Id = id;
    newSlideId.RelationshipId = presentationPart.GetIdOfPart(newSlidePart);
  }

  public static void RemoveSlide(this PresentationPart presentationPart, int index)
  {
    if (presentationPart is null) throw new Exception("Invalid document: no presentation part.");

    var slideIdList = presentationPart.Presentation.SlideIdList;
    if (slideIdList is null) throw new Exception("Invalid document provided: No slide ids list.");

    var slideId = slideIdList.ChildElements[index] as SlideId;
    if (slideId is null) throw new Exception($"Invalid document: no slide with index {index}.");
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
    var slidePart = presentationPart.GetPartById(slideId.RelationshipId!) as SlidePart;
    presentationPart.DeletePart(slidePart!);
  }
}