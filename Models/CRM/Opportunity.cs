using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleverCRM.Models.CRM
{
    [Table("Opportunities", Schema = "dbo")]
    public partial class Opportunity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public decimal Amount { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Name { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string UserId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int ContactId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public int StatusId { get; set; }

        [Required]
        [ConcurrencyCheck]
        public DateTime CloseDate { get; set; }

        public Contact Contact { get; set; }

        public OpportunityStatus OpportunityStatus { get; set; }

        public ICollection<Task> Tasks { get; set; }

    }
}