using System.Xml.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Validation;
using FluentAssertions.Execution;

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

    public static IEnumerable<object[]> DiscoverTemplatesValidTestData() => new List<object[]> 
    {
        new object[] { "EightTemplatesOnce.pptx", EightTemplatesOnceResult },
        new object[] { "EightTemplatesOnceWithCompanies.pptx", EightTemplatesOnceWithCompaniesResult },
        new object[] { "OneTemplateTwice.pptx", OneTemplateTwiceResult },
        new object[] { "NoFirstName.pptx", NoFirstNameResult },
        new object[] { "NoLastName.pptx", NoLastNameResult },
        new object[] { "MissingIndex.pptx", MissingIndexResult },
        new object[] { "NoTemplates.pptx", NoTemplatesResult },
    };

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

    public static IEnumerable<object[]> IsTemplateValidTestData() => new List<object[]> 
    {
        new object[] { EightTemplatesOnceResult, true },
        new object[] { EightTemplatesOnceWithCompaniesResult, true },
        new object[] { OneTemplateTwiceResult, true },
        new object[] { NoFirstNameResult, false },
        new object[] { NoLastNameResult, false },
        new object[] { MissingIndexResult, false },
        new object[] { NoTemplatesResult, false },
    };
    
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

    public static IEnumerable<object[]> GenerateFromTemplateTestData() => new List<object[]> 
    {
        new object[] { "EightTemplatesOnce.pptx", 8, People.Take(8), true, "Result_EightByEight.pptx" },
        new object[] { "EightTemplatesOnceWithCompanies.pptx", 8, People.Take(8), true, "Result_EightByEightWithCompanies.pptx" },
        new object[] { "EightTemplatesOnce.pptx", 8, People.Take(10), true, "Result_EightByTen.pptx" },
        new object[] { "OneTemplateTwice.pptx", 1, People.Take(1), true, "Result_OneByOne.pptx" },
        new object[] { "OneTemplateTwice.pptx", 1, People.Take(2), true, "Result_OneByTwo.pptx" },
        new object[] { "DifferentCasedNames.pptx", 6, People.Take(6), true, "Result_DifferentCasedNames.pptx" },
    };

    [Theory]
    [MemberData(nameof(GenerateFromTemplateTestData))]
    public void GenerateFromTemplate_ShouldGenerateNewFile(string templateFileName, int badgesPerPage, IEnumerable<Person> people, bool removeBlanks, string expectedFileName)
    {
        //Arrange
        var sut = new PowerPointTemplateService();
        var templateFileBytes = File.ReadAllBytes(Path.Combine("TestFiles", templateFileName));

        //Act
        var result = sut.GenerateFromTemplate(templateFileBytes, people.ToList(), badgesPerPage, removeBlanks);
        var resultDocument = PresentationDocument.Open(new MemoryStream(result), true);

        //Assert
        var validationResult = OpenXmlValidator.Validate(resultDocument);
        validationResult.Should().BeEmpty();

        var expectedFileBytes = File.ReadAllBytes(Path.Combine("TestFiles", expectedFileName));
        var expectedDocument = PresentationDocument.Open(new MemoryStream(expectedFileBytes), true);
        for (int i = 0; i < expectedDocument.PresentationPart!.SlideParts.Count(); i++)
        {
            var resultSlide = resultDocument.PresentationPart!.SlideParts.Skip(i).FirstOrDefault();
            var expectedSlide = expectedDocument.PresentationPart!.SlideParts.Skip(i).FirstOrDefault();

            resultSlide.Should().NotBeNull();
            XDocument.Parse(resultSlide!.Slide.OuterXml).Should().BeEquivalentTo(XDocument.Parse(expectedSlide!.Slide.OuterXml));
        }

        // TODO More assertions would be useful...
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

    private static List<Person> People = new List<Person> {
        new Person { FirstName = "First1", LastName = "Last1", Company = "Company1" },
        new Person { FirstName = "First2", LastName = "Last2", Company = "Company2" },
        new Person { FirstName = "First3", LastName = "Last3", Company = "Company3" },
        new Person { FirstName = "First4", LastName = "Last4", Company = "Company4" },
        new Person { FirstName = "First5", LastName = "Last5", Company = "Company5" },
        new Person { FirstName = "First6", LastName = "Last6", Company = "Company6" },
        new Person { FirstName = "First7", LastName = "Last7", Company = "Company7" },
        new Person { FirstName = "First8", LastName = "Last8", Company = "Company8" },
        new Person { FirstName = "First9", LastName = "Last9", Company = "Company9" },
        new Person { FirstName = "First10", LastName = "Last10", Company = "Company10" },
    };

    #endregion

    #region Helpers
    private static OpenXmlValidator OpenXmlValidator = new OpenXmlValidator();
    #endregion
}