using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SnaelyFashion_Models;


namespace SnaelyFashion_WebAPI.DataAccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<ProfilePicture> ProfilePicture { get; set; }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<BlogPostImage> BlogPostImages { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {


            base.OnModelCreating(modelBuilder);


          


            modelBuilder.Entity<Product>()
                .HasOne(x => x.Category)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
                .HasOne(x => x.SubCategory)
                .WithMany(x => x.Products)
                .HasForeignKey(x => x.SubCategoryId)
                .OnDelete(DeleteBehavior.Restrict);


     


            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Tops" },
                new Category { Id = 2, Name = "Outwear" },
                new Category { Id = 3, Name = "Bottoms" },
                new Category { Id = 4, Name = "Homewear" },
                new Category { Id = 5, Name = "Innerwear" },
                new Category { Id = 6, Name = "Sportwear" },
                new Category { Id = 7, Name = "Accessories" }
            );

            



        }

    }
}
