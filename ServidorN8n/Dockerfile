FROM n8nio/n8n:latest

USER root

# Instalar dependencias básicas en Alpine
RUN apk add --no-cache bash icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib \
    curl openssl

# Descargar e instalar .NET SDK 8.0 para Alpine x64
RUN curl -L https://dotnet.microsoft.com/download/dotnet/scripts/v1/dotnet-install.sh -o dotnet-install.sh \
    && chmod +x dotnet-install.sh \
    && ./dotnet-install.sh --version 8.0.303 --install-dir /usr/share/dotnet \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet \
    && rm dotnet-install.sh

# Volver al usuario que usa n8n
USER node

EXPOSE 5678
