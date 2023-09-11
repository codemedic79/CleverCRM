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
    public partial class EditTask : ComponentBase
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

        [Parameter]
        public int Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            task = await CRMService.GetTaskById(Id);

            opportunitiesForOpportunityId = await CRMService.GetOpportunities();

            taskStatusesForStatusId = await CRMService.GetTaskStatuses();

            taskTypesForTypeId = await CRMService.GetTaskTypes();
        }
        protected bool errorVisible;
        protected CleverCRM.Models.CRM.Task task;

        protected IEnumerable<CleverCRM.Models.CRM.Opportunity> opportunitiesForOpportunityId;

        protected IEnumerable<CleverCRM.Models.CRM.TaskStatus> taskStatusesForStatusId;

        protected IEnumerable<CleverCRM.Models.CRM.TaskType> taskTypesForTypeId;

        protected async Task FormSubmit()
        {
            try
            {
                await CRMService.UpdateTask(Id, task);
                DialogService.Close(task);
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


        protected async Task ReloadButtonClick(MouseEventArgs args)
        {
           CRMService.Reset();
            hasChanges = false;
            canEdit = true;

            task = await CRMService.GetTaskById(Id);
        }
    }
}