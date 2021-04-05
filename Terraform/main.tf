terraform { 
  required_providers {
    azurerm = {
      source = "hashicorp/azurerm"
      version = "2.53.0"
    }
  }
  backend "azurerm" {
    resource_group_name  = "terraform-state-rg"
    storage_account_name = "ganttcloudstate997"
    container_name       = "tfstate"
    key                  = "dev.seo-bot.tfstate"
  } 
}

provider "azurerm" {
  features {}
  client_id = var.ARM_CLIENT_ID
  client_secret = var.ARM_CLIENT_SECRET
  tenant_id = var.ARM_TENANT_ID
  subscription_id = var.ARM_SUBSCRIPTION_ID
}

resource "azurerm_resource_group" "main" {
  name     = "${var.prefix}-resources-${var.environment}"
  location = var.location

  tags = {
    environment = var.environment
  }
}

resource "azurerm_application_insights" "main" {
  name                = "${var.prefix}-${var.environment}-insights"
  location            = var.location
  resource_group_name = azurerm_resource_group.main.name
  application_type    = "web"
  tags = {
    environment = var.environment
  }
}

resource "azurerm_storage_account" "main" {
  name                     = "${var.prefix}${var.environment}storage"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  tags = {
    environment = var.environment
  }
}

resource "azurerm_container_registry" "main" {
  name                     = "${var.prefix}${var.environment}acr"
  resource_group_name      = azurerm_resource_group.main.name
  location                 = azurerm_resource_group.main.location
  sku                      = "Standard"
  admin_enabled            = true
  tags = {
    environment = var.environment
  }
}

resource "azurerm_app_service_plan" "main" {
  name                = "${var.prefix}-${var.environment}-asp"
  location            = var.location
  resource_group_name = azurerm_resource_group.main.name
  kind                = "Linux"
  reserved            = true

  sku {
    tier = "Premium"
    size = "P1V2"
  }
  tags = {
    environment = var.environment
  }
}

resource "azurerm_function_app" "main" {
  name                       = "${var.prefix}-${var.environment}-func"
  location                   = var.location
  resource_group_name        = azurerm_resource_group.main.name
  app_service_plan_id        = azurerm_app_service_plan.main.id
  storage_account_name       = azurerm_storage_account.main.name
  storage_account_access_key = azurerm_storage_account.main.primary_access_key
  os_type                    = "linux"
  version                    = "~3"

  app_settings = {
    APPINSIGHTS_INSTRUMENTATIONKEY = azurerm_application_insights.main.instrumentation_key
    DOCKER_REGISTRY_SERVER_URL = azurerm_container_registry.main.login_server
    DOCKER_REGISTRY_SERVER_USERNAME = azurerm_container_registry.main.admin_username
    DOCKER_REGISTRY_SERVER_PASSWORD = azurerm_container_registry.main.admin_password
    ENVIRONMENT = var.environment
    WEBSITE_ENABLE_SYNC_UPDATE_SITE = true
    WEBSITE_RUN_FROM_PACKAGE = 1
    WEBSITES_ENABLE_APP_SERVICE_STORAGE = false
  }

  site_config {
    always_on                 = true
    use_32_bit_worker_process = false
    linux_fx_version            = "DOCKER|${azurerm_container_registry.main.login_server}/${var.image_name}:${var.tag}"
  }

  tags = {
    environment = var.environment
  }
}

resource "azurerm_application_insights_web_test" "main" {
  name                    = "${var.prefix}-${var.environment}-webtest"
  location                = var.location
  resource_group_name     = azurerm_resource_group.main.name
  application_insights_id = azurerm_application_insights.main.id
  kind                    = "ping"
  frequency               = 300
  timeout                 = 60
  enabled                 = true
  geo_locations           = ["us-tx-sn1-azr", "us-il-ch1-azr", "us-va-ash-azr"]

  configuration = <<XML
<WebTest Name="WebTest1" Id="ABD48585-0831-40CB-9069-682EA6BB3583" Enabled="True" CssProjectStructure="" CssIteration="" Timeout="0" WorkItemIds="" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010" Description="" CredentialUserName="" CredentialPassword="" PreAuthenticate="True" Proxy="default" StopOnError="False" RecordedResultFile="" ResultsLocale="">
  <Items>
    <Request Method="GET" Guid="a5f10126-e4cd-570d-961c-cea43999a200" Version="1.1" Url="https://${azurerm_function_app.main.default_hostname}" ThinkTime="0" Timeout="300" ParseDependentRequests="True" FollowRedirects="True" RecordResult="True" Cache="False" ResponseTimeGoal="0" Encoding="utf-8" ExpectedHttpStatusCode="200" ExpectedResponseUrl="" ReportingName="" IgnoreHttpStatusCode="False" />
  </Items>
</WebTest>
XML

}