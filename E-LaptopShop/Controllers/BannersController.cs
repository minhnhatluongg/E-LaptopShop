using AutoMapper;
using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Models;
using E_LaptopShop.Controllers.Api_version;
using E_LaptopShop.Domain.Entities;
using E_LaptopShop.Domain.Repositories;
using E_LaptopShop.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace E_LaptopShop.Controllers
{
    [Route("api/v1/banners")]
    public class BannersController : ApiV1ControllerBase
    {
        private readonly IBannerRepository _repo;
        private readonly IMapper _mapper;
        private readonly ILogger<BannersController> _logger;

        public BannersController(IBannerRepository repo, IMapper mapper, ILogger<BannersController> logger)
        {
            _repo = repo;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>🔓 [PUBLIC] Banner đang active theo vị trí (dùng cho carousel homepage).</summary>
        [HttpGet("active")]
        [Tags(ApiTags.Public)]
        public async Task<ActionResult<ApiResponse<IEnumerable<BannerDto>>>> GetActive(
            [FromQuery] string position = "HOMEPAGE_TOP",
            CancellationToken ct = default)
        {
            var banners = await _repo.GetActiveByPositionAsync(position, ct);
            return Ok(ApiResponse<IEnumerable<BannerDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<BannerDto>>(banners)));
        }

        /// <summary>👑 [ADMIN] Toàn bộ banner (kể cả inactive).</summary>
        [HttpGet]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<IEnumerable<BannerDto>>>> GetAll(CancellationToken ct)
        {
            var all = await _repo.GetAllAsync(ct);
            var ordered = all.OrderBy(b => b.DisplayOrder);
            return Ok(ApiResponse<IEnumerable<BannerDto>>.SuccessResponse(
                _mapper.Map<IEnumerable<BannerDto>>(ordered)));
        }

        /// <summary>👑 [ADMIN] Chi tiết một banner.</summary>
        [HttpGet("{id:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<BannerDto>>> GetById(int id, CancellationToken ct)
        {
            var banner = await _repo.GetByIdAsync(id, ct);
            if (banner == null) return NotFound(ApiResponse<BannerDto>.ErrorResponse("Banner not found"));
            return Ok(ApiResponse<BannerDto>.SuccessResponse(_mapper.Map<BannerDto>(banner)));
        }

        /// <summary>👑 [ADMIN] Tạo banner mới.</summary>
        [HttpPost]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<BannerDto>>> Create(
            [FromBody] CreateBannerDto dto, CancellationToken ct)
        {
            try
            {
                var entity = _mapper.Map<Banner>(dto);
                entity.CreatedAt = DateTime.UtcNow;
                var saved = await _repo.AddAsync(entity, ct);
                return CreatedAtAction(nameof(GetById), new { id = saved.Id },
                    ApiResponse<BannerDto>.SuccessResponse(_mapper.Map<BannerDto>(saved)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating banner");
                return BadRequest(ApiResponse<BannerDto>.ErrorResponse(ex.Message));
            }
        }

        /// <summary>👑 [ADMIN] Cập nhật banner.</summary>
        [HttpPut("{id:int}")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<BannerDto>>> Update(
            int id, [FromBody] UpdateBannerDto dto, CancellationToken ct)
        {
            var existing = await _repo.GetByIdAsync(id, ct);
            if (existing == null) return NotFound(ApiResponse<BannerDto>.ErrorResponse("Banner not found"));

            _mapper.Map(dto, existing);
            var saved = await _repo.UpdateAsync(existing, ct);
            return Ok(ApiResponse<BannerDto>.SuccessResponse(_mapper.Map<BannerDto>(saved)));
        }

        /// <summary>👑 [ADMIN] Xoá banner.</summary>
        [HttpDelete("{id:int}")]
        [AdminOnly]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<int>>> Delete(int id, CancellationToken ct)
        {
            var ok = await _repo.DeleteAsync(id, ct);
            return Ok(new ApiResponse<int> { Success = ok, Data = id, Message = ok ? "Deleted" : "Not found" });
        }

        /// <summary>👑 [ADMIN] Đổi thứ tự nhanh (swap DisplayOrder).</summary>
        [HttpPut("{id:int}/order")]
        [AdminOrManager]
        [Tags(ApiTags.Admin)]
        public async Task<ActionResult<ApiResponse<BannerDto>>> SetOrder(
            int id, [FromQuery] int order, CancellationToken ct)
        {
            var banner = await _repo.GetByIdAsync(id, ct);
            if (banner == null) return NotFound(ApiResponse<BannerDto>.ErrorResponse("Banner not found"));
            banner.DisplayOrder = order;
            var saved = await _repo.UpdateAsync(banner, ct);
            return Ok(ApiResponse<BannerDto>.SuccessResponse(_mapper.Map<BannerDto>(saved)));
        }
    }
}
