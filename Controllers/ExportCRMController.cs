using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

using CleverCRM.Data;

namespace CleverCRM.Controllers
{
    public partial class ExportCRMController : ExportController
    {
        private readonly CRMContext context;
        private readonly CRMService service;

        public ExportCRMController(CRMContext context, CRMService service)
        {
            this.service = service;
            this.context = context;
        }

        [HttpGet("/export/CRM/contacts/csv")]
        [HttpGet("/export/CRM/contacts/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportContactsToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetContacts(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/contacts/excel")]
        [HttpGet("/export/CRM/contacts/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportContactsToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetContacts(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/opportunities/csv")]
        [HttpGet("/export/CRM/opportunities/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpportunitiesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpportunities(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/opportunities/excel")]
        [HttpGet("/export/CRM/opportunities/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpportunitiesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpportunities(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/opportunitystatuses/csv")]
        [HttpGet("/export/CRM/opportunitystatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpportunityStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetOpportunityStatuses(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/opportunitystatuses/excel")]
        [HttpGet("/export/CRM/opportunitystatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportOpportunityStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetOpportunityStatuses(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/tasks/csv")]
        [HttpGet("/export/CRM/tasks/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTasks(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/tasks/excel")]
        [HttpGet("/export/CRM/tasks/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTasksToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTasks(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/taskstatuses/csv")]
        [HttpGet("/export/CRM/taskstatuses/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskStatusesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskStatuses(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/taskstatuses/excel")]
        [HttpGet("/export/CRM/taskstatuses/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskStatusesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskStatuses(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/tasktypes/csv")]
        [HttpGet("/export/CRM/tasktypes/csv(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskTypesToCSV(string fileName = null)
        {
            return ToCSV(ApplyQuery(await service.GetTaskTypes(), Request.Query), fileName);
        }

        [HttpGet("/export/CRM/tasktypes/excel")]
        [HttpGet("/export/CRM/tasktypes/excel(fileName='{fileName}')")]
        public async Task<FileStreamResult> ExportTaskTypesToExcel(string fileName = null)
        {
            return ToExcel(ApplyQuery(await service.GetTaskTypes(), Request.Query), fileName);
        }
    }
}
