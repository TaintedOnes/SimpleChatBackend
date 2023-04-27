using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleChat.Core.Entities
{
    public class Conversation
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        public long ChatId { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(32)]
        public string UserName { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string FirstName { get; set; }
        [Column(TypeName = "VARCHAR")]
        [StringLength(64)]
        public string LastName { get; set; }
        public DateTime LastMessageDate { get; set; }
    }
}
