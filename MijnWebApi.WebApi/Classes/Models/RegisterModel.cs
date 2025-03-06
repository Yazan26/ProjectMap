using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes.Models
{
    public class RegisterModel
    {

        [Required(ErrorMessage ="Het Email veld is verplicht!")]
        [EmailAddress(ErrorMessage = "Het Email veld moet een geldig email adres bevatten!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Het Wachtwoord veld is verplicht!")]
        [MinLength(8, ErrorMessage = "Het Wachtwoord veld moet minimaal 8 karakters bevatten!")]
        public string Password { get; set; }
    }
}
