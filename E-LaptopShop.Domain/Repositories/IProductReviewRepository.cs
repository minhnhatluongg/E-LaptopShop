using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using E_LaptopShop.Domain.Entities;

namespace E_LaptopShop.Domain.Repositories
{
    public interface IProductReviewRepository
    {
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(int productId, CancellationToken ct = default);
        Task<ProductReview?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ProductReview> AddAsync(ProductReview review, CancellationToken ct = default);
        Task<int> DeleteAsync(int id, CancellationToken ct = default);
        Task<int> CountByProductIdAsync(int productId, CancellationToken ct = default);
        Task<double> AverageRatingByProductIdAsync(int productId, CancellationToken ct = default);
        Task<Dictionary<int, int>> RatingBreakdownByProductIdAsync(int productId, CancellationToken ct = default);
        Task<bool> ExistsByUserAndProductAsync(int userId, int productId, CancellationToken ct = default);

        /// <summary>
        /// Kiểm tra user đã từng mua sản phẩm này (status Delivered/Completed).
        /// Dùng để xác định IsVerifiedPurchase và quyền review.
        /// </summary>
        Task<bool> HasUserPurchasedProductAsync(int userId, int productId, CancellationToken ct = default);

        /// <summary>Lấy tên loyalty tier của user (vd: "Đồng", "Bạc", "Vàng"). Null nếu chưa có.</summary>
        Task<string?> GetUserLoyaltyTierNameAsync(int userId, CancellationToken ct = default);
    }
}
