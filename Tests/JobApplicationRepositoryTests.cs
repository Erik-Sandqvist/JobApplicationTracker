using JobApplicationTrackerV2.Data;
using JobApplicationTrackerV2.Models;
using Microsoft.EntityFrameworkCore;

namespace JobApplicationTrackerV2.Tests;

public class JobApplicationRepositoryTests
{
    private ApplicationDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
      .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task CanAddJobApplicationToDatabase()
    {
        // Arrange
    using var context = CreateInMemoryContext();
     var jobApp = new JobApplication
  {
  Foretag = "Microsoft",
        Jobbtitel = "Software Engineer",
   Plats = "Stockholm",
    AnsokanDatum = DateTime.Today,
       Status = ApplicationStatus.VantarPaSvar,
            UserId = "test-user-123"
     };

      // Act
        context.JobApplications.Add(jobApp);
      await context.SaveChangesAsync();

  // Assert
        var savedJobApp = await context.JobApplications.FirstOrDefaultAsync();
        savedJobApp.Should().NotBeNull();
   savedJobApp!.Foretag.Should().Be("Microsoft");
        savedJobApp.Jobbtitel.Should().Be("Software Engineer");
    }

    [Fact]
    public async Task CanRetrieveJobApplicationById()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var jobApp = new JobApplication
     {
         Foretag = "Google",
            Jobbtitel = "DevOps Engineer",
 UserId = "test-user-123"
      };
        context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

        // Act
        var retrievedJobApp = await context.JobApplications.FindAsync(jobApp.Id);

        // Assert
        retrievedJobApp.Should().NotBeNull();
        retrievedJobApp!.Id.Should().Be(jobApp.Id);
        retrievedJobApp.Foretag.Should().Be("Google");
    }

    [Fact]
    public async Task CanUpdateJobApplication()
    {
      // Arrange
        using var context = CreateInMemoryContext();
        var jobApp = new JobApplication
        {
     Foretag = "Amazon",
            Jobbtitel = "Cloud Engineer",
            Status = ApplicationStatus.VantarPaSvar,
         UserId = "test-user-123"
        };
        context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

        // Act
jobApp.Status = ApplicationStatus.Intervju;
   jobApp.Anteckningar = "Fick intervjutid!";
   context.JobApplications.Update(jobApp);
   await context.SaveChangesAsync();

     // Assert
        var updatedJobApp = await context.JobApplications.FindAsync(jobApp.Id);
        updatedJobApp.Should().NotBeNull();
        updatedJobApp!.Status.Should().Be(ApplicationStatus.Intervju);
   updatedJobApp.Anteckningar.Should().Be("Fick intervjutid!");
 }

    [Fact]
    public async Task CanDeleteJobApplication()
    {
   // Arrange
        using var context = CreateInMemoryContext();
        var jobApp = new JobApplication
 {
            Foretag = "Netflix",
     Jobbtitel = "Frontend Developer",
    UserId = "test-user-123"
     };
 context.JobApplications.Add(jobApp);
   await context.SaveChangesAsync();
    var jobAppId = jobApp.Id;

        // Act
        context.JobApplications.Remove(jobApp);
        await context.SaveChangesAsync();

      // Assert
        var deletedJobApp = await context.JobApplications.FindAsync(jobAppId);
  deletedJobApp.Should().BeNull();
    }

    [Fact]
    public async Task CanFilterJobApplicationsByUserId()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var user1Id = "user-1";
        var user2Id = "user-2";

      context.JobApplications.AddRange(
     new JobApplication { Foretag = "Company1", Jobbtitel = "Job1", UserId = user1Id },
      new JobApplication { Foretag = "Company2", Jobbtitel = "Job2", UserId = user1Id },
            new JobApplication { Foretag = "Company3", Jobbtitel = "Job3", UserId = user2Id }
        );
await context.SaveChangesAsync();

    // Act
        var user1Applications = await context.JobApplications
 .Where(j => j.UserId == user1Id)
   .ToListAsync();

        // Assert
  user1Applications.Should().HaveCount(2);
        user1Applications.Should().OnlyContain(j => j.UserId == user1Id);
    }

    [Fact]
    public async Task CanOrderJobApplicationsByDate()
    {
        // Arrange
        using var context = CreateInMemoryContext();
var userId = "test-user";
        var today = DateTime.Today;

        context.JobApplications.AddRange(
 new JobApplication 
            { 
     Foretag = "Company1", 
   Jobbtitel = "Job1", 
     UserId = userId, 
    AnsokanDatum = today.AddDays(-5) 
         },
            new JobApplication 
 { 
       Foretag = "Company2", 
         Jobbtitel = "Job2", 
        UserId = userId, 
            AnsokanDatum = today 
 },
            new JobApplication 
       { 
    Foretag = "Company3", 
      Jobbtitel = "Job3", 
  UserId = userId, 
                AnsokanDatum = today.AddDays(-10) 
    }
        );
  await context.SaveChangesAsync();

        // Act
        var orderedApplications = await context.JobApplications
         .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.AnsokanDatum)
        .ToListAsync();

 // Assert
    orderedApplications.Should().HaveCount(3);
        orderedApplications[0].AnsokanDatum.Should().Be(today);
        orderedApplications[1].AnsokanDatum.Should().Be(today.AddDays(-5));
        orderedApplications[2].AnsokanDatum.Should().Be(today.AddDays(-10));
    }

    [Fact]
    public async Task CanFilterJobApplicationsByStatus()
    {
        // Arrange
        using var context = CreateInMemoryContext();
      var userId = "test-user";

   context.JobApplications.AddRange(
        new JobApplication 
 { 
          Foretag = "Company1", 
   Jobbtitel = "Job1", 
           UserId = userId, 
        Status = ApplicationStatus.VantarPaSvar 
            },
            new JobApplication 
{ 
         Foretag = "Company2", 
    Jobbtitel = "Job2", 
    UserId = userId, 
         Status = ApplicationStatus.Intervju 
   },
    new JobApplication 
     { 
        Foretag = "Company3", 
          Jobbtitel = "Job3", 
 UserId = userId, 
              Status = ApplicationStatus.VantarPaSvar 
      }
        );
        await context.SaveChangesAsync();

        // Act
     var waitingApplications = await context.JobApplications
            .Where(j => j.UserId == userId && j.Status == ApplicationStatus.VantarPaSvar)
         .ToListAsync();

    // Assert
  waitingApplications.Should().HaveCount(2);
        waitingApplications.Should().OnlyContain(j => j.Status == ApplicationStatus.VantarPaSvar);
    }

    [Fact]
    public async Task CanCountJobApplicationsPerStatus()
    {
      // Arrange
using var context = CreateInMemoryContext();
        var userId = "test-user";

        context.JobApplications.AddRange(
  new JobApplication { Foretag = "C1", Jobbtitel = "J1", UserId = userId, Status = ApplicationStatus.VantarPaSvar },
       new JobApplication { Foretag = "C2", Jobbtitel = "J2", UserId = userId, Status = ApplicationStatus.VantarPaSvar },
       new JobApplication { Foretag = "C3", Jobbtitel = "J3", UserId = userId, Status = ApplicationStatus.Intervju },
      new JobApplication { Foretag = "C4", Jobbtitel = "J4", UserId = userId, Status = ApplicationStatus.Nej }
        );
        await context.SaveChangesAsync();

        // Act
   var statusCounts = await context.JobApplications
    .Where(j => j.UserId == userId)
            .GroupBy(j => j.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
 .ToListAsync();

// Assert
        statusCounts.Should().HaveCount(3);
   statusCounts.First(s => s.Status == ApplicationStatus.VantarPaSvar).Count.Should().Be(2);
        statusCounts.First(s => s.Status == ApplicationStatus.Intervju).Count.Should().Be(1);
        statusCounts.First(s => s.Status == ApplicationStatus.Nej).Count.Should().Be(1);
    }

    [Fact]
    public async Task CanSearchJobApplicationsByCompanyName()
    {
        // Arrange
     using var context = CreateInMemoryContext();
   var userId = "test-user";

        context.JobApplications.AddRange(
         new JobApplication { Foretag = "Microsoft", Jobbtitel = "Developer", UserId = userId },
   new JobApplication { Foretag = "Google", Jobbtitel = "Engineer", UserId = userId },
      new JobApplication { Foretag = "Facebook", Jobbtitel = "Developer", UserId = userId }
        );
 await context.SaveChangesAsync();

        // Act
    var searchResults = await context.JobApplications
    .Where(j => j.UserId == userId && j.Foretag.Contains("o"))
     .ToListAsync();

 // Assert
   searchResults.Should().HaveCount(3); // Microsoft, Google, and Facebook
        searchResults.Should().Contain(j => j.Foretag == "Microsoft");
        searchResults.Should().Contain(j => j.Foretag == "Google");
     searchResults.Should().Contain(j => j.Foretag == "Facebook");
    }

    [Fact]
  public async Task MultipleUsersCanHaveSeparateJobApplications()
    {
        // Arrange
        using var context = CreateInMemoryContext();
  var user1 = "user-1";
        var user2 = "user-2";

        context.JobApplications.AddRange(
         new JobApplication { Foretag = "CompanyA", Jobbtitel = "JobA", UserId = user1 },
     new JobApplication { Foretag = "CompanyB", Jobbtitel = "JobB", UserId = user2 }
        );
     await context.SaveChangesAsync();

        // Act
        var user1Apps = await context.JobApplications.Where(j => j.UserId == user1).ToListAsync();
  var user2Apps = await context.JobApplications.Where(j => j.UserId == user2).ToListAsync();

     // Assert
        user1Apps.Should().HaveCount(1);
        user2Apps.Should().HaveCount(1);
        user1Apps[0].Foretag.Should().Be("CompanyA");
      user2Apps[0].Foretag.Should().Be("CompanyB");
    }

    [Fact]
    public async Task CanStoreJobApplicationWithAllOptionalFields()
    {
        // Arrange
        using var context = CreateInMemoryContext();
        var jobApp = new JobApplication
        {
   Foretag = "Tesla",
            Jobbtitel = "Mechanical Engineer",
         Plats = "Stockholm",
      AnsokanDatum = new DateTime(2025, 1, 15),
          Status = ApplicationStatus.GattVideare,
  Anteckningar = "Spännande företag med bra kultur",
     Url = "https://tesla.com/careers/job123",
            UserId = "test-user-123"
        };

        // Act
   context.JobApplications.Add(jobApp);
     await context.SaveChangesAsync();

        // Assert
        var savedJobApp = await context.JobApplications.FirstOrDefaultAsync();
        savedJobApp.Should().NotBeNull();
        savedJobApp!.Plats.Should().Be("Stockholm");
        savedJobApp.Anteckningar.Should().Be("Spännande företag med bra kultur");
        savedJobApp.Url.Should().Be("https://tesla.com/careers/job123");
    }

    [Fact]
    public async Task CanStoreJobApplicationWithMinimalRequiredFields()
    {
   // Arrange
        using var context = CreateInMemoryContext();
        var jobApp = new JobApplication
        {
    Foretag = "MinimalCo",
            Jobbtitel = "Developer",
       UserId = "test-user-123"
        };

        // Act
 context.JobApplications.Add(jobApp);
        await context.SaveChangesAsync();

    // Assert
        var savedJobApp = await context.JobApplications.FirstOrDefaultAsync();
        savedJobApp.Should().NotBeNull();
    savedJobApp!.Foretag.Should().Be("MinimalCo");
   savedJobApp.Jobbtitel.Should().Be("Developer");
   savedJobApp.Plats.Should().BeNull();
        savedJobApp.Anteckningar.Should().BeNull();
        savedJobApp.Url.Should().BeNull();
    }
}
