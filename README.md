# CRMUtils
A set of utilities for interacting with Dynamics CRM

# Building
Standard c# build process. Just restore nuget packages and then build with Visual Studio or msbuild

# Usage

Install the nuget package Wycliffeassociates.CRMUtils

# Extensions

## IOrganizationService.RetrieveAll()

Gets all pages of a result and returns them as a List<Entity>

Example
```
using CRMUtils.Extensions;

// Get all contacts
QueryExpression query = new QueryExpression("contact");
List<Entity> result = service.RetrieveAll(query);
```

You can also add custom paging to the query if you don't want to use the standard paging

# Helpers

## Bulk Execute Helper
This helper allows you to create a large batch of CRM API calls and not have to worry about paging

Example
```
using CRMUtils.BulkExecuteHelper;

BulkExecuteHelper batch = new BulkExecuteHelper();
batch.Add(new CreateRequest() { Target = new Entity("contact", Guid.NewGuid()) });
batch.Execute(service)
```

You can also supply a callback action if you want

Example
```
using CRMUtils.BulkExecuteHelper;

BulkExecuteHelper batch = new BulkExecuteHelper();
batch.Add(new CreateRequest() { Target = new Entity("contact", Guid.NewGuid()) }, (response) =>
{
    Console.WriteLine(((CreateResponse)response).id);
});
batch.Execute(service)
```