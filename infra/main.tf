provider "aws" {
  profile = "default"
  region  = "eu-north-1"
}

module "network" {
  source         = "./network"
  vpc_cidr       = var.vpc_cidr
  allowed_ssh_ip = var.allowed_ssh_ip

  subnet_cidr = var.subnet_cidr
  subnet_az   = var.subnet_az
}

module "ecs" {
  source                  = "./ecs"
  aws_access_key          = var.aws_access_key
  aws_secret_key          = var.aws_secret_key
  aws_region              = var.aws_region
  api_image_uri           = var.api_image_uri
  ec2_ami_id              = var.ec2_ami_id
  param_resource          = var.param_resource
  user_pool_client_id     = var.user_pool_client_id
  user_pool_client_secret = var.user_pool_client_secret
  jwt_authority           = var.jwt_authority

  ssh_key_name   = var.ssh_key_name
  allowed_ssh_ip = var.allowed_ssh_ip

  vpc_id           = module.network.vpc_id
  public_subnet_id = module.network.public_subnet_id
}
