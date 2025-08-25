
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
  

COPY Payment.csproj .
RUN dotnet restore "Payment.csproj"
COPY app.db .
COPY . .
RUN dotnet publish "Payment.csproj" -c Release -o /app/publish


FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Payment.dll"]