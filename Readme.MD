🧩 Merge API

An ASP.NET Core Web API that merges two integer arrays, stores merge history in a SQLite database, and supports flexible runtime configuration.

🚀 Overview

The Merge API accepts two arrays of integers, merges and sorts them, stores the result in a local SQLite database, and allows lookup of previous merges by result length.


🧱 Features
- 🔢 **Merge two integer arrays** and return the combined, sorted result  
- 💾 **Automatically persists** each merge to a local SQLite database  
- 📜 **Retrieve previous merges** by result length  
- ⚙️ **Environment-controlled validation** — toggle sorted checks and array size limits  
- 🐳 **Works seamlessly inside Docker** using `docker-compose`  
- 🧭 **Swagger UI support** for quick and easy API testing  

🛠️ Tech Stack

| Component         | Technology                              |
|-------------------|------------------------------------------|
| 🧩 Framework       | ASP.NET Core 8                          |
| 🗄️ Database        | SQLite (local file: `/app/data/merge.db`) |
| 🐳 Containerization | Docker & Docker Compose                 |
| 💻 Language        | C#                                      |


Note: Changing environment variables requires a container restart or rebuild.

Docker Setup
- 1️⃣ Build and start
  docker compose up -d --build

- 2️⃣ Verify containers
  docker ps

You should see:
mergeapi   ...   Up ...   0.0.0.0:8080->8080/tcp

- 3️⃣ Access the API

 Open in your browser or Postman:
 Swagger UI — http://localhost:8080/swagger

🧭 Endpoints
POST /api/merge

Merge two arrays and store the result.

Request body
{
  "array1": [1, 3, 5],
  "array2": [2, 4, 6]
}

Response
{
  "result": [1, 2, 3, 4, 5, 6],
  "length": 6,
  "timestamp": "2025-11-13T16:10:23.456Z"
}


If arrays exceed the configured max size or are unsorted (and CHECK_SORTED=true):

{
  "status": 400,
  "message": "Both arrays must be sorted ascending."
}

GET /api/merge/by-length/{length}

Retrieve all previous merges where the resulting array had the specified length.

Example
GET /api/merge/by-length/6

Response
[
  {
    "array1": [1, 3, 5],
    "array2": [2, 4, 6],
    "result": [1, 2, 3, 4, 5, 6],
    "length": 6,
    "timestamp": "2025-11-13T16:10:23.456Z"
  }
]
