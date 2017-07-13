using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TradingPlatform.Models;

namespace TradingPlatform.Data.DataLayer
{
    public class OffersDatalayer
    {
        private ApplicationDbContext db;
        public OffersDatalayer(ApplicationDbContext ctx)
        {
            db = ctx;
        }

        public void AcceptOffer(Trade offer, Order order)
        {

        }
        
    }
}