using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TurtledDictionary.Resources.Payments;

namespace TurtledPayments.Contexts
{
    public class PaymentInformationDbContext : DbContext
    {
        public PaymentInformationDbContext(DbContextOptions<PaymentInformationDbContext> options)
            : base(options) { }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<PaymentRecord>(entity =>
            {
                entity.HasKey(x => x.Id);
            });

            
        }

    }
}
