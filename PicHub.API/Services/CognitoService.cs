using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;

using PicHub.API.Models;
using System.Security.Cryptography;
using System.Text;
using Amazon.S3.Util;
using Amazon.S3;

namespace PicHub.API.Services
{
    public class CognitoService
    {
        private readonly IAmazonCognitoIdentityProvider _cognitoClient;
        private readonly string _clientId;
        private readonly string _clientSecret;

        private readonly Amazon.RegionEndpoint _s3Region;
        private readonly S3Service _s3Service;
        private readonly ICacheService _cacheService;

        public CognitoService(IAmazonCognitoIdentityProvider cognitoClient, IConfiguration config,
             S3Service s3Service, ICacheService cacheService)
        {
            _cognitoClient = cognitoClient;
            _clientId = config["/pichub/user_pool_client_id"].ToString();
            _clientSecret = config["/pichub/user_pool_client_secret"].ToString();

            _s3Region = Amazon.RegionEndpoint.GetBySystemName(config["/pichub/aws_region"]);
            _s3Service = s3Service;
            _cacheService = cacheService;
        }

        public async Task<ResultModel> RegisterUser(string email, string password)
        {
            try
            {
                using var s3Client = new AmazonS3Client(_s3Region);
                string bucketName = GenerateBucketName();
                bool bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
                while (bucketExists)
                {
                    bucketName = GenerateBucketName();
                    bucketExists = await AmazonS3Util.DoesS3BucketExistV2Async(s3Client, bucketName);
                }
                await _cacheService.SetBucketName(email, bucketName);
                var request = new SignUpRequest
                {
                    ClientId = _clientId,
                    Username = email,
                    Password = password,
                    SecretHash = ComputeSecretHash(email),
                    UserAttributes = new List<AttributeType>
                    {
                        new AttributeType { Name = "email", Value = email },
                        new AttributeType { Name= "custom:s3Bucket", Value= bucketName  }
                    }
                };
                await _cognitoClient.SignUpAsync(request);
                return new SuccessResult("User is registered. Verify email. ");
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return new FailureResult($"AWS Cognito error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Error registering user: {ex.Message}");
            }
        }
        private string GenerateBucketName()
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new(10);
            byte[] randomBytes = new byte[10];

            RandomNumberGenerator.Fill(randomBytes);

            for (int i = 0; i < randomBytes.Length; i++)
            {
                result.Append(chars[randomBytes[i] % chars.Length]);
            }
            return result.ToString();
        }

        public async Task<ResultModel> ConfirmUser(ConfirmRequest confRequest)
        {
            try
            {
                var request = new ConfirmSignUpRequest
                {
                    ClientId = _clientId,
                    Username = confRequest.Email,
                    SecretHash = ComputeSecretHash(confRequest.Email),
                    ConfirmationCode = confRequest.ConfirmationCode
                };
                string bucketName = await _cacheService.GetBucketName(confRequest.Email);
                await _s3Service.CreateUserBucket(bucketName);

                await _cognitoClient.ConfirmSignUpAsync(request);
                return new SuccessResult("User confirmed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Confirmation error: {ex.Message}");
                return new FailureResult("Unexpected error while confirmation.");
            }
        }

        public async Task<ResultModel> LoginUser(string email, string password)
        {
            try
            {
                var request = new InitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                    ClientId = _clientId,
                    AuthParameters = new Dictionary<string, string>
                    {
                        {"USERNAME", email },
                        {"PASSWORD", password },
                        {"SECRET_HASH",ComputeSecretHash(email)}
                    }
                };

                var response = await _cognitoClient.InitiateAuthAsync(request);
                var auth = new LoginResponse
                {
                    AccessToken = response.AuthenticationResult?.IdToken,
                    RefreshToken = response.AuthenticationResult?.RefreshToken,
                };

                return new SuccessDataResult<LoginResponse>(auth);
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return new FailureResult($"AWS Cognito error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<ResultModel> RefreshToken(string refreshToken)
        {
            try
            {
                var request = new InitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.REFRESH_TOKEN_AUTH,
                    ClientId = _clientId,
                    AuthParameters = new Dictionary<string, string>
                    {
                        {"REFRESH_TOKEN", refreshToken }
                    }
                };
                var response = await _cognitoClient.InitiateAuthAsync(request);
                return new SuccessDataResult<string>(response.AuthenticationResult?.IdToken);  // new jwt
            }
            catch (AmazonCognitoIdentityProviderException ex)
            {
                return new FailureResult($"AWS Cognito error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return new FailureResult($"Unexpected error: {ex.Message}");
            }
        }

        public async Task<bool> LogoutUser(string accessToken)
        {
            try
            {
                var request = new GlobalSignOutRequest
                {
                    AccessToken = accessToken
                };

                await _cognitoClient.GlobalSignOutAsync(request);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Logout error: {ex.Message}");
                return false;
            }
        }

        private string ComputeSecretHash(string email)
        {
            string message = email + _clientId;
            byte[] keyBytes = Encoding.UTF8.GetBytes(_clientSecret);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                byte[] hashBytes = hmac.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashBytes);
            }
        }

    }
}
