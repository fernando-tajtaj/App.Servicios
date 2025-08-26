using App.Servicios.Models;
using Microsoft.AspNetCore.SignalR;

namespace App.Servicios.Hubs
{
    public class JuegoHub : Hub
    {
        // Enviar actualización completa del juego
        public async Task ActualizarJuego(Juego juego)
        {
            await Clients.All.SendAsync("MatchUpdated", new
            {
                uuid = juego.Uuid,
                juego
            });
        }

        // Métodos opcionales para eventos específicos
        public async Task ActualizarPuntos(string uuid, string team, int puntos)
        {
            await Clients.All.SendAsync("ScoreUpdated", new
            {
                uuid,
                team,
                puntos
            });
        }

        public async Task ActualizarFaltas(string uuid, string team, int faltas)
        {
            await Clients.All.SendAsync("FoulUpdated", new
            {
                uuid,
                team,
                faltas
            });
        }

        public async Task CambiarCuarto(string uuid, int cuarto)
        {
            await Clients.All.SendAsync("QuarterChanged", new
            {
                uuid,
                cuarto
            });
        }

        public async Task ActualizarTimer(string uuid, int remainingSeconds, bool isRunning)
        {
            await Clients.All.SendAsync("TimerUpdated", new
            {
                uuid,
                remainingSeconds,
                isRunning
            });
        }
    }
}
