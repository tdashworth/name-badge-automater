using System.Text.RegularExpressions;

namespace NameBadgeAutomater;

public class NameParserService
{
  static Regex EmailListRegex = new Regex("<.*@.*>");
  static string Space = " ";
  static string Tab = "	";
  static string Comma = ",";
  static string At = "@";
  static string Period = ".";

  public List<Person> ParseRawNames(string rawNames, string companies)
  {
    if (string.IsNullOrWhiteSpace(rawNames)) return new List<Person>();

    return rawNames
      .Split(Environment.NewLine)
      .SelectMany(line => line.Split(";"))
      .Select(ParseRawName)
      .ZipLongest(companies.Split(Environment.NewLine), (person, company) =>
      {
        if (person is not null)
        {
          person.Company = company?.Trim() ?? string.Empty;
        }
        return person;
      })
      .Where(person => person is not null)
      .Cast<Person>()
      .ToList();
  }

  public Person? ParseRawName(string? rawName)
  {
    if (string.IsNullOrWhiteSpace(rawName)) return null; // No name.

    rawName = rawName.Trim();

    if (EmailListRegex.IsMatch(rawName)) // FirstName LastName(s) <*@*>, LastName(s), FirstName <*@*>
    {
      rawName = EmailListRegex.Replace(rawName, string.Empty);
    }

    if (rawName.Contains(At) && rawName.IndexOf(Period) < rawName.IndexOf(At)) // FirstName.LastName@email.com
    {
      var nameParts = rawName.Split(At).First().Split(Period);
      return new Person { FirstName = nameParts[0].Trim().ToSentenceCase(), LastName = string.Join(" ", nameParts.Skip(1).Select(StringExtensions.ToSentenceCase)) };
    }

    if (rawName.Contains(Comma)) // LastName(s), FirstName
    {
      var nameParts = rawName.Split(Comma);
      return new Person { FirstName = nameParts[1].Trim().ToSentenceCase(), LastName = nameParts[0].Trim().ToSentenceCase() };
    }

    if (rawName.Contains(Tab)) // FirstName	LastName(s) (tab)
    {
      var nameParts = rawName.Split(Tab, 2, StringSplitOptions.RemoveEmptyEntries);
      return new Person { FirstName = nameParts[0].Trim().ToSentenceCase(), LastName = nameParts[1].Trim().ToSentenceCase() };
    }

    if (rawName.Contains(Space)) // FirstName LastName(s)
    {
      var nameParts = rawName.Split(Space, 2, StringSplitOptions.RemoveEmptyEntries);
      return new Person { FirstName = nameParts[0].Trim().ToSentenceCase(), LastName = nameParts[1].Trim().ToSentenceCase() };
    }

    return new Person { FirstName = rawName };
  }
}