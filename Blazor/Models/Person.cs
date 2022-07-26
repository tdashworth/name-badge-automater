namespace NameBadgeAutomater
{
  public class Person
  {
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public static Person? FromString(string rawName)
    {
      if (string.IsNullOrWhiteSpace(rawName)) return null; // No name.

      if (rawName.Contains("@"))
      {
        var nameParts = rawName.Split("@").First().Split(".");
        return new Person { FirstName = nameParts[0].Trim().ToSentenceCase(), LastName = nameParts[1].Trim().ToSentenceCase() };
      }

      if (rawName.Contains(","))
      {
        var nameParts = rawName.Split(",");
        return new Person { FirstName = nameParts[1].Trim().ToSentenceCase(), LastName = nameParts[0].Trim().ToSentenceCase() };
      }

      if (rawName.Contains(" "))
      {
        var nameParts = rawName.Split(" ");
        return new Person { FirstName = nameParts[0].Trim().ToSentenceCase(), LastName = nameParts[1].Trim().ToSentenceCase() };
      }

      return null;
    }
  }
}