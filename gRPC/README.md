# gRPC

## Start

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

## 형식

- bool?, google.protobuf.BoolValue
- double?, google.portobuf.DoubleValue
- flat?, google.protobuf.FloatValue
- int?, google.protobuf.Int32Value
- logn?, google.protobuf.Int64Value
- uint?, google.protobuf.UInt32Value
- ulong?, google.protobuf.UInt64Value
- string, google.protobuf.StringValue
- ByteString, google.protobuf.BytesValue
