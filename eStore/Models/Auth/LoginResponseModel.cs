using eStore.Models.User;

namespace eStore.Models.Auth
{
    public class LoginResponseModel
    {
        public string Message { get; set; }
        public UserViewModel User { get; set; }
    }
}
