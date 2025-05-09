﻿using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using NeoShopping.Data;
using NeoShopping.Entities;
using NeoShopping.Helpers;
using NeoShopping.Presentation;

namespace NeoShopping.Logic
{
    public class ProductoLogic
    {
        public static void AgregarProducto()
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╚═════════════════════ Agregar Producto ═════════════════════╝\n");
                Console.ResetColor();

                Producto nuevoProducto = InfoHelpers.ObtenerDatosProducto();

                GuardarProductoEnBaseDeDatos(nuevoProducto);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nProducto agregado correctamente.");
                Console.ResetColor();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al guardar en la base de datos: {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex}");
            }

            FrmProductos.MenuDeSalida();
        }

        private static void GuardarProductoEnBaseDeDatos(Producto nuevoProducto)
        {
            using (var context = new NeoShoppingDataContext())
            {
                context.Productos.Add(nuevoProducto);
                context.SaveChanges();
            }
        }

        public static void VerOBuscarProductos()
        {
            bool back = false;

            while (!back)
            {
                try
                {
                    using (var context = new NeoShoppingDataContext())
                    {
                        var productos = context.Productos.ToList();

                        Console.Clear();
                        FrmProductos.MenuVerOBuscarProductos();
                        Console.Write("Seleccione una opción: ");

                        int option;
                        while (!int.TryParse(Console.ReadLine(), out option))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Entrada inválida. Debes ingresar un número.\n");
                            Console.ResetColor();
                            
                            Console.Write("Seleccione una opción: ");
                        }

                        switch (option)
                        {
                            case 1:
                                MostrarListaProductos(context);
                                FrmProductos.MenuDeSalida();
                                break;

                            case 2:
                                BuscarProductoPorId(context);
                                FrmProductos.MenuDeSalida();
                                break;

                            case 3:
                                back = true;
                                Console.Clear();
                                FrmProductos.MenuGestionarProductos();
                                break;

                            default:
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Opción no válida. Intente nuevamente.\n");
                                Console.ResetColor();
                                break;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error inesperado al obtener productos: {ex.Message}");
                    InicioUI.Pausa();
                }
            }
        }

        public static void EditarProducto()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╚═════════════════════ Editar Producto ═════════════════════╝\n");
                Console.ResetColor();

                int id = InputHelper.LeerEntero("Ingrese el ID del producto que desea editar: ");

                using (var context = new NeoShoppingDataContext())
                {
                    var producto = ObtenerProductoPorId(context, id);
                    if (producto == null) return;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("\nProducto encontrado:\n");
                    Console.WriteLine(producto.MostrarInformacion());
                    Console.ResetColor();

                    InputHelper.LeerYActualizarDatosProducto(producto);
                    GuardarCambios(context);

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("Datos actuales del producto:\n");
                    Console.WriteLine(producto.MostrarInformacion());
                    Console.ResetColor();

                    FrmProductos.MenuDeSalida();
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nError al actualizar el producto: {ex.Message}");
                Console.ResetColor();

                InicioUI.Pausa();
            }
        }

        private static Producto ObtenerProductoPorId(NeoShoppingDataContext context, int id)
        {
            var producto = context.Productos.FirstOrDefault(p => p.IdProducto == id);

            if (producto == null)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nProducto no encontrado. Verifique que el ID sea correcto.");
                Console.ResetColor();
                FrmProductos.MenuDeSalida();
            }

            return producto;
        }

        private static void GuardarCambios(NeoShoppingDataContext context)
        {
            context.SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nProducto actualizado correctamente.\n");
            Console.ResetColor();
        }

        public static void EliminarProducto()
        {
            try
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("╚═════════════════════ Eliminar Producto ═════════════════════╝\n");
                Console.ResetColor();

                int idProducto = InputHelper.LeerEntero("Ingrese el ID del producto a eliminar: ");

                using (var context = new NeoShoppingDataContext())
                {
                    var producto = ObtenerProductoPorId(context, idProducto);
                    if (producto == null) return;

                    MostrarDatosProductoAEliminar(producto);
                    if (ConfirmarEliminacion())
                    {
                        EliminarProductoDeBaseDeDatos(context, producto);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nOperación cancelada por el usuario.\n");
                        Console.ResetColor();
                    }
                }

                FrmProductos.MenuDeSalida();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error al eliminar el producto: {ex.InnerException?.Message ?? ex.Message}");
                InicioUI.Pausa();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado: {ex.Message}");
                InicioUI.Pausa();
            }
        }

        private static void MostrarDatosProductoAEliminar(Producto producto)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nDatos del producto a eliminar:\n");
            Console.WriteLine(producto.MostrarInformacion());
            Console.ResetColor();
        }

        private static bool ConfirmarEliminacion()
        {
            while (true)
            {
                Console.Write("\n¿Está seguro que desea eliminar este producto? (s/n): ");
                string confirmacion = Console.ReadLine()?.Trim().ToLower();

                if (confirmacion == "s")
                    return true;
                else if (confirmacion == "n")
                    return false;
                else
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Por favor, ingrese una opcion valida: 's' para continuar o 'n' para cancelar.");
                    Console.ResetColor();
            }
        }


        private static void EliminarProductoDeBaseDeDatos(NeoShoppingDataContext context, Producto producto)
        {
            context.Productos.Remove(producto);
            context.SaveChanges();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\nProducto eliminado correctamente.\n");
            Console.ResetColor();
        }

        private static void MostrarListaProductos(NeoShoppingDataContext context)
        {
            Console.Clear();
            var productos = context.Productos.ToList();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚═════════════════════ Lista de Productos ═════════════════════╝\n");
            Console.ResetColor();

            if (productos.Any())
            {
                Console.WriteLine($"{"ID",-5}  {"Nombre",-25} {"Descripción",-30}  {"Precio",-10}  {"Stock",-10}  {"ID Proveedor",-18}");
                Console.WriteLine(new string('─', 100));

                foreach (var p in productos)
                {
                    Console.WriteLine($"{p.IdProducto,-5}  {p.Nombre,-25} {p.Descripcion,-30}  {p.Precio,-10}  {p.Stock,-10}  {p.IdProveedor,-18}");
                }

                Console.WriteLine("");
            }
            else
            {
                Console.WriteLine("No hay productos registrados.\n");
            }
        }

        private static void BuscarProductoPorId(NeoShoppingDataContext context)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╚═════════════════════ Buscar Producto ═════════════════════╝\n");
            Console.ResetColor();

            int id = InputHelper.LeerEntero("Ingrese el ID del producto: ");
            var producto = context.Productos.FirstOrDefault(p => p.IdProducto == id);

            if (producto != null)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nProducto encontrado:\n");
                Console.ResetColor();
                Console.WriteLine(producto.MostrarInformacion());
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\nProducto no encontrado. Verifique que el ID sea correcto.\n");
                Console.ResetColor();
            }
        }
    }
}