using System;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Radzen;

using CleverCRM.Data;

namespace CleverCRM
{
    public partial class CRMService
    {
        CRMContext Context
        {
           get
           {
             return this.context;
           }
        }

        private readonly CRMContext context;
        private readonly NavigationManager navigationManager;

        public CRMService(CRMContext context, NavigationManager navigationManager)
        {
            this.context = context;
            this.navigationManager = navigationManager;
        }

        public void Reset() => Context.ChangeTracker.Entries().Where(e => e.Entity != null).ToList().ForEach(e => e.State = EntityState.Detached);

        public void ApplyQuery<T>(ref IQueryable<T> items, Query query = null)
        {
            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Filter))
                {
                    if (query.FilterParameters != null)
                    {
                        items = items.Where(query.Filter, query.FilterParameters);
                    }
                    else
                    {
                        items = items.Where(query.Filter);
                    }
                }

                if (!string.IsNullOrEmpty(query.OrderBy))
                {
                    items = items.OrderBy(query.OrderBy);
                }

                if (query.Skip.HasValue)
                {
                    items = items.Skip(query.Skip.Value);
                }

                if (query.Top.HasValue)
                {
                    items = items.Take(query.Top.Value);
                }
            }
        }


        public async Task ExportContactsToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/contacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/contacts/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportContactsToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/contacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/contacts/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnContactsRead(ref IQueryable<CleverCRM.Models.CRM.Contact> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.Contact>> GetContacts(Query query = null)
        {
            var items = Context.Contacts.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnContactsRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnContactGet(CleverCRM.Models.CRM.Contact item);
        partial void OnGetContactById(ref IQueryable<CleverCRM.Models.CRM.Contact> items);


        public async Task<CleverCRM.Models.CRM.Contact> GetContactById(int id)
        {
            var items = Context.Contacts
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetContactById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnContactGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnContactCreated(CleverCRM.Models.CRM.Contact item);
        partial void OnAfterContactCreated(CleverCRM.Models.CRM.Contact item);

        public async Task<CleverCRM.Models.CRM.Contact> CreateContact(CleverCRM.Models.CRM.Contact contact)
        {
            OnContactCreated(contact);

            var existingItem = Context.Contacts
                              .Where(i => i.Id == contact.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Contacts.Add(contact);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(contact).State = EntityState.Detached;
                throw;
            }

            OnAfterContactCreated(contact);

            return contact;
        }

        public async Task<CleverCRM.Models.CRM.Contact> CancelContactChanges(CleverCRM.Models.CRM.Contact item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnContactUpdated(CleverCRM.Models.CRM.Contact item);
        partial void OnAfterContactUpdated(CleverCRM.Models.CRM.Contact item);

        public async Task<CleverCRM.Models.CRM.Contact> UpdateContact(int id, CleverCRM.Models.CRM.Contact contact)
        {
            OnContactUpdated(contact);

            var itemToUpdate = Context.Contacts
                              .Where(i => i.Id == contact.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(contact);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterContactUpdated(contact);

            return contact;
        }

        partial void OnContactDeleted(CleverCRM.Models.CRM.Contact item);
        partial void OnAfterContactDeleted(CleverCRM.Models.CRM.Contact item);

        public async Task<CleverCRM.Models.CRM.Contact> DeleteContact(int id)
        {
            var itemToDelete = Context.Contacts
                              .Where(i => i.Id == id)
                              .Include(i => i.Opportunities)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnContactDeleted(itemToDelete);


            Context.Contacts.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterContactDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOpportunitiesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/opportunities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/opportunities/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpportunitiesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/opportunities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/opportunities/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpportunitiesRead(ref IQueryable<CleverCRM.Models.CRM.Opportunity> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.Opportunity>> GetOpportunities(Query query = null)
        {
            var items = Context.Opportunities.AsQueryable();

            items = items.Include(i => i.Contact);
            items = items.Include(i => i.OpportunityStatus);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOpportunitiesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOpportunityGet(CleverCRM.Models.CRM.Opportunity item);
        partial void OnGetOpportunityById(ref IQueryable<CleverCRM.Models.CRM.Opportunity> items);


        public async Task<CleverCRM.Models.CRM.Opportunity> GetOpportunityById(int id)
        {
            var items = Context.Opportunities
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Contact);
            items = items.Include(i => i.OpportunityStatus);
 
            OnGetOpportunityById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOpportunityGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOpportunityCreated(CleverCRM.Models.CRM.Opportunity item);
        partial void OnAfterOpportunityCreated(CleverCRM.Models.CRM.Opportunity item);

        public async Task<CleverCRM.Models.CRM.Opportunity> CreateOpportunity(CleverCRM.Models.CRM.Opportunity opportunity)
        {
            OnOpportunityCreated(opportunity);

            var existingItem = Context.Opportunities
                              .Where(i => i.Id == opportunity.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Opportunities.Add(opportunity);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(opportunity).State = EntityState.Detached;
                throw;
            }

            OnAfterOpportunityCreated(opportunity);

            return opportunity;
        }

        public async Task<CleverCRM.Models.CRM.Opportunity> CancelOpportunityChanges(CleverCRM.Models.CRM.Opportunity item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOpportunityUpdated(CleverCRM.Models.CRM.Opportunity item);
        partial void OnAfterOpportunityUpdated(CleverCRM.Models.CRM.Opportunity item);

        public async Task<CleverCRM.Models.CRM.Opportunity> UpdateOpportunity(int id, CleverCRM.Models.CRM.Opportunity opportunity)
        {
            OnOpportunityUpdated(opportunity);

            var itemToUpdate = Context.Opportunities
                              .Where(i => i.Id == opportunity.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(opportunity);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOpportunityUpdated(opportunity);

            return opportunity;
        }

        partial void OnOpportunityDeleted(CleverCRM.Models.CRM.Opportunity item);
        partial void OnAfterOpportunityDeleted(CleverCRM.Models.CRM.Opportunity item);

        public async Task<CleverCRM.Models.CRM.Opportunity> DeleteOpportunity(int id)
        {
            var itemToDelete = Context.Opportunities
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOpportunityDeleted(itemToDelete);


            Context.Opportunities.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOpportunityDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportOpportunityStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/opportunitystatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/opportunitystatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportOpportunityStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/opportunitystatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/opportunitystatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnOpportunityStatusesRead(ref IQueryable<CleverCRM.Models.CRM.OpportunityStatus> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.OpportunityStatus>> GetOpportunityStatuses(Query query = null)
        {
            var items = Context.OpportunityStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnOpportunityStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnOpportunityStatusGet(CleverCRM.Models.CRM.OpportunityStatus item);
        partial void OnGetOpportunityStatusById(ref IQueryable<CleverCRM.Models.CRM.OpportunityStatus> items);


        public async Task<CleverCRM.Models.CRM.OpportunityStatus> GetOpportunityStatusById(int id)
        {
            var items = Context.OpportunityStatuses
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetOpportunityStatusById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnOpportunityStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnOpportunityStatusCreated(CleverCRM.Models.CRM.OpportunityStatus item);
        partial void OnAfterOpportunityStatusCreated(CleverCRM.Models.CRM.OpportunityStatus item);

        public async Task<CleverCRM.Models.CRM.OpportunityStatus> CreateOpportunityStatus(CleverCRM.Models.CRM.OpportunityStatus opportunitystatus)
        {
            OnOpportunityStatusCreated(opportunitystatus);

            var existingItem = Context.OpportunityStatuses
                              .Where(i => i.Id == opportunitystatus.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.OpportunityStatuses.Add(opportunitystatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(opportunitystatus).State = EntityState.Detached;
                throw;
            }

            OnAfterOpportunityStatusCreated(opportunitystatus);

            return opportunitystatus;
        }

        public async Task<CleverCRM.Models.CRM.OpportunityStatus> CancelOpportunityStatusChanges(CleverCRM.Models.CRM.OpportunityStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnOpportunityStatusUpdated(CleverCRM.Models.CRM.OpportunityStatus item);
        partial void OnAfterOpportunityStatusUpdated(CleverCRM.Models.CRM.OpportunityStatus item);

        public async Task<CleverCRM.Models.CRM.OpportunityStatus> UpdateOpportunityStatus(int id, CleverCRM.Models.CRM.OpportunityStatus opportunitystatus)
        {
            OnOpportunityStatusUpdated(opportunitystatus);

            var itemToUpdate = Context.OpportunityStatuses
                              .Where(i => i.Id == opportunitystatus.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(opportunitystatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterOpportunityStatusUpdated(opportunitystatus);

            return opportunitystatus;
        }

        partial void OnOpportunityStatusDeleted(CleverCRM.Models.CRM.OpportunityStatus item);
        partial void OnAfterOpportunityStatusDeleted(CleverCRM.Models.CRM.OpportunityStatus item);

        public async Task<CleverCRM.Models.CRM.OpportunityStatus> DeleteOpportunityStatus(int id)
        {
            var itemToDelete = Context.OpportunityStatuses
                              .Where(i => i.Id == id)
                              .Include(i => i.Opportunities)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnOpportunityStatusDeleted(itemToDelete);


            Context.OpportunityStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterOpportunityStatusDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTasksToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/tasks/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTasksToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/tasks/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTasksRead(ref IQueryable<CleverCRM.Models.CRM.Task> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.Task>> GetTasks(Query query = null)
        {
            var items = Context.Tasks.AsQueryable();

            items = items.Include(i => i.Opportunity);
            items = items.Include(i => i.TaskStatus);
            items = items.Include(i => i.TaskType);

            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTasksRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskGet(CleverCRM.Models.CRM.Task item);
        partial void OnGetTaskById(ref IQueryable<CleverCRM.Models.CRM.Task> items);


        public async Task<CleverCRM.Models.CRM.Task> GetTaskById(int id)
        {
            var items = Context.Tasks
                              .AsNoTracking()
                              .Where(i => i.Id == id);

            items = items.Include(i => i.Opportunity);
            items = items.Include(i => i.TaskStatus);
            items = items.Include(i => i.TaskType);
 
            OnGetTaskById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskCreated(CleverCRM.Models.CRM.Task item);
        partial void OnAfterTaskCreated(CleverCRM.Models.CRM.Task item);

        public async Task<CleverCRM.Models.CRM.Task> CreateTask(CleverCRM.Models.CRM.Task task)
        {
            OnTaskCreated(task);

            var existingItem = Context.Tasks
                              .Where(i => i.Id == task.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.Tasks.Add(task);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(task).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskCreated(task);

            return task;
        }

        public async Task<CleverCRM.Models.CRM.Task> CancelTaskChanges(CleverCRM.Models.CRM.Task item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskUpdated(CleverCRM.Models.CRM.Task item);
        partial void OnAfterTaskUpdated(CleverCRM.Models.CRM.Task item);

        public async Task<CleverCRM.Models.CRM.Task> UpdateTask(int id, CleverCRM.Models.CRM.Task task)
        {
            OnTaskUpdated(task);

            var itemToUpdate = Context.Tasks
                              .Where(i => i.Id == task.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(task);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskUpdated(task);

            return task;
        }

        partial void OnTaskDeleted(CleverCRM.Models.CRM.Task item);
        partial void OnAfterTaskDeleted(CleverCRM.Models.CRM.Task item);

        public async Task<CleverCRM.Models.CRM.Task> DeleteTask(int id)
        {
            var itemToDelete = Context.Tasks
                              .Where(i => i.Id == id)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskDeleted(itemToDelete);


            Context.Tasks.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTaskStatusesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/taskstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/taskstatuses/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskStatusesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/taskstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/taskstatuses/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskStatusesRead(ref IQueryable<CleverCRM.Models.CRM.TaskStatus> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.TaskStatus>> GetTaskStatuses(Query query = null)
        {
            var items = Context.TaskStatuses.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskStatusesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskStatusGet(CleverCRM.Models.CRM.TaskStatus item);
        partial void OnGetTaskStatusById(ref IQueryable<CleverCRM.Models.CRM.TaskStatus> items);


        public async Task<CleverCRM.Models.CRM.TaskStatus> GetTaskStatusById(int id)
        {
            var items = Context.TaskStatuses
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetTaskStatusById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskStatusGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskStatusCreated(CleverCRM.Models.CRM.TaskStatus item);
        partial void OnAfterTaskStatusCreated(CleverCRM.Models.CRM.TaskStatus item);

        public async Task<CleverCRM.Models.CRM.TaskStatus> CreateTaskStatus(CleverCRM.Models.CRM.TaskStatus taskstatus)
        {
            OnTaskStatusCreated(taskstatus);

            var existingItem = Context.TaskStatuses
                              .Where(i => i.Id == taskstatus.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.TaskStatuses.Add(taskstatus);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(taskstatus).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskStatusCreated(taskstatus);

            return taskstatus;
        }

        public async Task<CleverCRM.Models.CRM.TaskStatus> CancelTaskStatusChanges(CleverCRM.Models.CRM.TaskStatus item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskStatusUpdated(CleverCRM.Models.CRM.TaskStatus item);
        partial void OnAfterTaskStatusUpdated(CleverCRM.Models.CRM.TaskStatus item);

        public async Task<CleverCRM.Models.CRM.TaskStatus> UpdateTaskStatus(int id, CleverCRM.Models.CRM.TaskStatus taskstatus)
        {
            OnTaskStatusUpdated(taskstatus);

            var itemToUpdate = Context.TaskStatuses
                              .Where(i => i.Id == taskstatus.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(taskstatus);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskStatusUpdated(taskstatus);

            return taskstatus;
        }

        partial void OnTaskStatusDeleted(CleverCRM.Models.CRM.TaskStatus item);
        partial void OnAfterTaskStatusDeleted(CleverCRM.Models.CRM.TaskStatus item);

        public async Task<CleverCRM.Models.CRM.TaskStatus> DeleteTaskStatus(int id)
        {
            var itemToDelete = Context.TaskStatuses
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskStatusDeleted(itemToDelete);


            Context.TaskStatuses.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskStatusDeleted(itemToDelete);

            return itemToDelete;
        }
    
        public async Task ExportTaskTypesToExcel(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/tasktypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/tasktypes/excel(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        public async Task ExportTaskTypesToCSV(Query query = null, string fileName = null)
        {
            navigationManager.NavigateTo(query != null ? query.ToUrl($"export/crm/tasktypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')") : $"export/crm/tasktypes/csv(fileName='{(!string.IsNullOrEmpty(fileName) ? UrlEncoder.Default.Encode(fileName) : "Export")}')", true);
        }

        partial void OnTaskTypesRead(ref IQueryable<CleverCRM.Models.CRM.TaskType> items);

        public async Task<IQueryable<CleverCRM.Models.CRM.TaskType>> GetTaskTypes(Query query = null)
        {
            var items = Context.TaskTypes.AsQueryable();


            if (query != null)
            {
                if (!string.IsNullOrEmpty(query.Expand))
                {
                    var propertiesToExpand = query.Expand.Split(',');
                    foreach(var p in propertiesToExpand)
                    {
                        items = items.Include(p.Trim());
                    }
                }

                ApplyQuery(ref items, query);
            }

            OnTaskTypesRead(ref items);

            return await Task.FromResult(items);
        }

        partial void OnTaskTypeGet(CleverCRM.Models.CRM.TaskType item);
        partial void OnGetTaskTypeById(ref IQueryable<CleverCRM.Models.CRM.TaskType> items);


        public async Task<CleverCRM.Models.CRM.TaskType> GetTaskTypeById(int id)
        {
            var items = Context.TaskTypes
                              .AsNoTracking()
                              .Where(i => i.Id == id);

 
            OnGetTaskTypeById(ref items);

            var itemToReturn = items.FirstOrDefault();

            OnTaskTypeGet(itemToReturn);

            return await Task.FromResult(itemToReturn);
        }

        partial void OnTaskTypeCreated(CleverCRM.Models.CRM.TaskType item);
        partial void OnAfterTaskTypeCreated(CleverCRM.Models.CRM.TaskType item);

        public async Task<CleverCRM.Models.CRM.TaskType> CreateTaskType(CleverCRM.Models.CRM.TaskType tasktype)
        {
            OnTaskTypeCreated(tasktype);

            var existingItem = Context.TaskTypes
                              .Where(i => i.Id == tasktype.Id)
                              .FirstOrDefault();

            if (existingItem != null)
            {
               throw new Exception("Item already available");
            }            

            try
            {
                Context.TaskTypes.Add(tasktype);
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(tasktype).State = EntityState.Detached;
                throw;
            }

            OnAfterTaskTypeCreated(tasktype);

            return tasktype;
        }

        public async Task<CleverCRM.Models.CRM.TaskType> CancelTaskTypeChanges(CleverCRM.Models.CRM.TaskType item)
        {
            var entityToCancel = Context.Entry(item);
            if (entityToCancel.State == EntityState.Modified)
            {
              entityToCancel.CurrentValues.SetValues(entityToCancel.OriginalValues);
              entityToCancel.State = EntityState.Unchanged;
            }

            return item;
        }

        partial void OnTaskTypeUpdated(CleverCRM.Models.CRM.TaskType item);
        partial void OnAfterTaskTypeUpdated(CleverCRM.Models.CRM.TaskType item);

        public async Task<CleverCRM.Models.CRM.TaskType> UpdateTaskType(int id, CleverCRM.Models.CRM.TaskType tasktype)
        {
            OnTaskTypeUpdated(tasktype);

            var itemToUpdate = Context.TaskTypes
                              .Where(i => i.Id == tasktype.Id)
                              .FirstOrDefault();

            if (itemToUpdate == null)
            {
               throw new Exception("Item no longer available");
            }
                
            var entryToUpdate = Context.Entry(itemToUpdate);
            entryToUpdate.CurrentValues.SetValues(tasktype);
            entryToUpdate.State = EntityState.Modified;

            Context.SaveChanges();

            OnAfterTaskTypeUpdated(tasktype);

            return tasktype;
        }

        partial void OnTaskTypeDeleted(CleverCRM.Models.CRM.TaskType item);
        partial void OnAfterTaskTypeDeleted(CleverCRM.Models.CRM.TaskType item);

        public async Task<CleverCRM.Models.CRM.TaskType> DeleteTaskType(int id)
        {
            var itemToDelete = Context.TaskTypes
                              .Where(i => i.Id == id)
                              .Include(i => i.Tasks)
                              .FirstOrDefault();

            if (itemToDelete == null)
            {
               throw new Exception("Item no longer available");
            }

            OnTaskTypeDeleted(itemToDelete);


            Context.TaskTypes.Remove(itemToDelete);

            try
            {
                Context.SaveChanges();
            }
            catch
            {
                Context.Entry(itemToDelete).State = EntityState.Unchanged;
                throw;
            }

            OnAfterTaskTypeDeleted(itemToDelete);

            return itemToDelete;
        }
        }
}