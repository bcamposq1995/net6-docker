# Net6 Docker
My definition of a well structured net6 multi container project, contains the following components:
- Redis (Cache): All get requests are cached, so the next request is retrieved from the redis cache.
- RabbitMQ (Queue Management): Process the asynchronous requests.
- Postgres (SQL Database): Store the people entities
- Consul (Service Discovery): Discover the HttpMicroservice dinamically to Ocelot Gateway and Prometheus.
- Prometheus (Application Performance Manager/Logs): Audit the performance of the application.
- Grafana (APM Viewer): Render performance charts
- Ocelot (Application Gateway): The API gateway to redirect traffic to the microservice, it doesn't have Authorization Mechanisms, but can be added by the Ocelot Authorization Middleware verifying the Bearer token header by OpenId provider such as Azure B2C or Amazon Cognito, LDAP network authorization or another kind of authorization.

## Net6 Project structure
This project can deal with heavy workloads with a very cheap cloud infrastructure due to its asynchronous architecture.
There is an HttpMicroservice receiving requests to execute CRUD (create, read, update, delete) operatioins like a regular API, the async approach has a queue manager to execute and process those CRUD request.

The main goal of this application is manage people entities, with the following properties:
- Id: A UUID field
- FirstName: The person's first name
- LastName: The person's last name
- Emal: The person's email address
- BirthDate: The person's birth date

The solution file, contains the following projects:
- HttpMicroservice: Equeue the requests of the CRUD operations by get, post, patch, put, delete and provides a head method as healt check.
- WorkerMicroservice: Execute the enqueued requests and retrieve the response. It has entity code first/migration feature so it will create the database structure if it doesn't exist.
- API Gateway: The front gateway to redirect the traffic to the proper endpoint.

## Person request and response flows
In order to enqueue requests to process the different verbs, use the url http://localhost:8080/person/{verb}/{id} having {verb} as GET, PUT, PATCH, POST, DELETE and having {id} in case of the GET, PUT, PATCH and DELETE verbs. The localhost address is used by the gateway while running locally with docker-compose. the http://localhost:8081/swagger is the direct address of the person microservice.

### GET by id
- Path: /{id}
- Description: Get a person by id
- Request:
    - Query string: /{id} is the UUID
    - Body: None
- Response: 
    - Body: The 'GetPersonResponse' object
    - Headers: The cache headers
### GET all
- Path: /
- Description: Get all people, this method could be improved by implementing pagination using the query params page number and page size, the retrieve the pages quantity. This method has redis cache.
- Request:
    - Query string: None
    - Body: None
- Response: 
    - Body: The 'GetPersonListResponse' object
    - Headers: The cache headers
### POST
- Path: /
- Description: Create a person entity.
- Request:
    - Query string: None
    - Body: A JSON object representing PostPersonRequest object.
- Response: 
    - Body: The 'PostPersonResponse' object
    - Headers: The person entity location (transaction/post/{transactionId})

### PATCH
- Path: /{id}
- Description: Update the whole person entity.
- Request:
    - Query string: /{id} is the UUID to patch.
    - Body: A JSON object representing PatchPersonRequest object.
- Response: 
    - Body: The 'PatchPersonResponse' object
    - Headers: None

### PUT
- Path: /{id}
- Description: Update the specified person entity field.
- Request:
    - Query string: /{id} is the UUID to put.
    - Body: A JSON object representing PutPersonRequest object.
- Response: 
    - Body: The 'PutPersonResponse' object
    - Headers: None

### DELETE
- Path: /{id}
- Description: Delete the person by {id}.
- Request:
    - Query string: /{id} is the UUID to delete.
    - Body: None.
- Response: 
    - Body: The 'DeletePersonResponse' object
    - Headers: None

### HEAD
- Path: /
- Description: Health probe endpoint.
- Request:
    - Query string: None.
    - Body: None.
- Response: 
    - Body: None.
    - Headers: None.
    - Status code 200

## Transaction request and response flows
In order to get the response of the transaction enqueued in the person microservice, send the query param {transactionId} returned from the {Verb}PersonResponse to the following methods:

### GET by id
- Path: /transaction/get-by-id/{transactionId}
- Description: Get the get by id transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the GetPersonResponse object.
    - Body: None.
- Response: 
    - Body: The GetPersonResponse object.
    - Headers: The cache headers.

### GET all
- Path: /transaction/get-all/{transactionId}
- Description: Get the get all transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the GetPersonListResponse object.
    - Body: None.
- Response: 
    - Body: The GetPersonListResponse object.
    - Headers: The cache headers.

### POST
- Path: /transaction/post/{transactionId}
- Description: Get the post transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the PostPersonResponse object.
    - Body: None.
- Response: 
    - Body: The PostPersonResponse object.
    - Headers: The cache headers.

### PUT
- Path: /transaction/put/{transactionId}
- Description: Get the put transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the PutPersonResponse object.
    - Body: None.
- Response: 
    - Body: The PutPersonResponse object.
    - Headers: The cache headers.

### PATCH
- Path: /transaction/patch/{transactionId}
- Description: Get the patch transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the PatchPersonResponse object.
    - Body: None.
- Response: 
    - Body: The PatchPersonResponse object.
    - Headers: The cache headers.

### DELETE
- Path: /transaction/delete/{transactionId}
- Description: Get the delete transaction details and response.
- Request:
    - Query string: {/transactionId} The transaction id given in the DeletePersonResponse object.
    - Body: None.
- Response: 
    - Body: The DeletePersonResponse object.
    - Headers: The cache headers.

## Running the application
### Compiling the project
This solution does not build as the Visual Studio docker build support does, because it takes a long time to finish the build and that is not functional for a development environment.
Execute the command ```dotnet build -c Release``` on each Net6 project folder (HttpMicroservice, WorkerMicroservice and APIGateway)

### Executing docker compose
Once each project already has the bin/Release/publish folder, you are able to run in the root folder the following command: ```docker-compose up -d````

## Environment requirements
- Docker: Follow this guid to install docker desktop https://docs.docker.com/desktop
- Docker compose: Follow this guid to install docker compose https://docs.docker.com/compose/install
- Net6: Download the latest release for Net6 SDK https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- VS Code: Download the best code editor https://code.visualstudio.com
    - VS Code c# extension: https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp

# My contact info
- GitHub: https://github.com/bcamposq1995
- LinkedIn: https://www.linkedin.com/in/bryan-campos-a04a9689
- Email: bcamposq1995@hotmail.com, Subject: Net6 Docker