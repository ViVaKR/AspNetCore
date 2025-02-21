# gRPC

```bash
$ dotnet new grpc -o GrpcGreeter

dotnet add GrpcGreeterClient.csproj package Grpc.Net.Client
dotnet add GrpcGreeterClient.csproj package Google.Protobuf
dotnet add GrpcGreeterClient.csproj package Grpc.Tools

dotnet add package Grpc.AspNetCore

dotnet add package Grpc.Net.Client
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools

# Server
dotnet add package Grpc.AspNetCore.Server.Reflection

# homebrew
brew install grpcui
grpcui localhost:<port>
```

```xml
  <ItemGroup>
    <Protobuf Include="Protos\greet.proto" GrpcServices="Client" />
  </ItemGroup>

```
