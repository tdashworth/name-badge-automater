namespace NameBadgeAutomater.Tests.Services;

public class AppStateTests
{

    [Fact]
    public void CompanySupported_ShouldBeFalseWhenNoCompanyPlaceholders()
    {
        //Arrange
        var appState = new AppState();

        //Act 
        appState.DiscoveredPlaceholders = EightTemplatesOnceResult;

        //Assert
        appState.CompanySupported.Should().BeFalse();
    }

    [Fact]
    public void CompanySupported_ShouldBeTrueWhenAllCompanyPlaceholders()
    {
        //Arrange
        var appState = new AppState();

        //Act 
        appState.DiscoveredPlaceholders = EightTemplatesOnceWithCompaniesResult;

        //Assert
        appState.CompanySupported.Should().BeTrue();
    }

    [Fact]
    public void CompanySupported_ShouldBeTrueWhenAnyCompanyPlaceholders()
    {
        //Arrange
        var appState = new AppState();

        //Act 
        appState.DiscoveredPlaceholders = EightTemplatesOnceWithOneCompanyResult;

        //Assert
        appState.CompanySupported.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(SimpleRawNames, SimpleRawCompanies)]
    [InlineData(RawNamesWithEmptyLine, RawCompaniesWithEmptyLine)]
    [InlineData(EmailListRawNames, SimpleRawCompanies)]
    public void NamesAndCompaniesCountMatch_ShouldBeTrueWhenCountsMatch(string rawNames, string rawCompanies)
    {
        //Arrange
        var appState = new AppState();

        //Act 
        appState.RawNames = rawNames;
        appState.RawCompanies = rawCompanies;

        //Assert
        appState.NamesAndCompaniesCountMatch.Should().BeTrue();
    }

    [Theory]
    [InlineData(SimpleRawNames, RawCompaniesWithEmptyLine)]
    [InlineData(RawNamesWithEmptyLine, SimpleRawCompanies)]
    [InlineData(EmailListRawNames, RawCompaniesWithEmptyLine)]
    public void NamesAndCompaniesCountMatch_ShouldBeFalseWhenCountsDoNotMatch(string rawNames, string rawCompanies)
    {
        //Arrange
        var appState = new AppState();

        //Act 
        appState.RawNames = rawNames;
        appState.RawCompanies = rawCompanies;

        //Assert
        appState.NamesAndCompaniesCountMatch.Should().BeFalse();
    }

    #region Test Data
    private static IEnumerable<NameTemplateResult> EightTemplatesOnceResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 2, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 3, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 4, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 5, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 6, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 7, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 8, FirstCount = 1, LastCount = 1 },
    };

    private static IEnumerable<NameTemplateResult> EightTemplatesOnceWithOneCompanyResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 2, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 3, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 4, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 5, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 6, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 7, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 8, FirstCount = 1, LastCount = 1 },
    };

    private static IEnumerable<NameTemplateResult> EightTemplatesOnceWithCompaniesResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 2, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 3, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 4, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 5, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 6, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 7, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
        new NameTemplateResult { Index = 8, FirstCount = 1, LastCount = 1, CompanyCount = 1 },
    };

    private const string SimpleRawNames =
    """
    First1 Last1
    First2 Last2
    """;
    private const string RawNamesWithEmptyLine =
    """
    First1 Last1

    First2 Last2

    """;

    private const string EmailListRawNames =
    """
    First1 Last1 <ignore@email.com>; First2 Last2 <ignore@email.com>;
    """;

    private const string SimpleRawCompanies =
    """
    Company1
    Company2
    """;
    private const string RawCompaniesWithEmptyLine =
    """
    Company1

    Company2

    """;

    #endregion
}