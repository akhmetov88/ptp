using System;
using System.Collections.Generic;
using System.Linq;
using TradingPlatform.Data;

namespace TradingPlatform.Extentions
{
    public static class LinqExtentions
    {
        public static IEnumerable<Trade> ActualOffers(this IQueryable<Trade> trades)
        {

            return trades.Where(c => c.IsOffer).ToList().Where(c=>c.DateEnd>DateTime.UtcNow&& (c.TotalVolume.Value - c.Orders.Where(o => o.IsAcceptedBySeller).Sum(v => v.Volume) > 0));
        }
      
        public static IQueryable<Trade> NotActualOffers(this IQueryable<Trade> offers)
        {
            return offers.Where(c => (c.IsOffer && c.DateEnd > DateTime.UtcNow) || (c.TotalVolume - c.Orders.Where(o => o.IsAcceptedBySeller).Sum(v => v.Volume) == 0));
        }


        public static int GetFreeOfferVolume(this Trade trade)
        {
            if (trade.Orders != null && trade.Orders.Any())
                return (int)trade.TotalVolume.Value - trade.Orders.Where(o => o.IsAcceptedBySeller).Sum(v => v.Volume);
            else
                return (int)trade.TotalVolume.Value;
        }
        public static IEnumerable<Trade> ActualOrders(this IQueryable<Trade> trades)
        {

            return trades.Where(c => c.IsOrder).ToList().Where(c => c.DateEnd > DateTime.UtcNow && (c.TotalVolume.Value - c.Orders.Where(o => o.IsAcceptedBySeller).Sum(v => v.Volume) > 0));
        }
        public static IQueryable<Trade> NotActualOrders(this IQueryable<Trade> offers)
        {
            return offers.Where(c => (c.IsOrder && c.DateEnd > DateTime.UtcNow) || (c.TotalVolume - c.Orders.Where(o => o.IsAcceptedBySeller).Sum(v => v.Volume) == 0));
        }

    }

}