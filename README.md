# Sistema de Facturación en C#

## Tecnologías y Herramientas Utilizadas

Este proyecto consiste en el desarrollo de un **sistema de facturación en C#** utilizando tecnologías modernas compatibles con múltiples sistemas operativos.

### Creación del Proyecto con Avalonia

Para la interfaz gráfica se ha seleccionado **Avalonia UI**, debido a su enfoque multiplataforma y su compatibilidad con Linux, Windows y macOS.

El proyecto fue creado mediante el siguiente comando:

```bash
dotnet new avalonia.app -n Sistema_Facturacion
```

---

## Configuración de Dependencias

### Conexión con la Base de Datos

Para permitir la conexión con SQL Server, se agregó el paquete `Microsoft.Data.SqlClient`:

```bash
dotnet add package Microsoft.Data.SqlClient
```

### Controles Adicionales para la Interfaz

Asimismo, se incorporó el paquete `Avalonia.Controls.DataGrid`, el cual proporciona componentes avanzados para la manipulación y visualización de datos:

```bash
dotnet add package Avalonia.Controls.DataGrid
```

---

## Versión de .NET Utilizada

El proyecto utiliza **.NET 8.0**, principalmente por su estabilidad, rendimiento y excelente compatibilidad con sistemas operativos Linux.

---

# Configuración de la Base de Datos

Una vez creado el archivo `.sql` correspondiente a la estructura de la base de datos, se procede con su ejecución mediante `sqlcmd`.

> En este caso, se ha configurado una contraseña personalizada para el usuario `sa`.
> La contraseña personalizada es `fdx_santi@#282805`

---

## Creación de la Base de Datos

El siguiente comando crea la base de datos `InvoiceSystem`:

```bash
sqlcmd -S localhost -U sa -P "PASSWORDS" -C -Q "CREATE DATABASE InvoiceSystem"
```

---

## Ejecución del Script SQL

Posteriormente, se ejecuta el script que contiene las tablas, relaciones y demás configuraciones necesarias:

```bash
sqlcmd -S localhost -U sa -P "PASSWORDS" -C -d InvoiceSystem -i ~/Projects/Sistema_Facturacion/Facturacion_DB/facturacion_db.sql
```

---

## Verificacion del proyecto

Una forma curiosa de probar que el proyecto este bien construido es con:

```bash
rm -rf bin obj

/usr/share/dotnet/dotnet restore
/usr/share/dotnet/dotnet build
/usr/share/dotnet/dotnet run
```

Con esto lo que hacemos es limpiar y compilar el proyecto, ademas de que se probara si es que este mismo funciona con el RUN.
