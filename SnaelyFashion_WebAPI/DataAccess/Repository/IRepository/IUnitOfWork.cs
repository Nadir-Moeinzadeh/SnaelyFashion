namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        IReviewRepository Review { get; }
        IProductSizeRepository ProductSize { get; }
        IBlogPostImageRepository BlogPostImage{ get; }
        IProductColorRepository ProductColor { get; }
        IBlogPostRepository BlogPost { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        ISubCategoryRepository SubCategory { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
        IProductImageRepository ProductImage { get; }

        IProfilePictureRepository ProfilePicture { get; }

        void Save();


    }
}
