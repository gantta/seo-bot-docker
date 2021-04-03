FROM mcr.microsoft.com/azure-functions/dotnet:3.0 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
COPY . /src/dotnet-function-app
RUN cd /src/dotnet-function-app && \
    mkdir -p /home/site/wwwroot && \
    dotnet publish *.csproj --output /home/site/wwwroot
RUN apt-get update && \
    apt-get install -y gnupg wget curl unzip --no-install-recommends && \
    wget -q -O - https://dl-ssl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    echo "deb http://dl.google.com/linux/chrome/deb/ stable main" >> /etc/apt/sources.list.d/google.list && \
    apt-get update -y && \
    apt-get install -y google-chrome-stable && \
    CHROMEVER=$(google-chrome --product-version | grep -o "[^.]*.[^.]*.[^.]*" | sed 1q) && \
    echo "$CHROMEVER" && \
    DRIVERVER=$(curl -s "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROMEVER") && \
    echo "$DRIVERVER" && \
    wget -q --continue -P /chromedriver "http://chromedriver.storage.googleapis.com/$DRIVERVER/chromedriver_linux64.zip" && \
    unzip /chromedriver/chromedriver* -d /usr/bin/

# To enable ssh & remote debugging on app service change the base image to the one below
# FROM mcr.microsoft.com/azure-functions/dotnet:3.0-appservice
FROM base AS final
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true \
    AzureWebJobsStorage="DefaultEndpointsProtocol=https;AccountName=ganttcloudstate997;AccountKey=KAb9rSIaVCzi4qLFnWrlYdnP/NU+GsH5C1IXwjW/o2fGl08flsGXx4nubrTyFQfBLLxw4bWt9MxiK+nNRHB5fA==;EndpointSuffix=core.windows.net" \
    ENVIRONMENT="dev"

COPY --from=build ["/home/site/wwwroot", "/home/site/wwwroot"]
COPY --from=build ["/usr/bin/chromedriver", "/usr/bin/chromedriver"]