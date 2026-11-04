```
TriviaPruebaTecnica/
│
├── Controllers/
│   ├── AccountController.cs       <-- Maneja Registro y Login (Autenticación y Cookies)
│   └── TriviaController.cs        <-- Maneja la vista de Categorías y la Partida de Trivia
│
├── Models/
│   ├── User.cs                    <-- Entidad de usuario
│   ├── Category.cs                <-- Entidad de categorías (Animales, Carros)
│   ├── Question.cs                <-- Entidad de preguntas
│   ├── Answer.cs                  <-- Entidad de respuestas (con su indicador IsCorrect)
│   │
│   └── ViewModels/
│       ├── LoginViewModel.cs      <-- Datos para el formulario de login
│       ├── RegisterViewModel.cs   <-- Datos para el formulario de registro
│       └── GameViewModel.cs       <-- Estructura para pintar la partida (3 preguntas y sus respuestas)
│
├── Repositories/                  <-- Capa de Datos exclusiva para SPs (Dapper)
│   ├── IUserRepository.cs         <-- Contrato para operaciones de usuario
│   ├── UserRepository.cs          <-- Implementación con Dapper (llama a sp_RegisterUser y sp_LoginUser)
│   ├── ITriviaRepository.cs       <-- Contrato para categorías y juego
│   └── TriviaRepository.cs        <-- Implementación con Dapper (llama a sp_GetCategories y sp_GetRandomQuestionsByCategory)
│
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml           <-- Vista de inicio de sesión
│   │   └── Register.cshtml        <-- Vista de registro
│   │
│   ├── Trivia/
│   │   ├── Index.cshtml           <-- Muestra las 2 categorías (Animales y Carros)
│   │   └── Play.cshtml            <-- Interfaz del juego (muestra las 3 preguntas aleatorias)
│   │
│   └── Shared/
│       └── _Layout.cshtml         <-- Layout general con Bootstrap
│
├── wwwroot/                       <-- Archivos estáticos (CSS, JS, imágenes)
├── Database           
│
├── appsettings.json               <-- Cadena de conexión a SQL Server
│
└── Program.cs                     <-- Configuración de servicios, Dapper y Cookies de autenticación
```

# Dependencias 
```
dotnet add package Dapper
dotnet add package Microsoft.Data.SqlClient
dotnet add package BCrypt.Net-Next
```