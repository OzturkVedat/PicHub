resource "aws_vpc" "main" {
  cidr_block = var.vpc_cidr # IP ranges available inside the vpc

  tags = {
    "Name" = "pichub_vpc"
  }
}

resource "aws_subnet" "public_subnet" {
  vpc_id                  = aws_vpc.main.id
  cidr_block              = var.subnet_cidr
  availability_zone       = var.subnet_az
  map_public_ip_on_launch = true

  tags = {
    Name = "pichub_public_subnet"
  }
}

resource "aws_internet_gateway" "pichub_igw" {
  vpc_id = aws_vpc.main.id

  tags = {
    Name = "pichub_internet_gateway"
  }
}

resource "aws_route_table" "public" {
  vpc_id = aws_vpc.main.id

  route {
    cidr_block = "0.0.0.0/0"
    gateway_id = aws_internet_gateway.pichub_igw.id
  }

  tags = {
    Name = "pichub_internet_route_table"
  }
}

resource "aws_route_table_association" "public" {
  subnet_id      = aws_subnet.public_subnet.id
  route_table_id = aws_route_table.public.id
}
