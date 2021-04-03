# seo-bot

Directing search engines to https://palmbeachacu.com/

# Getting Started with Terraform
For local devlopment and testing

All infrastructure code will reside within the `Terraform` dir.

Create a `envVariables.tfvars` file in the root directory with the following variables:

    ARM_CLIENT_ID="<YourPrincipalAppID>"
    ARM_CLIENT_SECRET="<YourAppsSecret>"
    ARM_SUBSCRIPTION_ID="<SubscriptionID>"
    ARM_TENANT_ID="<AzureActiveDirectoryTenantID>"

1.	terraform init
2.	terraform plan -var-file="envVariables.tfvars"
3.	terraform apply -var-file="envVariables.tfvars"
4.	terraform destroy -var-file="envVariables.tfvars"