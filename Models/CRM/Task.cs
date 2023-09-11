using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleverCRM.Models.CRM
{
    [Table("Tasks", Schema = "dbo")]
    public partial class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ConcurrencyCheck]
        public string Title { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int OpportunityId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime DueDate { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int TypeId { get; set; }

        [ConcurrencyCheck]
        public int? StatusId { get; set; }

        public Opportunity Opportunity { get; set; }

        public TaskStatus TaskStatus { get; set; }

        public TaskType TaskType { get; set; }

    }
}