
namespace NETCoreSeed.CoreDomain.Commands.Inputs
{
    public class RegisterUserCommand : ICommand
    {
        public string Email { get; set; }
        public string Document { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}