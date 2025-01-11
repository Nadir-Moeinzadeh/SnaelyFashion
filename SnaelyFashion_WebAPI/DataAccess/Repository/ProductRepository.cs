using SnaelyFashion_Models;
using SnaelyFashion_WebAPI.DataAccess.Data;
using SnaelyFashion_WebAPI.DataAccess.Repository.IRepository;

namespace SnaelyFashion_WebAPI.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Product? obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.SubCategoryId = obj.SubCategoryId;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;

                objFromDb.ProductImages = obj.ProductImages;
                objFromDb.Colors = obj.Colors;
                objFromDb.Reviews = obj.Reviews;

                //if (obj.ProductImages != null)
                //{
                //    foreach (var image in obj.ProductImages)
                //    { 


                //       objFromDb.ImageUrl = obj.ImageUrl;
                //    }
                //}
            }
            _db.SaveChanges();
        }

        public async Task UpdateAsync(Product obj)
        {
            var objFromDb = _db.Products.FirstOrDefault(u => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                //objFromDb.SubCategoryId = obj.SubCategoryId;
                objFromDb.Price = obj.Price;
                objFromDb.Description = obj.Description;
                objFromDb.CategoryId = obj.CategoryId;
                
                objFromDb.ProductImages = obj.ProductImages;
                objFromDb.Colors = obj.Colors;
                objFromDb.Reviews = obj.Reviews;

                //if (obj.ImageUrl != null)
                //{
                //    objFromDb.ImageUrl = obj.ImageUrl;
                //}
            }
            await _db.SaveChangesAsync();
        }
    }
}
