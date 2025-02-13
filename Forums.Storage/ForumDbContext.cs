using Forums.Storage.Entities;
using Microsoft.EntityFrameworkCore;

namespace Forums.Storage;

public class ForumDbContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Forum> Forums { get; set; }

    public DbSet<Topic> Topics { get; set; }

    public DbSet<Comment> Comments { get; set; }

    public DbSet<Session> Sessions { get; set; }

    public ForumDbContext(DbContextOptions<ForumDbContext> options) 
        : base(options)
    {

    }
}