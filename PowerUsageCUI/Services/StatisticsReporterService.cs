using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using PowerUsageCUI.DataModel.PowerModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerUsageCUI.Services
{
    public class StatisticsReporterService
    {
        private ILogger<StatisticsReporterService> logger;


        public StatisticsReporterService(ILogger<StatisticsReporterService> logger) 
        {
            Guard.Against.Null(logger);

            this.logger = logger;
        }

        public void ShowStatistics()
        {
            using var db = new PowerModelContext();


            // First check if we have sufficient data to avoid SQLlite issues
            if (!db.ThreePhaseMeterSet.Where(pred => pred.DateTime >= DateTime.Now.AddMinutes(-1)).Any())
            {
                // Insufficient data to perform action
                return;
            }

            var result =
            db.ThreePhaseMeterSet.
                Where(pred => pred.DateTime >= DateTime.Now.AddMinutes(-1)).
                Select(report => new
                {
                    machine = report.DeviceName,
                    date = report.DateTime,
                    totalpower = report.TotalActivePower
                })
                .GroupBy(x => x.machine)
                .Select(sel => new
                {
                    machine = sel.Key,
                    AverageTotalPower = sel.Average(q => q.totalpower),
                    MinTotalPower = sel.Min(q => q.totalpower),
                    MaxTotalPower = sel.Max(q => q.totalpower)
                }).FirstOrDefault();

            if (result != null) 
                logger.LogInformation(result.ToString());
        }

        
    }
}
