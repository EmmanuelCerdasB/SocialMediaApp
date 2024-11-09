using Microsoft.AspNetCore.Mvc;
using SocialMediaApp.Dominio.Interfaces;
using SocialMediaApp.Dominio.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SocialMediaApp.Presentacion.Controllers
{
    // Define la ruta base para el controlador de eventos como "api/Events"
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        // Define una instancia de la interfaz IEvento para acceder al servicio de eventos
        private readonly IEvento _eventoService;

        // Constructor que inyecta la dependencia de IEvento en el controlador
        public EventsController(IEvento eventoService)
        {
            _eventoService = eventoService;
        }

        // M�todo GET para obtener eventos de un usuario espec�fico
        // Ruta: api/Events/User/{userId}
        [HttpGet("User/{userId}")]
        public async Task<IActionResult> GetEventsForUser(int userId)
        {
            // Llama al m�todo de servicio para obtener los eventos del usuario
            var eventos = await _eventoService.ObtenerEventosParaUsuarioAsync(userId);

            // Verifica si no se encontraron eventos para el usuario
            if (eventos == null)
            {
                // Retorna un mensaje NotFound si no hay eventos
                return NotFound("No se encontraron eventos para el usuario especificado.");
            }

            // Retorna la lista de eventos en un formato JSON con estado 200 OK
            return Ok(eventos);
        }

        // M�todo POST para agregar un nuevo evento
        // Ruta: api/Events
        [HttpPost]
        public async Task<IActionResult> AddEvent([FromBody] Evento evento)
        {
            // Verifica si el objeto evento es nulo
            if (evento == null)
            {
                // Retorna BadRequest si el evento no es v�lido
                return BadRequest("El evento no puede ser nulo.");
            }

            // Llama al m�todo de servicio para agregar el evento a la base de datos
            await _eventoService.AgregarEventoAsync(evento);

            // Retorna CreatedAtAction con la ruta para obtener eventos del usuario, junto con el nuevo evento creado
            return CreatedAtAction(nameof(GetEventsForUser), new { userId = evento.UsuarioId }, evento);
        }

        // M�todo POST para invitar a un usuario a un evento espec�fico
        // Ruta: api/Events/{eventId}/Invite/{userId}
        [HttpPost("{eventId}/Invite/{userId}")]
        public async Task<IActionResult> InviteUser(int eventId, int userId)
        {
            // Llama al m�todo de servicio para invitar al usuario al evento
            await _eventoService.InvitarUsuarioAsync(eventId, userId);

            // Retorna un mensaje de �xito si el usuario ha sido invitado correctamente
            return Ok("Usuario invitado correctamente al evento.");
        }

        // M�todo PUT para confirmar la asistencia de un usuario a un evento
        // Ruta: api/Events/{eventId}/ConfirmAttendance/{userId}
        [HttpPut("{eventId}/ConfirmAttendance/{userId}")]
        public async Task<IActionResult> ConfirmAttendance(int eventId, int userId, [FromBody] string confirmacion)
        {
            // Verifica que la confirmaci�n sea v�lida
            if (string.IsNullOrEmpty(confirmacion) ||
                !(confirmacion == "Asistir�" || confirmacion == "No asistir�" || confirmacion == "Pendiente"))
            {
                // Retorna BadRequest si la confirmaci�n es inv�lida
                return BadRequest("Confirmaci�n inv�lida.");
            }

            // Llama al m�todo de servicio para actualizar la confirmaci�n de asistencia
            await _eventoService.ConfirmarAsistenciaAsync(eventId, userId, confirmacion);

            // Retorna un mensaje de �xito si la confirmaci�n se ha actualizado correctamente
            return Ok("Asistencia confirmada correctamente.");
        }
    }
}
