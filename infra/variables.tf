# ECS vars
variable "aws_access_key" {
  type      = string
  sensitive = true
}

variable "aws_secret_key" {
  type      = string
  sensitive = true
}

variable "aws_region" {
  type      = string
  sensitive = true
}

variable "api_image_uri" {
  description = "Pichub API image URI"
  type        = string
}

variable "ec2_ami_id" {
  description = "EC2 AMI ID for server"
  type        = string
}

variable "param_resource" {
  description = "SSM parameter store resource"
  type        = string
}

variable "user_pool_client_id" {
  type      = string
  sensitive = true
}

variable "user_pool_client_secret" {
  type      = string
  sensitive = true
}

variable "jwt_authority" {
  type      = string
  sensitive = true
}

variable "ssh_key_name" {
  description = "Name of the AWS key pair to use for SSH access"
  type        = string
}



# Network vars
variable "vpc_cidr" {}

variable "subnet_cidr" {}

variable "subnet_az" {
  description = "Public subnet availability zone of VPC (first)"
  type        = string
}

variable "allowed_ssh_ip" {}
