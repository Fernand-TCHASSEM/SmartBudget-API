#!/bin/bash
# set -e

# Si le projet n'existe pas encore, on l'initialise
if [ ! -f "SmartBudget.sln" ]; then
  echo "Initialisation du projet SmartBudget..."

  dotnet new sln -n SmartBudget

  dotnet new webapi   -n SmartBudget.API
  dotnet new classlib -n SmartBudget.Application
  dotnet new classlib -n SmartBudget.Domain
  dotnet new classlib -n SmartBudget.Infrastructure
  dotnet new xunit    -n SmartBudget.Tests

  dotnet sln add SmartBudget.API/SmartBudget.API.csproj
  dotnet sln add SmartBudget.Application/SmartBudget.Application.csproj
  dotnet sln add SmartBudget.Domain/SmartBudget.Domain.csproj
  dotnet sln add SmartBudget.Infrastructure/SmartBudget.Infrastructure.csproj
  dotnet sln add SmartBudget.Tests/SmartBudget.Tests.csproj

  dotnet add SmartBudget.API/SmartBudget.API.csproj \
    reference SmartBudget.Application/SmartBudget.Application.csproj
  dotnet add SmartBudget.API/SmartBudget.API.csproj \
    reference SmartBudget.Infrastructure/SmartBudget.Infrastructure.csproj
  dotnet add SmartBudget.Application/SmartBudget.Application.csproj \
    reference SmartBudget.Domain/SmartBudget.Domain.csproj
  dotnet add SmartBudget.Infrastructure/SmartBudget.Infrastructure.csproj \
    reference SmartBudget.Application/SmartBudget.Application.csproj
  dotnet add SmartBudget.Tests/SmartBudget.Tests.csproj \
    reference SmartBudget.Application/SmartBudget.Application.csproj

  echo "Projet initialisé."
else
  echo "Projet déjà existant, démarrage..."
fi

# Maintenir le container en vie pour travailler dedans
# exec sleep infinity