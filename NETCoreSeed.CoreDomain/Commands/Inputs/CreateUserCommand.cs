using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.CoreDomain.Commands.Inputs
{
    public class CreateUserCommand : ICommand
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Birthdate { get; set; }
        public Gender Gender { get; set; }
        public UserProfile Profile { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }
}