using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtledDictionary.Resources.Authentication
{
    public class TokenPair
    {
        public string? Token { get; set; }
        public string? RefreshToken { get; set; }
    }
}
