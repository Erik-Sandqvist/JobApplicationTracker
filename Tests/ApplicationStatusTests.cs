using JobApplicationTrackerV2.Models;

namespace JobApplicationTrackerV2.Tests;

public class ApplicationStatusTests
{
    [Fact]
    public void ApplicationStatus_ShouldHaveAllExpectedValues()
    {
        // Arrange & Act
        var statusValues = Enum.GetValues<ApplicationStatus>();

        // Assert
        statusValues.Should().HaveCount(6);
        statusValues.Should().Contain(ApplicationStatus.VantarPaSvar);
        statusValues.Should().Contain(ApplicationStatus.Nej);
        statusValues.Should().Contain(ApplicationStatus.Ja);
        statusValues.Should().Contain(ApplicationStatus.GattVideare);
        statusValues.Should().Contain(ApplicationStatus.Intervju);
        statusValues.Should().Contain(ApplicationStatus.Avbruten);
    }

    [Theory]
    [InlineData(ApplicationStatus.VantarPaSvar, 0)]
    [InlineData(ApplicationStatus.Nej, 1)]
    [InlineData(ApplicationStatus.Ja, 2)]
    [InlineData(ApplicationStatus.GattVideare, 3)]
    [InlineData(ApplicationStatus.Intervju, 4)]
    [InlineData(ApplicationStatus.Avbruten, 5)]
    public void ApplicationStatus_ShouldHaveCorrectIntegerValue(ApplicationStatus status, int expectedValue)
    {
        // Act
        var actualValue = (int)status;

        // Assert
        actualValue.Should().Be(expectedValue);
    }

    [Theory]
    [InlineData(0, ApplicationStatus.VantarPaSvar)]
    [InlineData(1, ApplicationStatus.Nej)]
    [InlineData(2, ApplicationStatus.Ja)]
    [InlineData(3, ApplicationStatus.GattVideare)]
    [InlineData(4, ApplicationStatus.Intervju)]
    [InlineData(5, ApplicationStatus.Avbruten)]
    public void ApplicationStatus_CanConvertFromInteger(int value, ApplicationStatus expectedStatus)
    {
        // Act
        var status = (ApplicationStatus)value;

        // Assert
        status.Should().Be(expectedStatus);
    }

    [Fact]
    public void ApplicationStatus_VantarPaSvar_ShouldBeDefaultValue()
    {
        // Arrange & Act
        var defaultStatus = default(ApplicationStatus);

        // Assert
        defaultStatus.Should().Be(ApplicationStatus.VantarPaSvar);
    }

    [Theory]
    [InlineData(ApplicationStatus.VantarPaSvar, "VantarPaSvar")]
    [InlineData(ApplicationStatus.Nej, "Nej")]
    [InlineData(ApplicationStatus.Ja, "Ja")]
    [InlineData(ApplicationStatus.GattVideare, "GattVideare")]
    [InlineData(ApplicationStatus.Intervju, "Intervju")]
    [InlineData(ApplicationStatus.Avbruten, "Avbruten")]
    public void ApplicationStatus_ToString_ShouldReturnCorrectName(ApplicationStatus status, string expectedName)
    {
        // Act
        var statusName = status.ToString();

        // Assert
        statusName.Should().Be(expectedName);
    }

    [Theory]
    [InlineData("VantarPaSvar", ApplicationStatus.VantarPaSvar)]
    [InlineData("Nej", ApplicationStatus.Nej)]
    [InlineData("Ja", ApplicationStatus.Ja)]
    [InlineData("GattVideare", ApplicationStatus.GattVideare)]
    [InlineData("Intervju", ApplicationStatus.Intervju)]
    [InlineData("Avbruten", ApplicationStatus.Avbruten)]
    public void ApplicationStatus_CanParseFromString(string statusString, ApplicationStatus expectedStatus)
    {
        // Act
        var success = Enum.TryParse<ApplicationStatus>(statusString, out var status);

        // Assert
        success.Should().BeTrue();
        status.Should().Be(expectedStatus);
    }

    [Fact]
    public void ApplicationStatus_InvalidValue_ShouldNotParse()
    {
        // Act
        var success = Enum.TryParse<ApplicationStatus>("InvalidStatus", out var status);

        // Assert
        success.Should().BeFalse();
        status.Should().Be(default(ApplicationStatus));
    }
}
