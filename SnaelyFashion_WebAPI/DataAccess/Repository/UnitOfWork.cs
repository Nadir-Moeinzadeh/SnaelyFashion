using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _db;
        public ICategoryRepository Category { get; private set; }
        public IProductColorRepository ProductColor { get; private set; }
        public IProductSizeRepository ProductSize { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IBlogPostRepository BlogPost { get; private set; }

        public IBlogPostImageRepository BlogPostImage { get; private set; }
        public IProductRepository Product { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IProductImageRepository ProductImage { get; private set; }
        public ISubCategoryRepository SubCategory { get; private set; }
        public IProfilePictureRepository ProfilePicture { get; private set; }
        public IReviewRepository Review { get; private set; }
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            ProductColor = new ProductColorRepository(_db);
            ProductSize = new ProductSizeRepository(_db);
            SubCategory = new SubCategoryRepository(_db);
            ApplicationUser = new ApplicationUserRepository(_db);
            ProductImage = new ProductImageRepository(_db);
            ShoppingCart = new ShoppingCartRepository(_db);
            Category = new CategoryRepository(_db);
            Product = new ProductRepository(_db);
            OrderHeader = new OrderHeaderRepository(_db);
            OrderDetail = new OrderDetailRepository(_db);
            ProfilePicture = new ProfilePictureRepository(_db);
            BlogPost = new BlogPostRepository(_db); 
            BlogPostImage = new BlogPostImageRepository(_db);
            Review = new ReviewRepository(_db); 
        }

        public async Task SaveAsync()
        {
            _db.SaveChanges();
            await _db.SaveChangesAsync();
        }

        public void Save()
        {
            _db.SaveChanges();
        }
    }
}
