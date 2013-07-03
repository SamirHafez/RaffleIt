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

        [Required(ErrorMessage="*")]
        [MaxLength(40)]
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        [MaxLength(200)]
        public string Description { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Currency)]
        public int Price { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public DateTime? ShippedAt { get; set; }

        public DateTime? ReceivedAt { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        [ForeignKey("Category")]
        [Required(ErrorMessage = "*")]
        public int CategoryId { get; set; }

        public virtual Category Category { get; set; }

        public virtual UserProfile Owner { get; set; }

        public virtual ICollection<Raffle> Raffles { get; set; }

        public bool CanBuy
        {
            get { return Raffles.Count != Price + (int)(Price * 0.40); }
        }

        public Item()
        {
            Raffles = new List<Raffle>();
        }
    }

    [Table("Raffle")]
    public class Raffle 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("UserProfile")]
        public int UserProfileId { get; set; }

        [ForeignKey("Item")]
        public int ItemId { get; set; }

        public int RaffleNumber { get; set; }

        public bool? IsPrized { get; set; }

        public DateTime PurchasedAt { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual Item Item { get; set; }
    }

    [Table("Category")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "*")]
        public string Name { get; set; }
    }
}