using System.ComponentModel.DataAnnotations;

namespace Tickets.Shared.Entites
{
    public class Ticket
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Fecha")]
        public DateTime? Date { get; set; }

        [Display(Name = "Usada")]
        public bool Used { get; set; } = false;

        [Display(Name = "Porteria")]
        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Entrance { get; set; }

    }
}
