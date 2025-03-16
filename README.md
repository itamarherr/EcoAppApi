# EcoAdviceApp Backend

?? **EcoAdviceApp Backend** is a scalable and efficient backend built using **.NET 8 and Entity Framework Core** to support a **React frontend**. It provides **user authentication, order management, and environmental advice functionality**.

This project is **under active development**, with additional features being implemented.

---

## **?? Features**
- ? **Orders CRUD** - Fully implemented.
- ?? **Products CRUD** - In progress.
- ? **User Authentication & Role Management**.
- ? **Integration with SQL Server using Entity Framework Core**.
- ? **Unit Testing with xUnit and InMemory Database**.

---

## **?? Project Structure**
EcoAdviceAppBackend/ ??? DAL/ # Data Access Layer (Entity Framework) ??? Services/ # Business Logic Layer ??? Controllers/ # API Endpoints ??? Models/ # Data Models ??? Utils/ # Utility Classes ??? xUnitTests/ # Unit Tests (Order deletion, CRUD validation) ??? appsettings.json # Configuration File ??? Program.cs # API Entry Point ??? README.md # Project Documentation

yaml
Copy
Edit

---

## **?? Installation & Setup**
### **1?? Clone the Repository**
```bash
git clone https://github.com/yourusername/EcoAdviceAppBackend.git
cd EcoAdviceAppBackend
2?? Set Up the Database
Update the connection string in appsettings.json:
json
Copy
Edit
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER;Database=EcoAdviceDB;Trusted_Connection=True;"
}
Run migrations:
bash
Copy
Edit
dotnet ef database update
3?? Run the Application
bash
Copy
Edit
dotnet run
API will be available at:
?? https://localhost:5001 (HTTPS)
?? http://localhost:5000 (HTTP)

?? Running Unit Tests
This project includes xUnit tests with an InMemory Database to ensure reliable and repeatable results.

Running Tests
bash
Copy
Edit
dotnet test
Test Organization
Services - Validates service layer logic.
Repositories - Tests database interactions.
Utilities - Ensures correct pricing and data transformations.
Example Tests

CreateOrder_ShouldAddOrderToDatabase
DeleteOrder_ShouldThrowExceptionWhenOrderNotFound
?? API Endpoints
Method	Endpoint	Description	Auth
POST	/api/auth/register	Register a new user	?
POST	/api/auth/login	Login and get JWT token	?
GET	/api/orders	Get all orders	? Admin
DELETE	/api/orders/{id}	Delete order by ID	? Admin
POST	/api/posts	Create a new post	? User
?? Current Development Focus
?? In Progress:

Expanding role-based access control.
Improving validation & business logic.
Completing Product CRUD functionality.
Enhancing API documentation with Swagger.
?? License
This project is open-source under the MIT License.

?? Contributors
Itamar Herr ?????
For any questions or contributions, feel free to open an issue or contact the repository maintainer.

