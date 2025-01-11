namespace SnaelyFashion_WebAPI.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }

        IProductSizeRepository ProductSize { get; }

        IProductColorRepository ProductColor { get; }

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
