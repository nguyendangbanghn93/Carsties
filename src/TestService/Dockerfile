FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
WORKDIR /app/TestService

COPY *.csproj ./
RUN dotnet restore


COPY . ./
RUN dotnet publish -c Release -o out

ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8000","--force"]


# FROM mcr.microsoft.com/dotnet/sdk:8.0.300 AS build
# WORKDIR /app

# # Copy file csproj và restore các dependencies
# COPY *.csproj ./
# RUN dotnet restore

# # Copy các tệp và thư mục từ dự án Contracts.csproj
# COPY /app/../Contracts/*.csproj /app/../Contracts/
# RUN dotnet restore ../Contracts/Contracts.csproj

# # Copy toàn bộ mã nguồn và build dự án
# COPY . ./
# RUN dotnet publish -c Release -o out

# EXPOSE 8000

# # Khởi động ứng dụng
# ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:8000"]

