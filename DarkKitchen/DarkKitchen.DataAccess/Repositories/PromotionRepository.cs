using DarkKitchen.Domain.Entities;
using DarkKitchen.Domain.Interfaces.Repository;
using Microsoft.EntityFrameworkCore;

namespace DarkKitchen.DataAccess.Repositories;

public class PromotionRepository(AppDbContext context) : IPromotionRepository
{
    public void Add(Promotion promotion)
    {
        context.Promotion.Add(promotion);
        context.SaveChanges();
    }

    public void Update(Promotion promotion)
    {
        var existingPromotion = context.Promotion.Find(promotion.Id);
        context.Entry(existingPromotion!).CurrentValues.SetValues(promotion);
        context.SaveChanges();
    }

    public void SetProducts(int promotionId, List<int> productIds)
    {
        var promotion = context.Promotion
            .Include(p => p.Products)
            .First(p => p.Id == promotionId);

        promotion.Products = context.Products.Where(p => productIds.Contains(p.Id)).ToList();
        context.SaveChanges();
    }

    public Promotion? GetById(int id)
    {
        return context.Promotion.Find(id);
    }

    public IEnumerable<Promotion> GetPromotions(DateTime? date, string? productLine, string? productName)
    {
        var query = context.Promotion.Include(p => p.Products).AsQueryable();

        if(date.HasValue)
        {
            query = query.Where(p => p.DateFrom <= date && p.DateTo >= date);
        }

        if(!string.IsNullOrEmpty(productLine))
        {
            query = query.Where(p => p.Products.Any(prod => prod.ProductLine == productLine));
        }

        if(!string.IsNullOrEmpty(productName))
        {
            query = query.Where(p => p.Products.Any(prod => prod.Name.Contains(productName)));
        }

        return query.ToList();
    }

    public IEnumerable<Promotion> GetActiveForProducts(IEnumerable<int> productIds, DateTime date)
    {
        return context.Promotion
            .Include(p => p.Products)
            .Where(p => p.DateFrom <= date && p.DateTo >= date && p.Products.Any(prod => productIds.Contains(prod.Id)))
            .ToList();
    }
}
