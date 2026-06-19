using Microsoft.EntityFrameworkCore;
using PruebaPayment.DB.Models;

namespace PruebaPayment.DB;

internal class DBSeeder
{
    internal static void Seed(DbContext context, bool _)
    {
        bool changes = false;

        if(!context.Set<Merchant>().Any())
        {
            context.Set<Merchant>().Add(new Merchant()
            {
                Name = "Merchant 1",
            });

            context.Set<Merchant>().Add(new Merchant()
            {
                Name = "Merchant 2",
                IsActive = false
            });

            changes = true;
        }

        if (changes)
        {
            context.SaveChanges();
        }
    }
}