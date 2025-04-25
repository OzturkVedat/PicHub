terraform {
  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 5.0"
    }
  }
  backend "s3" {
    bucket       = "tfstate-bucket-pichub"
    key          = "terraform.tfstate"
    region       = "eu-north-1"
    encrypt      = true
    use_lockfile = true # prevent concurrent ops on statefile
  }
}

provider "aws" {
  region = "eu-north-1"
}

module "network" {
  source = "./modules/network"

  subnet_az = var.subnet_az
}

module "server" {
  source = "./modules/server"

  aws_region       = var.aws_region
  vpc_id           = module.network.vpc_id
  public_subnet_id = module.network.public_subnet_id

  api_image_uri = local.api_img_uri

  user_pool_arn           = data.aws_ssm_parameter.user_pool_arn.value
  user_pool_client_id     = data.aws_ssm_parameter.user_pool_client_id.value
  user_pool_client_secret = data.aws_ssm_parameter.user_pool_client_secret.value
  jwt_authority           = data.aws_ssm_parameter.jwt_authority.value
}
