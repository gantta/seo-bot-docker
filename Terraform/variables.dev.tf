variable "ARM_CLIENT_ID" {
  default = ""
  description = "The service principal application ID from Azure Active Directory"
}

variable "ARM_CLIENT_SECRET" {
  default = ""
  description = "The application secret to use as credential password"
}

variable "ARM_SUBSCRIPTION_ID" {
  default = ""
  description = "The default subscription to use"
}

variable "ARM_TENANT_ID" {
  default = ""
  description = "The Azure Active Directory where the service principal resides"
}

variable "environment" {
  default = "dev"
  description = "Describes the environment tag for all resources"
}

variable "location" {
  default = "East US"
  description = "The Azure location where all resources in this example should be created"
}

variable "prefix" {
  default = "seobot"
  description = "The prefix used for all resources in this example"
}