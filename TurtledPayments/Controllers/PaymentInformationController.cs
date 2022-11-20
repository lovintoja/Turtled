using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TurtledDictionary.Resources.Payments;
using TurtledPayments.Contexts;

namespace TurtledPayments.Controllers
{
    [Route("payments")]
    [ApiController]
    public class PaymentInformationController : ControllerBase
    {
        private readonly PaymentInformationDbContext _context;

        public PaymentInformationController(PaymentInformationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public void AddRecords()
        {
            Random random = new Random();
            for (int i = 1; i < 51; i++)
            {
                _context.Add(new PaymentRecord { Id = Guid.NewGuid().ToString(), Amount = random.Next(1000000), ReceiverId = (i % 2).ToString(), UserId = ((i - 1) % 2).ToString() });
            }
            _context.SaveChanges();
        }
    }
}
