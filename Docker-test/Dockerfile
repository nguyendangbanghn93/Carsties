FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
WORKDIR /app

# Copy file csproj và restore các dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ mã nguồn và build dự án
COPY . ./
RUN dotnet publish -c Release -o out

EXPOSE 7003

# Khởi động ứng dụng
ENTRYPOINT ["dotnet", "watch"]