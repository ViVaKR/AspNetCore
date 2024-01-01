# KR.CO.TEXT.API

## Start

```bash

    $ dotnet new webapi --use-controllers -o KR.CO.TEXT.API
    $ cd KR.CO.TEXT.API
    $ code -r .

    # enable https
    $ dotnet dev-certs https --trust
    $ dotnet run --launch-profile https

```

## WebAPI ( DBL + BLL, MiddleWare, Data [XML, Json] )

$DB\,\rightarrow\,\left( DBL\,\rightarrow\,BLL\right)\,\rightarrow\,UI (HTML, Angular, React)$


## Web API Request & Response
* Request
  * Verb (http actions) : Get, Post, Put, Delete, Patch
  * Headers (metadata) : Content type, Content length, Authorization (인증토큰)
  * Content (Data)
* Response
  * Status Code
    * 100-199 Information
    * 200 - 299 Success
      * 200 - OK
      * 201 - Created
      * 204 - No Content
    * 300 - 399 Redirection
    * 400 - 499 Client Errors
      * 400 - Bad Request
      * 401 - UnAuthorised
      * 404 - Not Found
      * 409 - Conflict
    * 500 - 599 Server Errors
      * 500 - Internal Server Error
  * Headers (Metadata)
  * Content (Data)


## Add New Controller Example

`$ dotnet new apicontroller -o Controllers -n DemoController --namespace KR.CO.TEXT.API.Controllers`
