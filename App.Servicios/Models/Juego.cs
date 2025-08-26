using System.ComponentModel.DataAnnotations;

namespace App.Servicios.Models
{
    public class Juego
    {
        [Key]
        public required string Uuid { get; set; }
        public string HomeTeam { get; set; } = "Local";
        public string AwayTeam { get; set; } = "Visitante";
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public int Quarter { get; set; } = 1;
        public int HomeFouls { get; set; }
        public int AwayFouls { get; set; }
        public int QuarterDurationSeconds { get; set; } = 600;
        public int RemainingSeconds { get; set; }
        public bool IsRunning { get; set; } = false;
    }
}
