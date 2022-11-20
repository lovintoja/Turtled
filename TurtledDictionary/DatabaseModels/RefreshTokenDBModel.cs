using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtledDictionary.DatabaseModels
{
    public class RefreshTokenDBModel
    {
        public string UserId { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ValidUntil { get; set; }
    }
}
