using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Forums.Storage;

public class Forum
{
    [Key]
    public Guid ForumId { get; set; }

    [MaxLength(50)]
    public string Title { get; set; }

    [InverseProperty(nameof(Topic.Forum))]
    public ICollection<Topic> Topics { get; set; }
}