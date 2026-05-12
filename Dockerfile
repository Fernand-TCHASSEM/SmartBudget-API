FROM mcr.microsoft.com/dotnet/sdk:10.0

ENV APP_HOME=/app
ENV TZ=America/Toronto
ENV DOTNET_ROOT="/usr/share/dotnet"
ENV PATH="/usr/share/dotnet:${PATH}"

RUN ln -sf /usr/share/dotnet/dotnet /usr/local/bin/dotnet

# Install dependencies and set timezone
RUN apt-get update \
    && apt-get install -y tzdata default-mysql-client nodejs npm git \
    && npm install -g @anthropic-ai/claude-code \
    && ln -snf /usr/share/zoneinfo/${TZ} /etc/localtime \
    && echo ${TZ} > /etc/timezone \
    && rm -rf /var/lib/apt/lists/*

# Give ubuntu user ownership of /app
RUN mkdir -p ${APP_HOME} \
    && chown -R ubuntu:ubuntu ${APP_HOME}

USER ubuntu

# NuGet cache inside /app — persisted via Docker volume
ENV NUGET_PACKAGES=${APP_HOME}/.nuget/packages

# dotnet tools
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/home/ubuntu/.dotnet/tools"

WORKDIR ${APP_HOME}

CMD ["sleep", "infinity"]