variable "aws_region" {}
variable "vpc_id" {}
variable "public_subnet_id" {}

variable "api_image_uri" {
  default = "962546904675.dkr.ecr.eu-north-1.amazonaws.com/pichub/api:latest"
}

variable "param_resource" {
  default = "arn:aws:ssm:eu-north-1:962546904675:parameter/pichub"
}

variable "user_pool_arn" {
  default = "arn:aws:cognito-idp:eu-north-1:962546904675:userpool/eu-north-1_oHWl79A66"
}
