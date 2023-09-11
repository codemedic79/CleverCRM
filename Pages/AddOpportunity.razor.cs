using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace CleverCRM.Pages
{
    public partial class AddOpportunity : ComponentBase
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
        public CRMService CRMService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            opportunity = new CleverCRM.Models.CRM.Opportunity();

            contactsForContactId = await CRMService.GetContacts();

            opportunityStatusesForStatusId = await CRMService.GetOpportunityStatuses();
        }
        protected bool errorVisible;
        protected CleverCRM.Models.CRM.Opportunity opportunity;

        protected IEnumerable<CleverCRM.Models.CRM.Contact> contactsForContactId;

        protected IEnumerable<CleverCRM.Models.CRM.OpportunityStatus> opportunityStatusesForStatusId;

        protected async Task FormSubmit()
        {
            try
            {
                await CRMService.CreateOpportunity(opportunity);
                DialogService.Close(opportunity);
            }
            catch (Exception ex)
            {
                hasChanges = ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException;
                canEdit = !(ex is Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException);
                errorVisible = true;
            }
        }

        protected async Task CancelButtonClick(MouseEventArgs args)
        {
            DialogService.Close(null);
        }


        protected bool hasChanges = false;
        protected bool canEdit = true;

        [Inject]
        protected SecurityService Security { get; set; }
    }
}