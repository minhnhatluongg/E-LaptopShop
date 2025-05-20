using E_LaptopShop.Application.DTOs;
using E_LaptopShop.Application.Features.SysFile.Command.UploadChunkCommand;
using E_LaptopShop.Application.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace E_LaptopShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHostEnvironment _hostEnvironment;

        public FileController(IMediator mediator, IHostEnvironment hostEnvironment)
        {
            _mediator = mediator;
            _hostEnvironment = hostEnvironment;
        }

        [HttpPost("upload-chunk")]
        public async Task<ActionResult<ApiResponse<ChunkUploadResponseDto>>> UploadChunk(
                IFormFile chunk, 
                [FromForm] string fileName,
                [FromForm] int chunkNumber, 
                [FromForm] int totalChunks
                )
        {
            try
            {
                if (chunk == null || chunk.Length == 0)
                {
                    return BadRequest(ApiResponse<ChunkUploadResponseDto>.ErrorResponse(("No chunk data was uploaded")));
                }
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous"; 
                var command = new UploadChunkCommand
                {
                    Chunk = chunk,
                    FileName = fileName,
                    ChunkNumber = chunkNumber,
                    TotalChunks = totalChunks,
                    UploadedBy = userId
                };
                var result = await _mediator.Send(command);
                var responseDto = new ChunkUploadResponseDto
                {
                    IsCompleted = result.IsCompleted,
                    SysFileId = result.SysFileId,
                    FileUrl = result.FileUrl
                };
                return Ok(ApiResponse<ChunkUploadResponseDto>.SuccessResponse(
                    responseDto,
                    result.IsCompleted ? "All chunks uploaded successfully" : "Chunk uploaded successfully"));
            }
            catch (Exception ex) {
                return StatusCode(500, ApiResponse<ChunkUploadResponseDto>.ErrorResponse("An error occurred while uploading chunk"));
            }
        }

        [HttpGet("validate-chunk")]
        public ActionResult<ApiResponse<ChunkValidationResponseDto>> ValidateChunk(string fileName, int chunkNumber)
        {
            try
            {
                var fileFolder = Path.Combine(_hostEnvironment.ContentRootPath, "temp-uploads", fileName.Replace(" ", "_"));
                var chunkPath = Path.Combine(fileFolder, $"chunk_{chunkNumber}");

                var exists = System.IO.File.Exists(chunkPath);

                var responseDto = new ChunkValidationResponseDto { Exists = exists };

                return Ok(ApiResponse<ChunkValidationResponseDto>.SuccessResponse(
                    responseDto,
                    "Chunk validation completed"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse<ChunkValidationResponseDto>.ErrorResponse("An error occurred while validating chunk"));
            }
        }

        [HttpPost("upload-file")]
        public async Task<ActionResult<ApiResponse<ChunkUploadResponseDto>>> UploadFile(IFormFile file)
        {
            if (file == null)
                return BadRequest(ApiResponse<ChunkUploadResponseDto>.ErrorResponse("No file uploaded"));

            // Kích thước mỗi chunk (1MB)
            const int chunkSize = 1024 * 1024;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "anonymous";
            // Tính tổng số chunk
            int totalChunks = (int)Math.Ceiling((double)file.Length / chunkSize);

            ChunkUploadResult finalResult = null;

            using (var stream = file.OpenReadStream())
            {
                for (int chunkNumber = 1; chunkNumber <= totalChunks; chunkNumber++)
                {
                    // Tạo chunk từ file
                    byte[] buffer = new byte[Math.Min(chunkSize, file.Length - ((chunkNumber - 1) * chunkSize))];
                    await stream.ReadAsync(buffer, 0, buffer.Length);

                    // Tạo IFormFile cho chunk
                    using (var memoryStream = new MemoryStream(buffer))
                    {
                        var chunkFile = new FormFile(memoryStream, 0, buffer.Length, "chunk", file.FileName);

                        // Gọi command handler để xử lý chunk
                        var command = new UploadChunkCommand
                        {
                            Chunk = chunkFile,
                            FileName = file.FileName,
                            ChunkNumber = chunkNumber,
                            TotalChunks = totalChunks,
                            UploadedBy = userId
                        };

                        finalResult = await _mediator.Send(command);
                    }
                }
            }

            if (finalResult != null && finalResult.IsCompleted)
            {
                return Ok(new ApiResponse<ChunkUploadResponseDto>
                {
                    Success = true,
                    Message = "File uploaded successfully",
                    Data = new ChunkUploadResponseDto
                    {
                        IsCompleted = true,
                        SysFileId = finalResult.SysFileId,
                        FileUrl = finalResult.FileUrl
                    }
                });
            }
            return BadRequest(ApiResponse<ChunkUploadResponseDto>.ErrorResponse("Failed to upload file"));
        }
    }
}
