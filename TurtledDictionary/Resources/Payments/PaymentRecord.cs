using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TurtledDictionary.Resources.Payments
{
    public class PaymentRecord
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public DateTime Created { get; set; }
        public string ReceiverId { get; set; }
        public double Amount { get; set; }
    }
}
