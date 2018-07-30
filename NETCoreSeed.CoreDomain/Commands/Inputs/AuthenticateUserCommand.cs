namespace NETCoreSeed.CoreDomain.Commands.Inputs
{
    public class AuthenticateUserCommand : ICommand
    {
        public string Email { get; set; }
        public string Password { get; set; }
        //public LoginProvider LoginProvider { get; set; }
    }
}