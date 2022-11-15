namespace NameBadgeAutomater.Tests.Services;

public class NameParserServiceTests
{

    [Theory]
    [InlineData("First Last", "First", "Last")] // Space
    [InlineData("First	Last", "First", "Last")] // Tab
    [InlineData("Last, First", "First", "Last")] // Comma, reversed
    [InlineData("first.last@email.com", "First", "Last")] // Email
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
    public void ParseRawName_ShouldReturnNull_WhenEmptyValue(string rawName)
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
        var actualPeople = sut.ParseRawNames(rawNames);

        //Assert
        actualPeople.Should().BeEquivalentTo(expectedPeople);
    }

    #region Test Data

    public static IEnumerable<object[]> ParseRawNamesTestData() => new List<object[]> 
    {
        new object[] { SimpleRawNames, ExpectedPeople },
        new object[] { RawNamesWithEmptyLine, ExpectedPeople },
        new object[] { EmailListRawNames, ExpectedPeople },
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

    private static string EmailListRawNames = 
    """
    First1 Last1 <ignore@emial.com>; First2 Last2 <ignore@email.com>;
    """;
    
    private static List<Person> ExpectedPeople = new List<Person>{
        new Person { FirstName = "First1", LastName = "Last1" },
        new Person { FirstName = "First2", LastName = "Last2" },
    };

    #endregion
}