variable "aws_access_key" {}
variable "aws_secret_key" {}
variable "aws_region" {}
variable "api_image_uri" {}
variable "ec2_ami_id" {}

variable "param_resource" {}
variable "user_pool_client_id" {}
variable "user_pool_client_secret" {}
variable "jwt_authority" {}
variable "ssh_key_name" {}
variable "allowed_ssh_ip" {}

variable "vpc_id" {
  description = "The ID of the VPC where ECS will be deployed"
  type        = string
}
variable "public_subnet_id" {
  description = "ID of public subnet (first)"
  type        = string
}
