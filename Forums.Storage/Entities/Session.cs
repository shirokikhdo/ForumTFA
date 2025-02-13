using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Forums.Storage.Entities;

public class Session
{
    [Key]
    public Guid SessionId { get; set; }

    public Guid UserId { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}