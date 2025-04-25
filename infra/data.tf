data "aws_ssm_parameter" "param_resource" {
  name = "/ivote/param_resource"
}

data "aws_ssm_parameter" "user_pool_arn" {
  name = "/ivote/user_pool_arn"
}

data "aws_ecr_repository" "pichub" {
  name = "pichub/api"
}

data "aws_ecr_image" "api_img" {
  repository_name = data.aws_ecr_repository.pichub.name
  image_tag       = "latest"
}

locals {
  api_img_uri = "${data.aws_ecr_repository.pichub.repository_url}@${data.aws_ecr_image.api_img.image_digest}"
}

data "aws_instances" "ecs_nodes" {
  filter {
    name   = "tag:aws:autoscaling:groupName"
    values = [module.server.ecs_asg_name]
  }

  filter {
    name   = "instance-state-name"
    values = ["running"]
  }
}

data "aws_instance" "first_ecs_instance" {
  instance_id = tolist(data.aws_instances.ecs_nodes.ids)[0]
}

output "swagger_ui_url" {
  value = "http://${data.aws_instance.first_ecs_instance.public_ip}/swagger/index.html"
}
