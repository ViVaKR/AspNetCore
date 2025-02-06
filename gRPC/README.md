# gRPC

```bash
dotnet add package Grpc.AspNetCore

dotnet add GrpcWriterOrKrClient package Grpc.Net.Client
dotnet add package Google.Protobuf
dotnet add package Grpc.Tools

# Server
dotnet add package Grpc.AspNetCore.Server.Reflection

# homebrew
brew install grpcui
$ grpcui localhost:<port>
```

```xml
  <ItemGroup>
    <Protobuf Include="Protos\greet.proto" GrpcServices="Client" />
  </ItemGroup>

```
