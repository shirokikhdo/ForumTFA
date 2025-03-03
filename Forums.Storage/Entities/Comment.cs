﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Forums.Storage.Entities;

public class Comment
{
    [Key]
    public Guid CommentId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public Guid TopicId { get; set; }

    public Guid UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User Author { get; set; }

    [ForeignKey(nameof(TopicId))]
    public Topic Topic { get; set; }

    public string Text { get; set; }
}