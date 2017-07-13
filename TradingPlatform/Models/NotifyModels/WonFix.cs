using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using TradingPlatform.Data;

namespace TradingPlatform.Models.NotifyModels
{
    public class WonFix : BaseNotify
    {
       // public WonFix() { }
        public  WonFix(Trade trade, Order order, ApplicationUser user) : base(user)
        {
            TradeNumber = trade.Id;
            Volume = order.Volume
            .ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ОБЪЕМ"
            Price = (order.Price).ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ЦЕНА"
            Sum = (order.Volume * order.Price)
            .ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"СУММА"
            Seller = trade?.Seller?.LongName??"";
            Buyer = order?.Buyer?.LongName??"";
            Link = $"https://ptp.ua/trade/bill?t={trade.Id}&b={order.BuyerId}";
            ProductName = StaticData.Inst.GetCatalogValue(trade.ProductName);
            Subject = (trade.IsOffer ? "Пропозиція " : "Заявка ") + "№ " + TradeNumber;
            Trade = true;
            Send = IsSend(user);
          //  SendMessage();
        }

        public WonFix(Trade trade, List<Bet> bets, Contragent buyer, ApplicationUser user) : base(user)
        {
            TradeNumber = trade.Id;
            Volume = bets.Sum(x => x.Volume ?? 0)
                .ToString("C2", new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ОБЪЕМ"
            Price = (bets.Sum(x => x.Price ?? 0) / bets.Sum(x => x.LotsCount)).ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ЦЕНА"
            Sum = bets.Sum(x => x.Volume * x.Price ?? 0)
                .ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"СУММА"
            Seller = trade.Seller.LongName;
            Buyer = buyer.LongName;
            Link = $"http://ptp.ua/trade/bill?t={trade.Id}&b={buyer.Id}";
            ProductName = StaticData.Inst.GetCatalogValue(trade.ProductName);
            Trade = true;
            Send = IsSend(user);
            //SendMessage();
        }
        public WonFix(Trade trade, Bet bet, Contragent buyer, ApplicationUser user) : base(user)
        {
            TradeNumber = trade.Id;
            Volume = (bet.LotsCount * trade.LotVolume.Value)
            .ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ОБЪЕМ"
            Price = (bet.Price.Value).ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                }); //"ЦЕНА"
            Sum = (bet.LotsCount * trade.LotVolume.Value * bet.Price.Value)
            .ToString("C2",
                new NumberFormatInfo()
                {
                    CurrencySymbol = "",
                    CurrencyDecimalSeparator = ".",
                    CurrencyGroupSeparator = " ",
                    CurrencyDecimalDigits = 0
                });
            Seller = trade.Seller.LongName;
            Buyer = bet.Buyer.LongName;
            Link = $"https://ptp.ua/trade/bill?t={trade.Id}&b={bet.BuyerId}";
            ProductName = StaticData.Inst.GetCatalogValue(trade.ProductName);
            Trade = true;
            Send = IsSend(user);
           // SendMessage();
        }
        public int TradeNumber;
        public string Volume;
        public string Price;
        public string Sum;
        public string Seller;
        public string Buyer;
        public string ProductName;
    }
}