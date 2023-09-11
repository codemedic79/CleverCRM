using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CleverCRM.Models.CRM
{
    [Table("Contacts", Schema = "dbo")]
    public partial class Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ConcurrencyCheck]
        public string Email { get; set; }

        [ConcurrencyCheck]
        public string Company { get; set; }

        [ConcurrencyCheck]
        public string LastName { get; set; }

        [ConcurrencyCheck]
        public string FirstName { get; set; }

        [ConcurrencyCheck]
        public string Phone { get; set; }

        public ICollection<Opportunity> Opportunities { get; set; }

    }
}