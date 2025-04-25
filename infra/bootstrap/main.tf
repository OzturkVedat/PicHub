terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
}

provider "aws" {
  profile = "default"
  region  = "eu-north-1"
}

resource "aws_ssm_parameter" "user_pool_arn" {
  name  = "/ivote/user_pool_arn"
  type  = "SecureString"
  value = var.user_pool_arn
}

resource "aws_ssm_parameter" "user_pool_client_id" {
  name  = "/ivote/user_pool_client_id"
  type  = "SecureString"
  value = var.user_pool_client_id
}

resource "aws_ssm_parameter" "user_pool_client_secret" {
  name  = "/ivote/user_pool_client_secret"
  type  = "SecureString"
  value = var.user_pool_client_secret
}

resource "aws_ssm_parameter" "jwt_authority" {
  name  = "/ivote/jwt_authority"
  type  = "SecureString"
  value = var.jwt_authority
}
