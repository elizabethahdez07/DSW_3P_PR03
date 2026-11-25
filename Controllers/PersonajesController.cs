using DSW_3P_PR03.Models;
using DSW_3P_PR03.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace DSW_3P_PR03.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonajesController(IPersonajeService service) : ControllerBase
{
    [HttpGet]
    public ActionResult<List<Personaje>> ObtenerTodos() => Ok(service.ObtenerTodos());

    [HttpGet("{id:int}")]
    public ActionResult<Personaje> ObtenerPorId(int id)
    {
        var personaje = service.ObtenerPorId(id);
        return personaje is null ? NoEncontrado(id) : Ok(personaje);
    }

    [HttpGet("videojuego/{nombre}")]
    public ActionResult<List<Personaje>> ObtenerPorVideojuego(string nombre) =>
        Ok(service.ObtenerPorVideojuego(nombre));

    [HttpGet("nivel/{nivel:int}")]
    public ActionResult<List<Personaje>> ObtenerPorNivelMinimo(int nivel) =>
        Ok(service.ObtenerPorNivelMinimo(nivel));

    [HttpPost]
    public ActionResult<Personaje> Agregar(Personaje nuevo)
    {
        try
        {
            var creado = service.Agregar(nuevo);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }
        catch (ArgumentException ex) { return DatosInvalidos(ex.Message); }
    }

    [HttpPut("{id:int}")]
    public IActionResult Actualizar(int id, Personaje actualizado)
    {
        try { return service.Actualizar(id, actualizado) ? NoContent() : NoEncontrado(id); }
        catch (ArgumentException ex) { return DatosInvalidos(ex.Message); }
    }

    [HttpDelete("{id:int}")]
    public IActionResult Eliminar(int id) =>
        service.Eliminar(id) ? NoContent() : NoEncontrado(id);

    private ObjectResult NoEncontrado(int id) =>
        Problem(statusCode: 404, title: "Personaje no encontrado", detail: $"No existe el personaje con Id {id}.");

    private ObjectResult DatosInvalidos(string mensaje) =>
        Problem(statusCode: 400, title: "Datos inválidos", detail: mensaje);
}