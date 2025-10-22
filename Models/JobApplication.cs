using System.ComponentModel.DataAnnotations;

namespace JobApplicationTrackerV2.Models
{
    public enum ApplicationStatus
    {
        VantarPaSvar,
        Nej,
        Ja,
        GattVideare,
        Intervju,
        Avbruten
    }

    public class JobApplication
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Företag är obligatoriskt")]
        [StringLength(200)]
        public string Foretag { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jobbtitel är obligatoriskt")]
        [StringLength(200)]
        public string Jobbtitel { get; set; } = string.Empty;

        [StringLength(200)]
        public string? Plats { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AnsokanDatum { get; set; } = DateTime.Today;

        [Required]
        public ApplicationStatus Status { get; set; } = ApplicationStatus.VantarPaSvar;

        [StringLength(1000)]
        public string? Anteckningar { get; set; }

        [StringLength(300)]
        [Url(ErrorMessage = "Ange en giltig URL")]
        public string? Url { get; set; }

        // Koppla till användare
        [Required]
        public string UserId { get; set; } = string.Empty;
    }
}