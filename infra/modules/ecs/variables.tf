variable "aws_region" {}
variable "vpc_id" {}
variable "public_subnet_id" {}

variable "api_image_uri" {
  default = "${data.aws_caller_identity.current.account_id}.dkr.ecr.${var.aws_region}.amazonaws.com/pichub/api:latest"
}

variable "ec2_ami_id" {
  default = "ami-07f6330cb37447858"
}

variable "param_resource" {
  default = "arn:aws:ssm:${var.aws_region}:${data.aws_caller_identity.current.account_id}:parameter/pichub"
}

variable "user_pool_arn" {
  default = "arn:aws:cognito-idp:${var.aws_region}:${data.aws_caller_identity.current.account_id}:userpool/eu-north-1_oHWl79A66"
}
