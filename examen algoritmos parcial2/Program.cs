using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Program
{
    const string Archivo = "mascotas.csv";

    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Requisito 9: cargar al iniciar (revisando File.Exists)
        List<Mascota> mascotas = CargarMascotas();

        // Requisito 9: recalcular el contador de Id después de cargar
        int contadorId = CalcularSiguienteId(mascotas);

        string opcion;
        do
        {
            Console.WriteLine();
            Console.WriteLine("===== VETERINARIA - REGISTRO DE MASCOTAS =====");
            Console.WriteLine("1. Agregar mascota");
            Console.WriteLine("2. Listar mascotas");
            Console.WriteLine("3. Buscar por especie");
            Console.WriteLine("4. Salir");
            Console.Write("Seleccione una opción: ");
            opcion = Console.ReadLine() ?? "";

            switch (opcion)
            {
                case "1":
                    AgregarMascota(mascotas, ref contadorId); // Requisito 5: contador por ref
                    break;
                case "2":
                    ListarMascotas(mascotas);
                    break;
                case "3":
                    BuscarPorEspecie(mascotas);
                    break;
                case "4":
                    GuardarMascotas(mascotas); // Requisito 9: guardar al salir
                    Console.WriteLine("Datos guardados en mascotas.csv. ¡Hasta luego!");
                    break;
                default:
                    Console.WriteLine("Error: opción no válida.");
                    break;
            }
        } while (opcion != "4");
    }

    // Requisitos 2, 3, 4 y 5
    static void AgregarMascota(List<Mascota> mascotas, ref int contadorId)
    {
        Console.Write("Nombre: ");
        string nombre = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(nombre))
        {
            Console.WriteLine("Error: el nombre no puede estar vacío ni tener solo espacios.");
            return; // vuelve al menú sin agregar
        }

        Console.Write("Especie: ");
        string especie = Console.ReadLine() ?? "";
        if (string.IsNullOrWhiteSpace(especie))
        {
            Console.WriteLine("Error: la especie no puede estar vacía.");
            return;
        }

        Console.Write("Peso (kg): ");
        string textoPeso = (Console.ReadLine() ?? "").Trim().Replace(',', '.');
        if (!decimal.TryParse(textoPeso, NumberStyles.AllowDecimalPoint,
                              CultureInfo.InvariantCulture, out decimal peso) || peso <= 0)
        {
            Console.WriteLine("Error: el peso debe ser un número mayor que 0.");
            return;
        }

        Mascota nueva = new Mascota(contadorId, nombre.Trim(), especie.Trim(), peso);
        mascotas.Add(nueva);
        contadorId++; // al ser ref, el cambio se refleja en Main

        Console.WriteLine("Mascota agregada: " + nueva);
    }

    // Requisitos 6 y 7
    static void ListarMascotas(List<Mascota> mascotas)
    {
        if (mascotas.Count == 0)
        {
            Console.WriteLine("No hay mascotas registradas.");
            return;
        }

        for (int i = 0; i < mascotas.Count; i++)
        {
            Mascota m = mascotas[i];
            string clasificacion = m.Peso > 25 ? "Raza grande" : "Raza pequeña/mediana";
            Console.WriteLine($"Mascota {i + 1}: {m.Nombre} ({m.Especie}) - " +
                              $"{m.Peso.ToString(CultureInfo.InvariantCulture)} kg - {clasificacion}");
        }
    }

    // Requisito 8
    static void BuscarPorEspecie(List<Mascota> mascotas)
    {
        Console.Write("Texto a buscar en la especie: ");
        string texto = (Console.ReadLine() ?? "").Trim();
        if (string.IsNullOrWhiteSpace(texto))
        {
            Console.WriteLine("Error: debe ingresar un texto para buscar.");
            return;
        }

        bool encontrada = false;
        foreach (Mascota m in mascotas)
        {
            if (m.Especie.Contains(texto, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine(m);
                encontrada = true;
            }
        }

        if (!encontrada)
            Console.WriteLine("No se encontraron mascotas con esa especie.");
    }

    // Requisito 9: guardar
    static void GuardarMascotas(List<Mascota> mascotas)
    {
        List<string> lineas = new List<string>();
        lineas.Add("Id,Nombre,Especie,Peso");

        foreach (Mascota m in mascotas)
        {
            lineas.Add($"{m.Id},{QuitarComas(m.Nombre)},{QuitarComas(m.Especie)}," +
                       $"{m.Peso.ToString(CultureInfo.InvariantCulture)}");
        }

        File.WriteAllLines(Archivo, lineas);
    }

    // Requisito 9: cargar
    static List<Mascota> CargarMascotas()
    {
        List<Mascota> lista = new List<Mascota>();

        if (!File.Exists(Archivo))
        {
            Console.WriteLine("No se encontró mascotas.csv. Se inicia con la lista vacía.");
            return lista;
        }

        string[] lineas = File.ReadAllLines(Archivo);
        for (int i = 1; i < lineas.Length; i++) // i = 1 para saltar el encabezado
        {
            string[] partes = lineas[i].Split(',');
            if (partes.Length != 4) continue;

            if (int.TryParse(partes[0], out int id) &&
                decimal.TryParse(partes[3], NumberStyles.AllowDecimalPoint,
                                 CultureInfo.InvariantCulture, out decimal peso))
            {
                lista.Add(new Mascota(id, partes[1], partes[2], peso));
            }
        }

        Console.WriteLine($"Se cargaron {lista.Count} mascota(s) desde mascotas.csv.");
        return lista;
    }

    // El siguiente Id es el mayor Id cargado + 1, para no repetir
    static int CalcularSiguienteId(List<Mascota> mascotas)
    {
        int maximo = 0;
        foreach (Mascota m in mascotas)
        {
            if (m.Id > maximo) maximo = m.Id;
        }
        return maximo + 1;
    }

    // Evita romper el CSV si alguien escribe una coma en el nombre o especie
    static string QuitarComas(string texto)
    {
        return texto.Replace(",", " ");
    }
}