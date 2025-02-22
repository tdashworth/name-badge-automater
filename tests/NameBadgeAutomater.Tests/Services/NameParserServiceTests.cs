namespace NameBadgeAutomater.Tests.Services;

public class NameParserServiceTests
{

    [Theory]
    [InlineData("First Last", "First", "Last")] // Space
    [InlineData("First	Last", "First", "Last")] // Tab
    [InlineData("Last, First", "First", "Last")] // Comma, reversed
    [InlineData("first.last@email.com", "First", "Last")] // Email
    [InlineData("first.m.last@email.com", "First", "M Last")] // Email, middle initial
    [InlineData("first@email.com", "first@email.com", "")] // Email, no period
    [InlineData("First Last <first.last@email.com>", "First", "Last")] // Email List, space
    [InlineData("Last, First <first.last@email.com>", "First", "Last")] // Email List, comma
    [InlineData("First Middle Last", "First", "Middle Last")] // Space
    [InlineData("Middle Last, First", "First", "Middle Last")] // Comma, reversed
    [InlineData("   First Last", "First", "Last")] // Whitespace before
    [InlineData("First Last   ", "First", "Last")] // Whitespace after
    [InlineData("First    Last", "First", "Last")] // Whitespace middle
    [InlineData("First", "First", "")] // First name only
    public void ParseRawName_ShouldReturnCorrectNames(string rawName, string firstName, string lastName)
    {
        //Arrange
        var sut = new NameParserService();

        //Act 
        var actualPerson = sut.ParseRawName(rawName);

        //Assert
        actualPerson.Should().NotBeNull();
        actualPerson!.FirstName.Should().Be(firstName);
        actualPerson!.LastName.Should().Be(lastName);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void ParseRawName_ShouldReturnNull_WhenEmptyValue(string? rawName)
    {
        //Arrange
        var sut = new NameParserService();

        //Act 
        var actualPerson = sut.ParseRawName(rawName);

        //Assert
        actualPerson.Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(ParseRawNamesTestData))]
    public void ParseRawNames_ShouldReturnCorrectListOfPeople(string rawNames, List<Person> expectedPeople)
    {
        //Arrange
        var sut = new NameParserService();

        //Act 
        var actualPeople = sut.ParseRawNames(rawNames, string.Empty);

        //Assert
        actualPeople.Should().BeEquivalentTo(expectedPeople);
    }

    [Theory]
    [MemberData(nameof(ParseRawNamesTestDataWithCompanies))]
    public void ParseRawNames_ShouldReturnCorrectListOfPeopleWithCompanies(string rawNames, string rawCompanies, List<Person> expectedPeople)
    {
        //Arrange
        var sut = new NameParserService();

        //Act 
        var actualPeople = sut.ParseRawNames(rawNames, rawCompanies);

        //Assert
        actualPeople.Should().BeEquivalentTo(expectedPeople);
    }

    #region Test Data

    public static IEnumerable<object[]> ParseRawNamesTestData() => new List<object[]> 
    {
        new object[] { SimpleRawNames, ExpectedPeople },
        new object[] { RawNamesWithEmptyLine, ExpectedPeople },
        new object[] { EmailListRawNames, ExpectedPeople },
        new object[] { FormatedEmailListRawNames, ExpectedPeople },
    };

    public static IEnumerable<object[]> ParseRawNamesTestDataWithCompanies() => new List<object[]> 
    {
        new object[] { SimpleRawNames, SimpleRawCompanies, ExpectedPeopleAllWithCompanies },
        new object[] { RawNamesWithEmptyLine, RawCompaniesWithEmptyLine, ExpectedPeopleAllWithCompanies },
        new object[] { SimpleRawNames, "Company1", ExpectedPeopleOneWithCompanies },
    };

    private static string SimpleRawNames = 
    """
    First1 Last1
    First2 Last2
    """;
    private static string RawNamesWithEmptyLine = 
    """
    First1 Last1

    First2 Last2

    """;

    private static string FormatedEmailListRawNames = 
    """
    First1 Last1 <ignore@email.com>; First2 Last2 <ignore@email.com>;
    """;

    private static string EmailListRawNames = 
    """
    first1.last1@email.com; first2.last2@email.com;
    """;

    private static string SimpleRawCompanies = 
    """
    Company1
    Company2
    """;
    private static string RawCompaniesWithEmptyLine = 
    """
    Company1

    Company2

    """;
    
    private static List<Person> ExpectedPeople = new List<Person>{
        new Person { FirstName = "First1", LastName = "Last1" },
        new Person { FirstName = "First2", LastName = "Last2" },
    };

    private static List<Person> ExpectedPeopleAllWithCompanies = new List<Person>{
        new Person { FirstName = "First1", LastName = "Last1", Company= "Company1" },
        new Person { FirstName = "First2", LastName = "Last2", Company= "Company2" },
    };

    private static List<Person> ExpectedPeopleOneWithCompanies = new List<Person>{
        new Person { FirstName = "First1", LastName = "Last1", Company= "Company1" },
        new Person { FirstName = "First2", LastName = "Last2" },
    };

    #endregion
}