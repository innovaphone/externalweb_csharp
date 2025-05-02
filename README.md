C# Web Server with Session Management and Authentication

This is a simple C# web server that uses ASP.NET Core to handle sessions, perform authentication, and serve static files. It features a basic authentication mechanism with challenge-response flow to secure access to an HTML page (app.htm).
Features

    Session Management: User session data is stored in-memory.

    Challenge-Response Authentication: Uses a hashed digest to verify credentials.

    Login Mechanism: /login endpoint accepts authentication requests.

    Access Control: Protects access to /app.htm page based on authentication.

Getting Started

    Clone or download this repository.

    Open the solution in your preferred C# development environment (e.g., Visual Studio, Visual Studio Code).

    Install dependencies by running:

dotnet restore

Build and run the project:

    dotnet run

    The server will run at https://localhost:8181.

Authentication Flow

    AppChallenge: When accessing /login?mt=AppChallenge, the server responds with a randomly generated challenge string, which must be used in the authentication request.

    AppLogin: The client sends credentials along with the challenge response. The server verifies the request using a SHA256 hash of the authentication data and responds with either a success or failure message.

Endpoints

    /login:

        GET /login?mt=AppChallenge: Initiates the challenge-response authentication.

        GET /login?mt=AppLogin: Verifies the login based on the challenge and digest.

    /app.htm: The protected page which is only accessible after successful login. If the user is not authenticated, a 403 Forbidden response is returned.

Session Management

    SetBool: Stores a boolean value in the session.

    GetBool: Retrieves the stored boolean value from the session.

Security Considerations

    HTTPS: The server is configured to use HTTPS to ensure secure communication.

    Session Cookies: The server uses secure cookies, and cross-site requests are allowed using SameSite=None and SecurePolicy.Always.

Helper Methods

    GenerateRandomString: Generates a random string of a specified length for use as the challenge string.

License

This project is licensed under the MIT License - see the LICENSE file for details.
