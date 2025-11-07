using System.ComponentModel.DataAnnotations;
using JobApplicationTrackerV2.Models;

namespace JobApplicationTrackerV2.Tests;

public class JobApplicationModelTests
{
    [Fact]
    public void JobApplication_DefaultConstructor_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var jobApp = new JobApplication();

        // Assert
        jobApp.Id.Should().Be(0);
        jobApp.Foretag.Should().Be(string.Empty);
        jobApp.Jobbtitel.Should().Be(string.Empty);
        jobApp.AnsokanDatum.Should().Be(DateTime.Today);
        jobApp.Status.Should().Be(ApplicationStatus.VantarPaSvar);
        jobApp.UserId.Should().Be(string.Empty);
    }

    [Fact]
    public void JobApplication_WithValidData_ShouldPassValidation()
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "Microsoft",
            Jobbtitel = "Software Engineer",
            Plats = "Stockholm",
            AnsokanDatum = DateTime.Today,
            Status = ApplicationStatus.VantarPaSvar,
            UserId = "test-user-123",
            Anteckningar = "Bra företag",
            Url = "https://careers.microsoft.com/job123"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void JobApplication_WithoutForetag_ShouldFailValidation(string? foretag)
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = foretag!,
            Jobbtitel = "Developer",
            UserId = "test-user"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("Företag är obligatoriskt");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void JobApplication_WithoutJobbtitel_ShouldFailValidation(string? jobbtitel)
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = jobbtitel!,
            UserId = "test-user"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("Jobbtitel är obligatoriskt");
    }

    [Fact]
    public void JobApplication_WithTooLongForetag_ShouldFailValidation()
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = new string('A', 201), // 201 characters (max is 200)
            Jobbtitel = "Developer",
            UserId = "test-user"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void JobApplication_WithTooLongJobbtitel_ShouldFailValidation()
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = new string('A', 201), // 201 characters (max is 200)
            UserId = "test-user"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Fact]
    public void JobApplication_WithInvalidUrl_ShouldFailValidation()
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = "Developer",
            UserId = "test-user",
            Url = "inte-en-giltig-url"
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().ContainSingle()
            .Which.ErrorMessage.Should().Contain("giltig URL");
    }

    [Theory]
    [InlineData("https://example.com")]
    [InlineData("http://jobs.example.com/job/123")]
    [InlineData("https://careers.company.se/position")]
    public void JobApplication_WithValidUrl_ShouldPassValidation(string url)
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = "Developer",
            UserId = "test-user",
            Url = url
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void JobApplication_WithTooLongAnteckningar_ShouldFailValidation()
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = "Developer",
            UserId = "test-user",
            Anteckningar = new string('A', 1001) // 1001 characters (max is 1000)
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData(ApplicationStatus.VantarPaSvar)]
    [InlineData(ApplicationStatus.Nej)]
    [InlineData(ApplicationStatus.Ja)]
    [InlineData(ApplicationStatus.GattVideare)]
    [InlineData(ApplicationStatus.Intervju)]
    [InlineData(ApplicationStatus.Avbruten)]
    public void JobApplication_WithAllStatusValues_ShouldBeValid(ApplicationStatus status)
    {
        // Arrange
        var jobApp = new JobApplication
        {
            Foretag = "TestCo",
            Jobbtitel = "Developer",
            UserId = "test-user",
            Status = status
        };

        // Act
        var validationResults = ValidateModel(jobApp);

        // Assert
        validationResults.Should().BeEmpty();
    }

    [Fact]
    public void JobApplication_CanSetAllProperties_Successfully()
    {
        // Arrange
        var expectedDate = new DateTime(2025, 1, 15);
        var jobApp = new JobApplication();

        // Act
        jobApp.Id = 1;
        jobApp.Foretag = "Google";
        jobApp.Jobbtitel = "Senior Developer";
        jobApp.Plats = "Göteborg";
        jobApp.AnsokanDatum = expectedDate;
        jobApp.Status = ApplicationStatus.Intervju;
        jobApp.Anteckningar = "Intressant roll";
        jobApp.Url = "https://careers.google.com";
        jobApp.UserId = "user123";

        // Assert
        jobApp.Id.Should().Be(1);
        jobApp.Foretag.Should().Be("Google");
        jobApp.Jobbtitel.Should().Be("Senior Developer");
        jobApp.Plats.Should().Be("Göteborg");
        jobApp.AnsokanDatum.Should().Be(expectedDate);
        jobApp.Status.Should().Be(ApplicationStatus.Intervju);
        jobApp.Anteckningar.Should().Be("Intressant roll");
        jobApp.Url.Should().Be("https://careers.google.com");
        jobApp.UserId.Should().Be("user123");
    }

    private static List<ValidationResult> ValidateModel(object model)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(model, null, null);
        Validator.TryValidateObject(model, validationContext, validationResults, true);
        return validationResults;
    }
}
