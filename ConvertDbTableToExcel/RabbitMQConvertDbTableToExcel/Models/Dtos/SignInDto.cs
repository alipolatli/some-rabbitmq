using System.ComponentModel.DataAnnotations;

namespace RabbitMQConvertDbTableToExcel.Models.Dtos
{
    public class SignInDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }

}
