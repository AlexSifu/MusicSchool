# MusicSchool 🎵

Aplicación web para la gestión de escuelas de música, estudiantes, docentes y sus asignaciones.

## 📦 Arquitectura

El sistema está dividido en 4 proyectos:

- **API/** – ASP.NET Core Web API (.NET 8)
- **Core/** – Entidades y lógica de dominio
- **Infrastructure/** – Acceso a datos (EF Core + procedimientos almacenados)
- **UI/** – Interfaz de usuario (Razor Pages)

## ⚙️ Requisitos

- .NET 8 SDK
- SQL Server
- Visual Studio 2022+ o VS Code
- Git

## 🏗️ Configuración e implementación

1. Clonar el repositorio:

```bash
git https://github.com/AlexSifu/MusicSchool.git
cd MusicSchool
```

2. Crear la base de datos en SQL Server, de preferencia llamarla music_school.

3. Configurar las credenciales de la cadena de conexión en MusicSchool.API.appSettings.json.

4. Abrir la consola de administrador de paquetes Nuget, y ejecutar el sigiente comando en la consola en el proyecto de MusicSchool.Infrastructure, para crear la estructura de la BD.
```bash
Update-Database -StartupProject MusicSchool.API
```

5. Los procedimientos almacenados necesarios para el sistema, así como un INSERT para agregar un usuario por defecto, se encuentran en el archivo:
```bash
/Database/stored_procedures/SPs_music_school.sql
```

6. Para correr el backend, tendremos que abrir una terminal en la carpeta de MusicSchool.API, y ejecutar:
```bash
dotnet run --urls "https://localhost:5001"
```

6. Para correr el fronted, tendremos que abrir una terminal en la carpeta de MusicSchool.UI, y ejecutar:
```bash
dotnet run --urls "https://localhost:7028"
```

## 📦 Importante
En caso de que los puertos estén ocupados, considerar estos puntos para cambiarlos:
- Para el backend: En MusicSchool.API, abrir el archivo programs.cs y cambiar la sección de política CORS, y colocar el puerto del fronted.
- Para el fronted: En MusicSchool.API, abrir el archivo en wwwroot/js/config.js, y cambiar el puerto del backend
## Autor

- [@Alex Sifuentes](https://github.com/AlexSifu/MusicSchool/)

