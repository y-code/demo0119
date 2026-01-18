FROM mcr.microsoft.com/devcontainers/dotnet:9.0-noble

RUN curl https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > microsoft.gpg
RUN mv microsoft.gpg /etc/apt/trusted.gpg.d/microsoft.gpg
RUN sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-$(lsb_release -cs)-prod $(lsb_release -cs) main" > /etc/apt/sources.list.d/dotnetdev.list'
RUN apt-get update && apt-get install -y azure-functions-core-tools-4 \
    && rm -rf /var/lib/apt/lists/*

RUN dotnet new install Microsoft.Azure.Functions.Worker.ProjectTemplates
RUN dotnet new install Microsoft.Azure.Functions.Worker.ItemTemplates

