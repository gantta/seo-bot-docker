# seo-bot-docker

Running Selenium web scraping in containerized Azure Functions

[![Build Status](https://dev.azure.com/gantta/SEO%20Bot/_apis/build/status/gantta.seo-bot-docker?branchName=master)](https://dev.azure.com/gantta/SEO%20Bot/_build/latest?definitionId=11&branchName=master)

Tech Stack: .Net Core 3.1 | Docker | Terraform | Azure Functions with Containers on Linux | Azure DevOps

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

## Azure DevOps Pipeline
The `azure-pipelines.yml` is used to configure and run the pipeline from Github source to Azure. 

### Prerequisites
1. Before creating the pipeline, create a new Azure Active Directory Service Principal following this [guide](https://docs.microsoft.com/en-us/azure/active-directory/develop/howto-create-service-principal-portal). Use the option to authenticate using a secret. 

2.  Create an Azure DevOps project and setup a Service Connection of type Azure Resource Manager to connect Azure subscription. Use the Service Principal Client ID, secret, and Azure Subscription details created in the firt step.

3.  The pipeline relies on a Service Connection of type Docker Registry to connect to the container registry for storing the image. In my example, using Terraform to create the container registry, but am unable to successfully run the pipeline prior to creating the required Azure Container Registry. In my case, I use Terraform locally to create the initial Azure resources and then create a Service Connection to the Azure Container Registry.

    NOTE: If the Azure Container Registry is ever removed and recreated, a new Service Connection is required to re-establish credentials.

4.  Update the `azure-pipelines.yml` variables `azureServiceConnection` and `dockerRegistryServiceConnection` to match the name defined when creating the Service Connections.

5.  Update the Terraform variables `TF_*` to match the shared state file stored in the Azure Storage Account.

### Create the Azure DevOps pipeline
Follow the new pipeline creation process to connect to your respisitory and select the existing `azure-pipelines.yml` file. 
Before saving the pipeline, create four new variables to store your Azure Active Directory Service Principal credentials:

    ARM_CLIENT_ID="<YourPrincipalAppID>"
    ARM_CLIENT_SECRET="<YourAppsSecret>"
    ARM_SUBSCRIPTION_ID="<SubscriptionID>"
    ARM_TENANT_ID="<AzureActiveDirectoryTenantID>"

Save and run the pipeline.

## Configure Azure Function to Pull Container
Navigate to the function app created by Terraform. From Deployment > Deployment Center, configure the Container Registry settings for a Single Container.