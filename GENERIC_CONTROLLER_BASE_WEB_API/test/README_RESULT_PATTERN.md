# Result Pattern Implementation

This document explains how to use the Result pattern in your API responses.

## Overview

The Result pattern provides a standardized way to return responses from your API endpoints. It includes:

- Success flag (bool): Indicates if the operation was successful
- Message (string): Provides information about the operation
- Data (T): Contains the data returned by the operation (generic)
- Errors (List<string>): Contains error messages if the operation failed
- StatusCode (int): HTTP status code for the response

## Classes

### Result

The base `Result` class is used for responses that don't return data:

```csharp
public class Result
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public List<string> Errors { get; set; } = new List<string>();
    public int StatusCode { get; set; }
}
```

### Result<T>

The generic `Result<T>` class is used for responses that return data:

```csharp
public class Result<T> : Result
{
    public T Data { get; set; }
}
```

## Factory Methods

Both classes provide factory methods to create results:

```csharp
// Non-generic Result
Result.Ok(message, statusCode);
Result.Fail(message, errors, statusCode);
Result.NotFound(message);

// Generic Result<T>
Result<T>.Ok(data, message, statusCode);
Result<T>.Fail(message, errors, statusCode);
Result<T>.NotFound(message);
```

## Controller Extensions

Extension methods for `ControllerBase` make it easy to return Result objects:

```csharp
// Return a successful result
this.Ok(message);

// Return a successful result with data
this.Ok(data, message);

// Return a created result with data
this.Created(data, message);

// Return a not found result
this.NotFound(message);

// Return a bad request result with validation errors
this.BadRequest(message, errors);

// Return a no content result
this.NoContent();
```

## ModelState Extensions

Extension methods for `ModelStateDictionary` make it easy to extract validation errors:

```csharp
// Extract validation errors from ModelState
ModelState.GetValidationErrors();
```

## Example Usage

```csharp
// Get all items
[HttpGet]
public async Task<ActionResult> GetAll()
{
    var items = await _service.GetAllAsync();
    return this.Ok(items, "Items retrieved successfully");
}

// Get item by id
[HttpGet("{id}")]
public async Task<ActionResult> GetById(int id)
{
    var item = await _service.GetByIdAsync(id);
    if (item == null)
        return this.NotFound($"Item with ID {id} not found");

    return this.Ok(item, "Item retrieved successfully");
}

// Create a new item
[HttpPost]
public async Task<ActionResult> Create([FromBody] CreateDto createDto)
{
    if (!ModelState.IsValid)
        return this.BadRequest("Invalid data", ModelState.GetValidationErrors());

    var item = await _service.CreateAsync(createDto);
    return this.Created(item, "Item created successfully");
}

// Update an existing item
[HttpPut("{id}")]
public async Task<ActionResult> Update(int id, [FromBody] UpdateDto updateDto)
{
    if (id != updateDto.Id)
        return this.BadRequest("ID mismatch between URL and body");

    if (!ModelState.IsValid)
        return this.BadRequest("Invalid data", ModelState.GetValidationErrors());

    if (!await _service.ExistsAsync(id))
        return this.NotFound($"Item with ID {id} not found");

    var item = await _service.UpdateAsync(updateDto);
    return this.Ok(item, "Item updated successfully");
}

// Delete an item
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(int id)
{
    if (!await _service.ExistsAsync(id))
        return this.NotFound($"Item with ID {id} not found");

    await _service.DeleteAsync(id);
    return this.NoContent();
}
```

## Response Examples

### Successful Response with Data

```json
{
  "success": true,
  "message": "Items retrieved successfully",
  "errors": [],
  "statusCode": 200,
  "data": [
    {
      "id": 1,
      "name": "Item 1"
    },
    {
      "id": 2,
      "name": "Item 2"
    }
  ]
}
```

### Not Found Response

```json
{
  "success": false,
  "message": "Item with ID 999 not found",
  "errors": [],
  "statusCode": 404,
  "data": null
}
```

### Bad Request Response with Validation Errors

```json
{
  "success": false,
  "message": "Invalid data",
  "errors": [
    "Name is required",
    "Price must be greater than 0"
  ],
  "statusCode": 400,
  "data": null
}
``` 