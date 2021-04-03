FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS installer-env

COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM mcr.microsoft.com/azure-functions/dotnet:3.0
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    AzureWebJobsStorage="DefaultEndpointsProtocol=https;AccountName=ganttcloudstate997;AccountKey=KAb9rSIaVCzi4qLFnWrlYdnP/NU+GsH5C1IXwjW/o2fGl08flsGXx4nubrTyFQfBLLxw4bWt9MxiK+nNRHB5fA==;EndpointSuffix=core.windows.net"

COPY --from=installer-env ["/home/site/wwwroot", "/home/site/wwwroot"]