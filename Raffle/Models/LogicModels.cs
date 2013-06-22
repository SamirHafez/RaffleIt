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
        public string Name { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [Required(ErrorMessage = "*")]
        public string Category { get; set; }

        [Required(ErrorMessage = "*")]
        [DataType(DataType.Currency)]
        public int Price { get; set; }

        [Required(ErrorMessage = "*")]
        [Display(Name = "Number of Raffles")]
        public int TotalRaffleCount { get; set; }

        public string PaypalCode { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }

        public bool? DeliveredSuccess { get; set; }

        [ForeignKey("Owner")]
        public int OwnerId { get; set; }

        public virtual UserProfile Owner { get; set; }

        public virtual ICollection<Raffle> Raffles { get; set; }

        public bool CanBuy
        {
            get { return Raffles.Count != TotalRaffleCount; }
        }

        public decimal RafflePrice
        {
            get { return decimal.Round((decimal)Price / TotalRaffleCount, 2); }
        }

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

        public bool? IsPrized { get; set; }

        public DateTime PurchasedAt { get; set; }

        public virtual UserProfile UserProfile { get; set; }

        public virtual Item Item { get; set; }
    }
}