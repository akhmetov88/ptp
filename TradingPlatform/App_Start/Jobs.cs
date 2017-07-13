using System;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using Quartz;
using TradingPlatform.Data.DataLayer;
using TradingPlatform.Models;


namespace TradingPlatform
{
    public class Jobs
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
 
        [DisallowConcurrentExecution]
        public class CheckNewTradesJob : IJob
        {
            public async void Execute(IJobExecutionContext context)
            {
                try
                {
                    using (var db = ApplicationDbContext.Create())
                    {
                        JobDataLayer dl = new JobDataLayer(db);

                      //  await dl.SendNotificationsAboutNewTrade();
                        var trades = dl.GetSuccesTrades().Distinct().ToList();
                        foreach (var trade in trades)
                        {
                            if (trades.Any())
                            {
                                trade.IsClosedByBills = true;
                                await db.UpdateEntityAsync(trade);
                                dl.CloseTrade(trade);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    
                }
            }
        }


    }
}
