using System.Text.Json;
using DSW_3P_PR03.Models;

namespace DSW_3P_PR03.Repositories;

public class PersonajeJsonRepository : IPersonajeRepository
{
    private readonly string _ruta;
    private readonly object _candado = new();
    private readonly JsonSerializerOptions _opciones = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public PersonajeJsonRepository(IWebHostEnvironment environment)
    {
        _ruta = Path.Combine(environment.ContentRootPath, "Data", "personajes.json");
        Directory.CreateDirectory(Path.GetDirectoryName(_ruta)!);
        if (!File.Exists(_ruta)) File.WriteAllText(_ruta, "[]");
    }

    private List<Personaje> Leer() =>
        JsonSerializer.Deserialize<List<Personaje>>(File.ReadAllText(_ruta), _opciones)
        ?? throw new JsonException("personajes.json debe contener un arreglo JSON.");

    private void Guardar(List<Personaje> datos)
    {
        // Reemplazar el archivo solo después de escribir el contenido completo.
        var temporal = _ruta + ".tmp";
        File.WriteAllText(temporal, JsonSerializer.Serialize(datos, _opciones));
        File.Move(temporal, _ruta, overwrite: true);
    }

    public List<Personaje> ObtenerTodos()
    {
        lock (_candado) return Leer();
    }

    public Personaje? ObtenerPorId(int id)
    {
        lock (_candado) return Leer().FirstOrDefault(p => p.Id == id);
    }

    public List<Personaje> ObtenerPorVideojuego(string nombre)
    {
        lock (_candado) return Leer()
            .Where(p => string.Equals(p.Videojuego, nombre, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }

    public List<Personaje> ObtenerPorNivelMinimo(int nivel)
    {
        lock (_candado) return Leer()
            .Where(p => p.Nivel >= nivel)
            .ToList();
    }

    public Personaje Agregar(Personaje nuevo)
    {
        lock (_candado)
        {
            var datos = Leer();
            nuevo.Id = datos.Count == 0 ? 1 : checked(datos.Max(p => p.Id) + 1);
            datos.Add(nuevo);
            Guardar(datos);
            return nuevo;
        }
    }

    public bool Actualizar(int id, Personaje actualizado)
    {
        lock (_candado)
        {
            var datos = Leer();
            var indice = datos.FindIndex(p => p.Id == id);
            if (indice < 0) return false;
            actualizado.Id = id;
            datos[indice] = actualizado;
            Guardar(datos);
            return true;
        }
    }

    public bool Eliminar(int id)
    {
        lock (_candado)
        {
            var datos = Leer();
            if (datos.RemoveAll(p => p.Id == id) == 0) return false;
            Guardar(datos);
            return true;
        }
    }
}