using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string PersonLogin { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string PersonPassword { get; set; }
    }
}
