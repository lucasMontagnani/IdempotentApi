# IdempotentApi

## Overview

This project, IdempotencyAPI, was created to explore different ways to handle API idempotency. Existing NuGet packages caused conflicts with parts of the code, leading to errors, so I implemented custom solutions.

To demonstrate these solutions, the project includes several endpoints that validate idempotency in different ways. An in-memory database is used with a single Product model to test the results.

## Endpoints
### Product Retrieval
<UL>
  <LI>GET /GetProductsById - Retrieves a product by its ID.</LI>
  <LI>GET /GetProducts - Retrieves all products currently stored in the database.</LI>
</UL>

### Product Creation

<UL>
  <LI>POST /CreateProduct - A standard product creation endpoint (control case) without idempotency validation.</LI>
</UL>

### Idempotent Product Creation

Each of the following endpoints requires a header Idempotency-Key, which must be a valid GUID.
<UL>
  <LI>POST /CreateProductIdempotentWithServiceInjection</LI>
      <UL><LI>Uses an IdempotencyService, injected via Dependency Injection in the controller, to validate the request.</LI></UL>
  <LI>POST /CreateProductIdempotentWithFilter</LI>
      <UL><LI>Uses an Action Filter triggered by the [ServiceFilter] attribute on the endpoint to validate idempotency.</LI></UL>
  <LI>POST /CreateProductIdempotentWithMiddleware</LI>
      <UL><LI>Uses Middleware to validate idempotency. This is applied globally to all POST requests that include the Idempotency-Key header.</LI></UL>
</UL>

## Comparison of Idempotency Approaches

<table>
  <tr>
    <th>Approach</th>
    <th>Pros</th>
    <th>Cons</th>
  </tr>
  <tr>
    <td>Service Injection</td>
    <td>Simple to implement</td>
    <td>Increases code dependency and coupling</td>
  </tr>
  <tr>
    <td>Action Filter</td>
    <td>Loose coupling, applies idempotency selectively</td>
    <td>Slightly more setup required</td>
  </tr>
  <tr>
    <td>Middleware</td>
    <td>Loose coupling, automatically applies to all POSTs</td>
    <td>Applies idempotency to all POST endpoints</td>
  </tr>
</table>

## Conclusion

Among the three approaches, Action Filters offer the best balance between flexibility and maintainability, allowing idempotency to be applied selectively while keeping the code loosely coupled.
