using System.Globalization;

public class Mascota
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Especie { get; set; }
    public decimal Peso { get; set; }

    public Mascota(int id, string nombre, string especie, decimal peso)
    {
        Id = id;
        Nombre = nombre;
        Especie = especie;
        Peso = peso;
    }

    // Requisito 1: override de ToString() con los datos formateados
    public override string ToString()
    {
        return $"[Id {Id}] {Nombre} ({Especie}) - {Peso.ToString(CultureInfo.InvariantCulture)} kg";
    }
}