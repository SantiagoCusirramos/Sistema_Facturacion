Este sera un sistema de facturacion realizado en C#

Para iniciar este proyecto se estan usandon las siguientes herramientas

Creamos el proyecto con Avalonia con el objetivo que nos ayude con la interfaz grafica para la aplicacion

``` bash
dotnet new avalonia.app -n Sistema_Facturacion
```

Posteriormente para la conexcion con la DB agregamos el siguiente paquete

``` bash
dotnet add package Microsoft.Data.SqlClient
```

Asi mismo esto lo complementamos con los controles de Avalonia

```bash
dotnet add package Avalonia.Controls.DataGrid
```

En relacion al .NET que se esta usando, se ha optado por la version 8.0 debido a la compatibilidad que tiene con sistema operativos como linux
