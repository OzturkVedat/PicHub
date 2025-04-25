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

resource "aws_ssm_parameter" "param_resource" {
  name  = "/ivote/param_resource"
  type  = "SecureString"
  value = var.param_resource
}

resource "aws_ssm_parameter" "user_pool_arn" {
  name  = "/ivote/user_pool_arn"
  type  = "SecureString"
  value = var.user_pool_arn
}
