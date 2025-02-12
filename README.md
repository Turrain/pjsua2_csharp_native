# pjsua2_csharp_native

This project provides a C# wrapper for the PJSUA2 library, enabling native integration with PJSIP functionalities.

## Setup Instructions

1. Clone the repository:
    ```sh
    git clone https://github.com/yourusername/pjsua2_csharp_native.git
    ```

2. Navigate to the project directory:
    ```sh
    cd pjsua2_csharp_native
    ```

3. Build the project:
    ```sh
    dotnet build
    ```

4. Run the project:
    ```sh
    dotnet run
    ```

## Usage

Provide examples of how to use the wrapper in your C# projects.

```csharp
using Pjsua2;

class Program
{
    static void Main(string[] args)
    {
        // Initialize PJSUA2
        Endpoint ep = new Endpoint();
        ep.libCreate();

        // ...additional setup and usage...
    }
}
```
