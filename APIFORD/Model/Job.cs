// APIFORD/Model/Job.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIFORD.Model;

public class Job
{
    public Guid Id { get; set; }

    [Required]
    [MaxLength(10)]
    public string Status { get; set; } = "pending"; // pending | running | done | error

    [Required]
    public string Payload { get; set; } = string.Empty; // JSON: { model, brand, year, urls }

    public string? Result { get; set; } // JSON com as specs (preenchido pelo Python)

    [MaxLength(500)]
    public string? Error { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string? UserId { get; set; }
    public int? CarroId { get; set; }      // preenchido quando processado
    public bool Notificado { get; set; }
}