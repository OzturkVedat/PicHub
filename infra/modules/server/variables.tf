variable "aws_region" {}
variable "vpc_id" {}
variable "public_subnet_id" {}

variable "api_image_uri" {}

variable "param_resource" {
  default = "arn:aws:ssm:eu-north-1:<account-id>:parameter/pichub"
}

variable "user_pool_arn" {
  default = "arn:aws:cognito-idp:eu-north-1:<account-id>:userpool/eu-north-1_xxxxxxx"
}
