using DSW_3P_PR03.Models;
using DSW_3P_PR03.Repositories;

namespace DSW_3P_PR03.Services;

public class PersonajeService(IPersonajeRepository repository) : IPersonajeService
{
    public List<Personaje> ObtenerTodos() => repository.ObtenerTodos();
    public Personaje? ObtenerPorId(int id) => repository.ObtenerPorId(id);
    public List<Personaje> ObtenerPorVideojuego(string nombre) => repository.ObtenerPorVideojuego(nombre);
    public List<Personaje> ObtenerPorNivelMinimo(int nivel) => repository.ObtenerPorNivelMinimo(nivel);

    private static void Validar(Personaje personaje)
    {
        if (string.IsNullOrWhiteSpace(personaje.Nombre))
            throw new ArgumentException("El nombre del personaje es obligatorio.");
        if (string.IsNullOrWhiteSpace(personaje.Videojuego))
            throw new ArgumentException("El videojuego es obligatorio.");
        if (string.IsNullOrWhiteSpace(personaje.Clase))
            throw new ArgumentException("La clase es obligatoria.");
        if (personaje.Nivel is < 1 or > 100)
            throw new ArgumentException("El nivel debe estar entre 1 y 100.");
        if (personaje.Poder <= 0)
            throw new ArgumentException("El poder debe ser mayor que cero.");
        if (!Uri.TryCreate(personaje.ImagenUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("ImagenUrl debe ser una URL absoluta válida con HTTP o HTTPS.");

        personaje.Nombre = personaje.Nombre.Trim();
        personaje.Videojuego = personaje.Videojuego.Trim();
        personaje.Clase = personaje.Clase.Trim();
        personaje.ImagenUrl = personaje.ImagenUrl.Trim();
    }

    public Personaje Agregar(Personaje nuevo)
    {
        Validar(nuevo);
        nuevo.Disponible = true;
        return repository.Agregar(nuevo);
    }

    public bool Actualizar(int id, Personaje actualizado)
    {
        if (repository.ObtenerPorId(id) is null) return false;
        Validar(actualizado);
        return repository.Actualizar(id, actualizado);
    }

    public bool Eliminar(int id) => repository.Eliminar(id);
}