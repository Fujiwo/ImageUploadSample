using Microsoft.EntityFrameworkCore;
namespace ImageUploadSample.Models;

public class ImagesContext : DbContext
{
    public ImagesContext(DbContextOptions<ImagesContext> options) : base(options)
    {}

    public virtual DbSet<Image>? Images { get; set; }
}
