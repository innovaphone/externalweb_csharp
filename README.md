## C# Web Server with Session Management and Authentication

This is a simple C# web server using ASP.NET Core to handle sessions, perform authentication, and serve static files. It features a basic authentication mechanism with a challenge-response flow to secure access to an HTML page (`app.htm`).

### Features

* **Session Management**: User session data is stored in memory.
* **Challenge-Response Authentication**: Uses a hashed digest to verify credentials.
* **Login Mechanism**: `/login` endpoint accepts authentication requests.
* **Access Control**: Protects access to `/app.htm` based on authentication.

### Getting Started

1. Clone or download this repository.
2. Open the solution in your preferred C# development environment (e.g., Visual Studio, Visual Studio Code).
3. Install dependencies by running:

   ```bash
   dotnet restore
   ```
4. Build and run the project:

   ```bash
   dotnet run
   ```

   The server will run at `https://localhost:8181`.

### Authentication Flow

* **AppChallenge**: When accessing `/login?mt=AppChallenge`, the server responds with a randomly generated challenge string.
* **AppLogin**: The client sends credentials along with the challenge response. The server verifies the request using a SHA256 hash and responds with either a success or failure message.

#### Endpoints

* **/login**:

  * `GET /login?mt=AppChallenge`: Initiates the challenge-response authentication.
  * `GET /login?mt=AppLogin`: Verifies login based on the challenge and digest.
  * **Note**: `var appPwd = "pwd";` is a shared secret used for demo purposes. You should replace this in production.

* **/app.htm**: The protected page only accessible after successful login. If the user is not authenticated, a 403 Forbidden response is returned.

### Session Management

* **SetBool**: Stores a boolean value in the session.
* **GetBool**: Retrieves the stored boolean value from the session.

### Security Considerations

* **HTTPS**: Configured to use HTTPS for secure communication.
* **Session Cookies**: Secure cookies are used, with `SameSite=None` and `SecurePolicy.Always`.

### Helper Methods

* **GenerateRandomString**: Generates a random string of a specified length for use as the challenge string.

### Integration in MyApps / PBX Environment

1. Create an app object.
2. Define the app name (you can choose this).
3. Set the shared password (example: `pwd`) in both PBX and the web service.
4. Use the web service URL (e.g., `https://192.168.2.100:8181/index.htm`), replacing the IP as necessary.

### License

This project is completely free to use, modify, and distribute.
