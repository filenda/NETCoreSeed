using System;
using System.Collections.Generic;
using System.Text;

namespace NETCoreSeed.CoreDomain.Commands.Inputs
{
    public class FacebookAuthCommand : ICommand
    {
        public string AccessToken { get; set; }
    }
}