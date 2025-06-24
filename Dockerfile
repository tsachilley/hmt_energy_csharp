#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 440
EXPOSE 4436
EXPOSE 44364
EXPOSE 60000/udp
EXPOSE 30000

WORKDIR /src
RUN ln -sf /usr/share/zoneinfo/Asia/Shanghai /etc/localtime
RUN echo 'Asia/Shanghai' >/etc/timezone

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["NuGet.Config", "."]
COPY ["src/hmt_energy_csharp.HttpApi.Host/hmt_energy_csharp.HttpApi.Host.csproj", "src/hmt_energy_csharp.HttpApi.Host/"]
COPY ["src/hmt_energy_csharp.Application/hmt_energy_csharp.Application.csproj", "src/hmt_energy_csharp.Application/"]
COPY ["src/hmt_energy_csharp.Domain/hmt_energy_csharp.Domain.csproj", "src/hmt_energy_csharp.Domain/"]
COPY ["src/hmt_energy_csharp.Domain.Shared/hmt_energy_csharp.Domain.Shared.csproj", "src/hmt_energy_csharp.Domain.Shared/"]
COPY ["src/hmt_energy_csharp.Util/hmt_energy_csharp.Util.csproj", "src/hmt_energy_csharp.Util/"]
COPY ["src/hmt_energy_csharp.Application.Contracts/hmt_energy_csharp.Application.Contracts.csproj", "src/hmt_energy_csharp.Application.Contracts/"]
COPY ["src/hmt_energy_csharp.EntityFrameworkCore/hmt_energy_csharp.EntityFrameworkCore.csproj", "src/hmt_energy_csharp.EntityFrameworkCore/"]
COPY ["src/hmt_energy_csharp.HttpApi/hmt_energy_csharp.HttpApi.csproj", "src/hmt_energy_csharp.HttpApi/"]
RUN dotnet restore "src/hmt_energy_csharp.HttpApi.Host/hmt_energy_csharp.HttpApi.Host.csproj"
COPY . .
WORKDIR "/src/src/hmt_energy_csharp.HttpApi.Host"
RUN dotnet build "hmt_energy_csharp.HttpApi.Host.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "hmt_energy_csharp.HttpApi.Host.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "hmt_energy_csharp.HttpApi.Host.dll"]