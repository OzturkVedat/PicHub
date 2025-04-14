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
  #profile = "default"
  region = "eu-north-1"
}

module "network" {
  source = "./modules/network"

  subnet_az = var.subnet_az
}

module "ecs" {
  source = "./modules/ecs"

  aws_region       = var.aws_region
  vpc_id           = module.network.vpc_id
  public_subnet_id = module.network.public_subnet_id
}
