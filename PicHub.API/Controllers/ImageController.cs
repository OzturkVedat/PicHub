using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PicHub.API.Models;
using PicHub.API.Services;

namespace PicHub.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImageController : ControllerBase
    {
        private readonly S3Service _s3Service;
        public ImageController(S3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpGet("img-base64/{objKey}")]
        public async Task<IActionResult> GetImageAsBase64(string objKey)
        {
            if (string.IsNullOrEmpty(objKey))
                return BadRequest(new FailureResult("Image name cannot be empty."));

            string bucket = User.FindFirst("custom:s3Bucket")?.Value;
            if (string.IsNullOrEmpty(bucket))
                return BadRequest(new FailureResult("Image repository not found for user."));


            var imgResult = await _s3Service.GetImageFromBucket(bucket, objKey);
            return imgResult.IsSuccess ? Ok(imgResult) : BadRequest(imgResult);
        }

        [HttpGet("all-img-urls")]
        public async Task<IActionResult> GetAllImageURLs()  // returns object keys from bucket
        {
            string bucket = User.FindFirst("custom:s3Bucket")?.Value;
            if (string.IsNullOrEmpty(bucket))
                return BadRequest(new FailureResult("Image repository not found for user."));

            var result = await _s3Service.ListUserBucketItems(bucket);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("add-new-image")]
        public async Task<IActionResult> PostNewImage(IFormFile imgFile)
        {
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (string.IsNullOrEmpty(accessToken))
                return Unauthorized(new FailureResult("Access token is missing."));

            string bucket = User.FindFirst("custom:s3Bucket")?.Value;
            if (bucket == null)
                return Unauthorized(new FailureResult("User is missing a credential."));

            var response = await _s3Service.UploadImageToBucket(bucket, imgFile);
            return response.IsSuccess ? Ok(response) : BadRequest(response);
        }

        [HttpDelete("delete-image/{objKey}")]
        public async Task<IActionResult> DeleteImage(string objKey)
        {
            if (string.IsNullOrEmpty(objKey))
                return BadRequest(new FailureResult("Image name cannot be empty."));

            string bucket = User.FindFirst("custom:s3Bucket")?.Value;
            if (string.IsNullOrEmpty(bucket))
                return BadRequest(new FailureResult("Image repository not found for user."));


            var imgResult = await _s3Service.DeleteImageFromBucket(bucket, objKey);
            return imgResult.IsSuccess ? Ok(imgResult) : BadRequest(imgResult);
        }


    }
}
