using Amazon.S3;
using Amazon.S3.Model;
using PicHub.API.Models;

namespace PicHub.API.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;    

        public S3Service(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<ResultModel> GetImageFromBucket(string bucketName, string objKey)
        {
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = objKey
                };
                using var response = await _s3Client.GetObjectAsync(request);

                using var responseStream = response.ResponseStream;
                using var memoryStream = new MemoryStream();
                await responseStream.CopyToAsync(memoryStream);

                var imgBytes = memoryStream.ToArray();
                string base64Img = Convert.ToBase64String(imgBytes);
                return new SuccessDataResult<string>(base64Img);

            }
            catch (AmazonS3Exception ex)
            {
                return new FailureResult($"AWS S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error retrieving image: {ex.Message}");
            }
        }

        public async Task<ResultModel> ListUserBucketItems(string bucketName)
        {
            try
            {
                var request = new ListObjectsV2Request { BucketName = bucketName };
                var response = await _s3Client.ListObjectsV2Async(request);

                var objKeys = response.S3Objects.Select(obj => obj.Key).ToList();
                return new SuccessDataResult<List<string>>(objKeys);
            }
            catch (AmazonS3Exception ex)
            {
                return new FailureResult($"AWS S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error retrieving bucket objects: {ex.Message}");
            }
        }

        public async Task<ResultModel> CreateUserBucket(string bucketName)
        {
            try
            {
                var request = new PutBucketRequest()
                {
                    BucketName = bucketName,
                    UseClientRegion = true
                };
                await _s3Client.PutBucketAsync(request);
                return new SuccessDataResult<string>("User bucket is created.", bucketName);
            }
            catch (AmazonS3Exception ex)
            {
                return new FailureResult($"AWS S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error creating image repository: {ex.Message}");
            }
        }

        public async Task<ResultModel> UploadImageToBucket(string bucketName, IFormFile img)
        {
            try
            {
                if (img == null || img.Length == 0)
                    return new FailureResult("Invalid image file");

                string fileExtension = Path.GetExtension(img.FileName);
                string objectKey = $"{Guid.NewGuid()}{fileExtension}";
                var objRequest = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = objectKey,
                    InputStream = img.OpenReadStream(),
                    ContentType = img.ContentType
                };
                await _s3Client.PutObjectAsync(objRequest);
                return new SuccessResult("Image uploaded successfully");
            }
            catch (AmazonS3Exception ex)
            {
                return new FailureResult($"AWS S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error uploading image: {ex.Message}");
            }
        }

        public async Task<ResultModel> DeleteImageFromBucket(string bucketName, string objectKey)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = objectKey
                };

                await _s3Client.DeleteObjectAsync(deleteRequest);
                return new SuccessResult($"Image '{objectKey}' deleted successfully.");
            }
            catch (AmazonS3Exception ex)
            {
                return new FailureResult($"AWS S3 error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error deleting image: {ex.Message}");
            }
        }

    }
}
