using System.ComponentModel.DataAnnotations;

namespace MijnWebApi.WebApi.Classes.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Het Email veld is verplicht!")]
        [EmailAddress(ErrorMessage = "Het Email veld moet een geldig email adres bevatten!")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Het Wachtwoord veld is verplicht!")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
