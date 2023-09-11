using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using Radzen;
using Radzen.Blazor;
using Microsoft.EntityFrameworkCore;
using CleverCRM.Models.Classes;

namespace CleverCRM.Pages
{
    public partial class Index : ComponentBase
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }

        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        public Data.CRMContext Context {get; set;}

        [Inject]
        public CRMService CRMService { get; set;}

        Stats monthlyStats;
        public IEnumerable<RevenueByCompany> revenueByCompany { get; set; }
        public IEnumerable<RevenueByMonth> revenueByMonth { get; set; }
        public IEnumerable<RevenueByEmployee> revenueByEmployee { get; set; }

        protected System.Linq.IQueryable<CleverCRM.Models.CRM.Opportunity> opportunities;

        protected System.Linq.IQueryable<CleverCRM.Models.CRM.Task> tasks;

     
        protected override async Task OnInitializedAsync()
        {
            monthlyStats = MonthlyStats();
            revenueByCompany = RevenueByCompany();
            revenueByMonth = RevenueByMonth();
            revenueByEmployee = RevenueByEmployee();
            opportunities = await CRMService.GetOpportunities();
            tasks = await CRMService.GetTasks();
        }


        public Stats MonthlyStats()
        {
            double wonOpportunities = Context.Opportunities
                                .Include(opportunity => opportunity.OpportunityStatus)
                                .Where(opportunity => opportunity.OpportunityStatus.Name == "Won")
                                .Count();

            var totalOpportunities = Context.Opportunities.Count();

            var ratio = wonOpportunities / totalOpportunities;

            return Context.Opportunities
                        .Include(opportunity => opportunity.OpportunityStatus)
                        .Where(opportunity => opportunity.OpportunityStatus.Name == "Won")
                        .ToList()
                        .GroupBy(opportunity => new DateTime(opportunity.CloseDate.Year, opportunity.CloseDate.Month, 1))
                        .Select(group => new Stats()
                        {
                            Month = group.Key,
                            Revenue = group.Sum(opportunity => opportunity.Amount),
                            Opportunities = group.Count(),
                            AverageDealSize = group.Average(opportunity => opportunity.Amount),
                            Ratio = ratio
                        })
                        .OrderBy(deals => deals.Month)
                        .LastOrDefault();
        }

        public IEnumerable<RevenueByCompany> RevenueByCompany()
        {
            return Context.Opportunities
                                    .Include(opportunity => opportunity.Contact)
                                    .ToList()
                                    .GroupBy(opportunity => opportunity.Contact.Company)
                                    .Select(group => new RevenueByCompany()
                                    {
                                        Company = group.Key,
                                        Revenue = group.Sum(opportunity => opportunity.Amount)
                                    });
        }

        public IEnumerable<RevenueByEmployee> RevenueByEmployee()
        {
            return Context.Opportunities
                                    .Include(opportunity => opportunity.User)
                                    .ToList()
                                    .GroupBy(opportunity => $"{opportunity.User.FirstName} {opportunity.User.LastName}")
                                    .Select(group => new RevenueByEmployee()
                                    {
                                        Employee = group.Key,
                                        Revenue = group.Sum(opportunity => opportunity.Amount)
                                    });
        }

        public IEnumerable<RevenueByMonth> RevenueByMonth()
        {
            return Context.Opportunities
                                    .Include(opportunity => opportunity.OpportunityStatus)
                                    .Where(opportunity => opportunity.OpportunityStatus.Name == "Won")
                                    .ToList()
                                    .GroupBy(opportunity => new DateTime(opportunity.CloseDate.Year, opportunity.CloseDate.Month, 1))
                                    .Select(group => new RevenueByMonth()
                                    {
                                        Revenue = group.Sum(opportunity => opportunity.Amount),
                                        Month = group.Key
                                    })
                                    .OrderBy(deals => deals.Month);
        }       
        
    }
    

}