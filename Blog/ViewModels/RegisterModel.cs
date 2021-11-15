using System.ComponentModel.DataAnnotations;

namespace Blog.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Не указан логин")]
        public string PersonLogin { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string PersonPassword { get; set; }

        [Required(ErrorMessage = "Не указано имя")]
        public string PersonFirstName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        public string PersonLastName { get; set; }

        [Required(ErrorMessage = "Не указан Email")]
        public string PersonEmail { get; set; }
    }
}
