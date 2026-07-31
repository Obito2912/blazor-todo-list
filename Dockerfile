# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY blazor-todo-list.csproj ./
RUN dotnet restore "blazor-todo-list.csproj"

COPY . .
RUN dotnet publish "blazor-todo-list.csproj" -c Release -o /app/publish --no-restore

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

RUN mkdir -p /app/data
VOLUME ["/app/data"]

COPY --from=build /app/publish .

ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "blazor-todo-list.dll"]
# -