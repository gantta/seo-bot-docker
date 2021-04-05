# seo-bot

Directing search engines to https://palmbeachacu.com/

## Getting Started with Terraform
For local devlopment and testing

All infrastructure code will reside within the `Terraform` dir.

Create a `envVariables.tfvars` file in the root directory with the following variables:

    ARM_CLIENT_ID="<YourPrincipalAppID>"
    ARM_CLIENT_SECRET="<YourAppsSecret>"
    ARM_SUBSCRIPTION_ID="<SubscriptionID>"
    ARM_TENANT_ID="<AzureActiveDirectoryTenantID>"

1.	`cd .\Terraform\`
2.  `terraform init`
3.	`terraform plan -var-file="envVariables.tfvars"`
4.	`terraform apply -var-file="envVariables.tfvars"`
5.	`terraform destroy -var-file="envVariables.tfvars"`

## Getting Started with Docker
Using Docker Desktop for local environment

1.  Build the image: `docker build --build-arg StorageConnectionString="<StorageAccountConnectionString" -t seo-bot:latest .`
2.  Launch from the built image: `docker run -d --name seo-bot -p 80:80 seo-bot:latest`
3.  Check the status: `docker ps`
4.  Check logs: `docker logs seo-bot`
5.  Remote shell into the image: `docker exec -it seo-bot /bin/bash`