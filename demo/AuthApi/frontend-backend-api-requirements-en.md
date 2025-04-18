# Frontend to Backend API Requirements

This document defines our expectations as the frontend team from the backend API implementation. An API that includes the following features and standards is critical for seamless integration with our frontend application.

## API Response Standard

All API responses should conform to the following schema:

```json
{
  "success": true/false,
  "message": "User-friendly message about the operation",
  "data": {}, // Operation-specific data (in successful responses)
  "errors": {}, // Validation errors (in failed responses)
  "statusCode": 200 // HTTP status code
}
```

This structure will enable us to establish a consistent error handling and user notification mechanism in the frontend.

## HTTP Status Codes

The API should appropriately use the following HTTP status codes:

- **200 OK**: Successful GET, PUT, PATCH requests
- **201 Created**: Successful POST requests (new resource creation)
- **400 Bad Request**: Validation errors, invalid request format
- **401 Unauthorized**: Authentication error, invalid token
- **403 Forbidden**: Authenticated but unauthorized access
- **404 Not Found**: Resource not found
- **409 Conflict**: Conflict situation (e.g., same username/email)
- **429 Too Many Requests**: Rate limit exceeded
- **500 Server Error**: Server-side errors

## Validation Mechanism

We want backend validation that aligns with frontend form validations:

1. All validation errors should be grouped by field name in the `errors` field of the standard API response format
2. Errors for each field should be in an array (multiple errors possible)
3. Error messages should be user-friendly

Example validation error response:

```json
{
  "success": false,
  "message": "Please check the form fields",
  "errors": {
    "username": ["Username is required", "Username must be at least 3 characters"],
    "password": ["Password must be at least 8 characters", "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"]
  },
  "statusCode": 400
}
```

## Authentication and Authorization

We expect a JWT-based authentication system:

1. Access token: 11 hours validity
2. Refresh token: 7 days validity
3. `Authorization: Bearer {token}` header check for all secure endpoints
4. Two-factor authentication support

## Rate Limiting

Rate limiting should be implemented for security and performance:

1. Special limits for critical endpoints in addition to global limit:
   - Login endpoints: 10 requests per 5 minutes
   - Registration endpoints: 3 requests per 10 minutes
   - Password reset: 3 requests per 30 minutes
   - Sensitive data operations: 5 requests per 5 minutes

2. Return appropriate HTTP 429 response when rate limit is exceeded

## Example Request/Response Formats

This section provides general examples of expected request and response formats. Backend developers should design their endpoints according to these formats.

### Example 1: Data Listing Endpoint

#### Request
```
GET /api/items?page=1&pageSize=10&sortBy=createdAt&sortDirection=desc
```

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Records retrieved successfully",
  "data": {
    "items": [
      {
        "id": 1,
        "name": "Example Item 1",
        "description": "Description text",
        "category": "Category A",
        "price": 149.99,
        "imageUrl": "https://example.com/images/item1.jpg",
        "createdAt": "2023-04-30T12:00:00Z"
      },
      {
        "id": 2,
        "name": "Example Item 2",
        "description": "Description text 2",
        "category": "Category B",
        "price": 89.99,
        "imageUrl": "https://example.com/images/item2.jpg",
        "createdAt": "2023-04-29T15:30:00Z"
      }
      // ... other items
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 10,
      "totalItems": 42,
      "totalPages": 5,
      "hasNext": true,
      "hasPrevious": false
    }
  },
  "statusCode": 200
}
```

### Example 2: Single Record Retrieval Endpoint

#### Request
```
GET /api/items/1
```

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Record retrieved successfully",
  "data": {
    "id": 1,
    "name": "Example Item 1",
    "description": "Detailed description text",
    "category": "Category A",
    "price": 149.99,
    "stock": 25,
    "imageUrl": "https://example.com/images/item1.jpg",
    "images": [
      "https://example.com/images/item1_1.jpg",
      "https://example.com/images/item1_2.jpg",
      "https://example.com/images/item1_3.jpg"
    ],
    "specifications": {
      "weight": "1.2 kg",
      "dimensions": "15 x 10 x 5 cm",
      "color": "Black"
    },
    "createdAt": "2023-04-30T12:00:00Z",
    "updatedAt": "2023-05-01T09:15:00Z"
  },
  "statusCode": 200
}
```

#### Record Not Found Response (404 Not Found)
```json
{
  "success": false,
  "message": "Record not found",
  "statusCode": 404
}
```

### Example 3: Record Creation Endpoint

#### Request
```
POST /api/items
```

```json
{
  "name": "New Item",
  "description": "New item description",
  "category": "Category C",
  "price": 199.99,
  "stock": 10,
  "specifications": {
    "weight": "0.8 kg",
    "dimensions": "12 x 8 x 4 cm",
    "color": "Blue"
  }
}
```

#### Successful Response (201 Created)
```json
{
  "success": true,
  "message": "Record created successfully",
  "data": {
    "id": 43,
    "name": "New Item",
    "description": "New item description",
    "category": "Category C",
    "price": 199.99,
    "stock": 10,
    "imageUrl": null,
    "specifications": {
      "weight": "0.8 kg",
      "dimensions": "12 x 8 x 4 cm",
      "color": "Blue"
    },
    "createdAt": "2023-05-15T14:22:00Z",
    "updatedAt": "2023-05-15T14:22:00Z"
  },
  "statusCode": 201
}
```

#### Validation Error Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Please check the form fields",
  "errors": {
    "name": ["Name field is required"],
    "price": ["Price must be greater than 0"],
    "category": ["Invalid category value"]
  },
  "statusCode": 400
}
```

### Example 4: Record Update Endpoint

#### Request
```
PUT /api/items/43
```

```json
{
  "name": "Updated Item",
  "description": "Updated description",
  "price": 179.99,
  "stock": 15
}
```

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Record updated successfully",
  "data": {
    "id": 43,
    "name": "Updated Item",
    "description": "Updated description",
    "category": "Category C",
    "price": 179.99,
    "stock": 15,
    "imageUrl": null,
    "specifications": {
      "weight": "0.8 kg",
      "dimensions": "12 x 8 x 4 cm",
      "color": "Blue"
    },
    "createdAt": "2023-05-15T14:22:00Z",
    "updatedAt": "2023-05-15T15:05:00Z"
  },
  "statusCode": 200
}
```

### Example 5: Record Deletion Endpoint

#### Request
```
DELETE /api/items/43
```

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Record deleted successfully",
  "statusCode": 200
}
```

### Example 6: File Upload Endpoint

#### Request
```
POST /api/items/43/images
```
Image file submission via multipart form data

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Image uploaded successfully",
  "data": {
    "imageUrl": "https://example.com/images/item43_1.jpg",
    "thumbnailUrl": "https://example.com/images/thumbnails/item43_1.jpg"
  },
  "statusCode": 200
}
```

#### Invalid File Error Response (400 Bad Request)
```json
{
  "success": false,
  "message": "Invalid file format",
  "errors": {
    "image": ["Only JPG, PNG, and GIF files are accepted", "File size must be less than 5MB"]
  },
  "statusCode": 400
}
```

### Example 7: Filtering and Search Endpoint

#### Request
```
GET /api/items/search?query=example&category=A,B&minPrice=50&maxPrice=200&page=1&pageSize=20
```

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Search results retrieved successfully",
  "data": {
    "items": [
      // Search results
    ],
    "pagination": {
      "currentPage": 1,
      "pageSize": 20,
      "totalItems": 7,
      "totalPages": 1,
      "hasNext": false,
      "hasPrevious": false
    },
    "filters": {
      "appliedFilters": {
        "query": "example",
        "category": ["A", "B"],
        "minPrice": 50,
        "maxPrice": 200
      },
      "availableCategories": [
        {"id": "A", "name": "Category A", "count": 5},
        {"id": "B", "name": "Category B", "count": 2},
        {"id": "C", "name": "Category C", "count": 0}
      ],
      "priceRange": {
        "min": 49.99,
        "max": 199.99
      }
    }
  },
  "statusCode": 200
}
```

## Error Handling Expectations

1. **Login error (401):**
```json
{
  "success": false,
  "message": "Invalid username or password",
  "statusCode": 401
}
```

2. **Same username/email usage (409):**
```json
{
  "success": false,
  "message": "This username is already in use",
  "statusCode": 409
}
```

3. **Password policy error (400):**
```json
{
  "success": false,
  "message": "Please check the form fields",
  "errors": {
    "password": ["Password must be at least 8 characters", 
                "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character"]
  },
  "statusCode": 400
}
```

4. **Invalid file error (400):**
```json
{
  "success": false,
  "message": "Unsupported file format. Please upload an image in JPG, PNG, or GIF format",
  "statusCode": 400
}
```

5. **Rate limiting error (429):**
```json
{
  "success": false,
  "message": "Too many requests. Please try again after X seconds.",
  "data": {
    "retryAfterSeconds": 30
  },
  "statusCode": 429
}
```

## Security Requirements

1. HTTPS requirement
2. Password policy: minimum 8 characters, at least 1 uppercase letter, 1 lowercase letter, 1 number, 1 special character
3. CORS configuration (frontend URLs in allowed origins list)
4. Rate limiting implementation
5. Token rotation (generating new refresh token with each refresh token usage)
6. Data masking for sensitive information (logs, error messages)

## Performance Expectations

1. API response time: less than 300ms (95th percentile)
2. CDN usage for user profile pictures
3. Asynchronous processing for heavy operations
4. Pagination support for large data sets

## Testing and Documentation Requirements

1. API documentation with Swagger/OpenAPI interface
2. Example request and response information for each endpoint
3. Dummy accounts and test data for testing environment

## Profile Picture Upload Requirements

### Profile Picture Upload Endpoint

#### Request
```
POST /api/Users/profile-picture
```
Image file submission via multipart form data

#### Successful Response (200 OK)
```json
{
  "success": true,
  "message": "Profile picture uploaded successfully",
  "data": {
    "profilePictureUrl": "/api/Users/{userId}/profile-picture"
  },
  "statusCode": 200
}
```

#### Validation Error (400 Bad Request)
```json
{
  "success": false,
  "message": "Invalid profile picture",
  "errors": {
    "file": [
      "Profile picture must be square format (e.g., 200x200)",
      "Profile picture must be maximum 1000x1000 pixels",
      "File size must be maximum 5MB",
      "Only JPG, PNG, and GIF formats are supported"
    ]
  },
  "statusCode": 400
}
```

### Profile Picture Retrieval Endpoint

#### Request
```
GET /api/Users/{userId}/profile-picture
```

#### Successful Response
- Content-Type: image/png, image/jpeg, or image/gif
- Binary image data

### Profile Picture Technical Requirements

1. **Image Format and Size**
   - Supported formats: JPG, PNG, GIF
   - Maximum file size: 5MB
   - Image dimensions: Square format (width = height)
   - Maximum size: 1000x1000 pixels
   - Recommended size: 200x200 pixels

2. **Image Processing**
   - Uploaded images should be automatically optimized
   - Large images should be resized to recommended dimensions
   - Image quality should be preserved (minimum 80% quality for JPEG)
   - EXIF data should be cleaned

3. **Storage and CDN**
   - Images should be stored on CDN
   - CDN URL structure: `https://cdn.example.com/profiles/{userId}.{ext}`
   - CDN cache duration: 1 hour
   - CDN cache should be cleared when image is updated
   - Separate CDN path for default profile picture: `https://cdn.example.com/profiles/default.png`

4. **Security**
   - File content and MIME type validation
   - Malware scanning
   - Maximum upload size limit
   - Only authenticated users can upload images
   - Rate limiting: maximum 3 image uploads per 5 minutes

5. **Performance**
   - Image compression and optimization
   - Progressive loading support
   - WebP format support (based on browser compatibility)
   - Responsive image support (for different dimensions)

6. **Error Handling**
   - Invalid file format
   - Size limit exceeded
   - Image dimension mismatch
   - Storage/CDN errors
   - Rate limit exceeded

---

This document serves as a basic guide for developing an API that meets our frontend team's expectations. An API that adheres to these standards will improve user experience and facilitate frontend-backend integration. 