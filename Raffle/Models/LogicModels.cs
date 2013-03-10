using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Raffle.Models
{
    [Table("Item")]
    public class Item 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public int Price { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        public int RafflePrice { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        public virtual UserProfile Owner { get; set; }

        public virtual ICollection<Raffle> Raffles { get; set; }

        public Item()
        {
            Raffles = new List<Raffle>();
        }
    }

    [Table("Raffle")]
    public class Raffle 
    {
        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public int RaffleNumber { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual Item Item { get; set; }
    }
}