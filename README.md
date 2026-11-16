# TiendaMVVM

Aplicación de escritorio **WPF (.NET 8)** para gestionar productos y categorías de una tienda, construida con el patrón **MVVM**, **EF Core + SQLite**, el patrón **Repository** y el principio de **inversión de dependencias (DIP)**. Proyecto de la clase INF033, desarrollado en pareja.

## Funcionalidades

**Pestaña Productos**
- Listado de productos con su categoría.
- Agregar, editar y eliminar productos (CRUD).
- Validación de datos antes de guardar (nombre, precio y categoría).
- Mensajes de estado y de error en pantalla.

**Pestaña Reportes**
- Resumen general: total de productos, valor total, precio promedio y producto más caro.
- Resumen por categoría: cantidad, total y promedio.
- Filtro por precio mínimo, ordenado de menor a mayor precio.
- Se actualiza automáticamente cuando se agrega, edita o elimina un producto.

## Tecnologías

| Componente | Tecnología |
|---|---|
| Interfaz | WPF (XAML) |
| Plataforma | .NET 8 |
| Patrón de interfaz | MVVM |
| Acceso a datos (opción 1) | Entity Framework Core 8 |
| Acceso a datos (opción 2) | ADO.NET con `Microsoft.Data.Sqlite` |
| Base de datos | SQLite (`tienda.db`) |
| Consultas de reportes | LINQ |

## Arquitectura

```
Vista (XAML)  ->  ViewModel  ->  IProductoRepositorio  ->  Base de datos
                                    ^            ^
                        ProductoRepositorio   ProductoAdoRepositorio
                           (EF Core)             (ADO.NET)
```

- **MVVM:** las vistas solo hacen binding; la lógica vive en los ViewModels (`ObservableObject`, `RelayCommand`, `AsyncRelayCommand`).
- **Repository + DIP:** los ViewModels dependen de la interfaz `IProductoRepositorio`, nunca de una clase concreta. La clase `App` es la raíz de composición y el único lugar que decide qué implementación usar.
- **Validación separada:** `ValidadorProducto` contiene las reglas de validación fuera del ViewModel.

## Estructura del proyecto

```
TiendaMVVM/
├── Models/            Producto, Categoria
├── Data/              AppDbContext, DbInicializador, Conexion
├── Infrastructure/    ObservableObject, RelayCommand, AsyncRelayCommand
├── Repositories/      IProductoRepositorio, ProductoRepositorio (EF Core),
│                      ProductoAdoRepositorio (ADO.NET)
├── Services/          ValidadorProducto
├── ViewModels/        MainViewModel, ProductosViewModel, ReportesViewModel
├── Views/             ProductosView, ReportesView
├── MainWindow.xaml    Ventana principal con las dos pestañas
└── App.xaml.cs        Raíz de composición
```

## Requisitos

- Windows 10 u 11
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 o superior con la carga de trabajo **Desarrollo de escritorio con .NET** (opcional)

## Cómo ejecutarlo

```bash
git clone <https://github.com/jordanelboss923-png/TiendaMVVM>
cd TiendaMVVM
dotnet run
```

Con Visual Studio: abre la solución y pulsa `F5`.

Al iniciar, la base de datos `tienda.db` se crea sola con 3 categorías (Bebidas, Tecnología, Hogar) y 5 productos de ejemplo.

## Cambiar entre EF Core y ADO.NET

En `App.xaml.cs` se encuentra la constante:

```csharp
private static readonly bool UsarAdoNet = false; // false = EF Core | true = ADO.NET
```

Cambia el valor a `true` y ejecuta de nuevo. La aplicación se comporta igual, sin modificar ningún ViewModel ni vista, lo que demuestra el principio DIP.

## Reglas de validación

- El nombre es obligatorio y tiene un máximo de 100 caracteres.
- El precio debe ser un número entre 0 y 999999.
- Se debe seleccionar una categoría.

## División del trabajo

| Parte | Contenido |
|---|---|
| **Parte 1** (Fase 2: DIP, EF Core y Repository) | Modelos, `AppDbContext`, base MVVM, `IProductoRepositorio`, `ProductoRepositorio`, `ProductosViewModel` y su vista |
| **Parte 2** (Fase 3: LINQ, ADO.NET y refactorización) | `ProductoAdoRepositorio`, `ReportesViewModel` y su vista, `ValidadorProducto` |

## Autores

- _Jordan Alexander Guzman_
- _Javier Bisono_
