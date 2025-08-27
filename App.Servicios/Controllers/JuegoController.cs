using App.Servicios.Hubs;
using App.Servicios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace App.Servicios.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JuegoController : ControllerBase
    {
        private readonly BasketDbContext context;
        private readonly IHubContext<JuegoHub> juegoHub;

        public JuegoController(BasketDbContext db, IHubContext<JuegoHub> hub)
        {
            context = db;
            juegoHub = hub;
        }

        [HttpGet("{uuid}")]
        public async Task<IActionResult> Get(string uuid)
        {
            var juego = await context.Juego.FindAsync(uuid);
            return juego == null ? NotFound() : Ok(juego);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDefault()
        {
            var uuid = Guid.NewGuid().ToString().Replace("-", "").ToLower();
            var juego = new Juego { Uuid = uuid, RemainingSeconds = 600 };

            context.Juego.Add(juego);
            await context.SaveChangesAsync();

            await juegoHub.Clients.All.SendAsync("MatchUpdated", juego); // <-- coincide con Angular

            return CreatedAtAction(nameof(Get), new { uuid = juego.Uuid }, juego);
        }

        [HttpPost("{uuid}/score")]
        public async Task<IActionResult> AddScore(string uuid, [FromQuery] string team, [FromQuery] int points)
        {
            var juego = await context.Juego.FindAsync(uuid);
            if (juego == null) return NotFound();

            if (team == "home") juego.HomeScore += points;
            else juego.AwayScore += points;

            await context.SaveChangesAsync();
            await juegoHub.Clients.All.SendAsync("MatchUpdated", juego);
            return Ok(juego);
        }

        [HttpPost("{uuid}/foul")]
        public async Task<IActionResult> AddFoul(string uuid, [FromQuery] string team)
        {
            var juego = await context.Juego.FindAsync(uuid);
            if (juego == null) return NotFound();

            if (team == "home") juego.HomeFouls++;
            else juego.AwayFouls++;

            await context.SaveChangesAsync();
            await juegoHub.Clients.All.SendAsync("MatchUpdated", juego);
            return Ok(juego);
        }

        [HttpPost("{uuid}/quarter")]
        public async Task<IActionResult> NextQuarter(string uuid)
        {
            var juego = await context.Juego.FindAsync(uuid);
            if (juego == null) return NotFound();

            juego.Quarter = Math.Min(4, juego.Quarter + 1);
            juego.RemainingSeconds = juego.QuarterDurationSeconds;
            juego.IsRunning = false;

            await context.SaveChangesAsync();

            await juegoHub.Clients.All.SendAsync("QuarterChanged", new { uuid = juego.Uuid, cuarto = juego.Quarter });

            return Ok(juego);
        }

        [HttpPost("{uuid}/timer")]
        public async Task<IActionResult> UpdateTimer(string uuid, [FromQuery] int remainingSeconds, [FromQuery] bool isRunning)
        {
            var juego = await context.Juego.FindAsync(uuid);
            if (juego == null) return NotFound();

            juego.RemainingSeconds = remainingSeconds;
            juego.IsRunning = isRunning;

            await context.SaveChangesAsync();
            await juegoHub.Clients.All.SendAsync("MatchUpdated", juego);
            return Ok(juego);
        }
    }
}
