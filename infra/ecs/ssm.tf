resource "aws_ssm_parameter" "aws_access_key" {
  name        = "/pichub/aws_access_key"
  description = "AWS access key for PicHub"
  type        = "SecureString"
  value       = var.aws_access_key
}

resource "aws_ssm_parameter" "aws_secret_key" {
  name        = "/pichub/aws_secret_key"
  description = "AWS secret key for PicHub"
  type        = "SecureString"
  value       = var.aws_secret_key
}

resource "aws_ssm_parameter" "aws_region" {
  name        = "/pichub/aws_region"
  description = "AWS region for PicHub"
  type        = "SecureString"
  value       = var.aws_region
}

resource "aws_ssm_parameter" "user_pool_client_id" {
  name        = "/pichub/user_pool_client_id"
  description = "Cognito user pool client ID of PicHub"
  type        = "SecureString"
  value       = var.user_pool_client_id
}
resource "aws_ssm_parameter" "user_pool_client_secret" {
  name        = "/pichub/user_pool_client_secret"
  description = "Cognito user pool client secret of PicHub"
  type        = "SecureString"
  value       = var.user_pool_client_secret
}
resource "aws_ssm_parameter" "jwt_authority" {
  name        = "/pichub/jwt_authority"
  description = "JWT authority of PicHub"
  type        = "SecureString"
  value       = var.jwt_authority
}
