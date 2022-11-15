namespace NameBadgeAutomater.Tests.Services;

public class PowerPointTemplateServiceTests
{

    [Theory]
    [InlineData("OldFormatPowerPointFile.ppt", "This file seems corrupt and cannot be read. Error: File contains corrupted data.")]
    [InlineData("TemplateWithNoPresentationPart.pptx", "This file seems corrupt and cannot be read. Error: Specified part does not exist in the package.")]
    [InlineData("TemplateWithNoSlides.pptx", "This file has no slide. One slide is expected.")]
    [InlineData("TemplateWithMultipleSlides.pptx", "This file has more than one slide. One slide expected.")]
    public void DiscoverTemplates_ShouldThrow_WhenFileIsNotValid(string fileName, string expectedException)
    {
        //Arrange
        var sut = new PowerPointTemplateService();
        var fileBytes = File.ReadAllBytes(Path.Combine("TestFiles", fileName));

        //Act
        var action = () => sut.DiscoverTemplates(fileBytes);

        //Assert
        action.Should().Throw<Exception>().WithMessage(expectedException);
    }

    [Theory]
    [MemberData(nameof(DiscoverTemplatesValidTestData))]
    public void DiscoverTemplates_ShouldReturnNameTemplateResults_WhenFileIsValid(string fileName, IEnumerable<NameTemplateResult> expectedResult)
    {
        //Arrange
        var sut = new PowerPointTemplateService();
        var fileBytes = File.ReadAllBytes(Path.Combine("TestFiles", fileName));

        //Act
        var result = sut.DiscoverTemplates(fileBytes);

        //Assert
        result.Should().BeEquivalentTo(expectedResult);
    }

    [Theory]
    [MemberData(nameof(IsTemplateValidTestData))]
    public void IsTemplateValid_ShouldReturnBoolean(IEnumerable<NameTemplateResult> discoveredTemplates, bool expectedResult)
    {
        //Arrange
        var sut = new PowerPointTemplateService();

        //Act
        var result = sut.IsTemplateValid(discoveredTemplates);

        //Assert
        result.Should().Be(expectedResult);
    }

    #region Test Data

    public static IEnumerable<object[]> DiscoverTemplatesValidTestData() => new List<object[]> 
    {
        new object[] { "EightTemplatesOnce.pptx", EightTemplatesOnceResult },
        new object[] { "OneTemplateTwice.pptx", OneTemplateTwiceResult },
        new object[] { "NoFirstName.pptx", NoFirstNameResult },
        new object[] { "NoLastName.pptx", NoLastNameResult },
        new object[] { "MissingIndex.pptx", MissingIndexResult },
        new object[] { "NoTemplates.pptx", NoTemplatesResult },
    };

    public static IEnumerable<object[]> IsTemplateValidTestData() => new List<object[]> 
    {
        new object[] { EightTemplatesOnceResult, true },
        new object[] { OneTemplateTwiceResult, true },
        new object[] { NoFirstNameResult, false },
        new object[] { NoLastNameResult, false },
        new object[] { MissingIndexResult, false },
        new object[] { NoTemplatesResult, false },
    };

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

    private static IEnumerable<NameTemplateResult> OneTemplateTwiceResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 2, LastCount = 2 },
    };

    private static IEnumerable<NameTemplateResult> NoFirstNameResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 0, LastCount = 1 },
    };

    private static IEnumerable<NameTemplateResult> NoLastNameResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 1, LastCount = 0 },
    };

    private static IEnumerable<NameTemplateResult> MissingIndexResult = new List<NameTemplateResult>
    {
        new NameTemplateResult { Index = 1, FirstCount = 1, LastCount = 1 },
        new NameTemplateResult { Index = 2, FirstCount = 0, LastCount = 0 },
        new NameTemplateResult { Index = 3, FirstCount = 1, LastCount = 1 },
    };

    private static IEnumerable<NameTemplateResult> NoTemplatesResult = new List<NameTemplateResult> {};
    
    #endregion
}