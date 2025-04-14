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
