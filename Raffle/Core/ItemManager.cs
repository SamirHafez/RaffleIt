﻿using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Raffle.Core
{
    public class ItemManager
    {
        private readonly Item item;

        public ItemManager(Item item)
        {
            this.item = item;
        }

        public Raffle.Models.Raffle BuyRaffle(UserProfile buyer)
        {
            lock (this.item)
            {
                ValidateTransaction(buyer);

                Raffle.Models.Raffle raffle = ExecuteTransaction(buyer);

                if (!this.item.CanBuy)
                    SetWinner();

                return raffle;
            }
        }

        private void SetWinner()
        {
            Raffle.Models.Raffle selectedRaffle = this.item.Raffles.OrderBy(r => Guid.NewGuid()).First();
            IEnumerable<Raffle.Models.Raffle> otherRaffles = this.item.Raffles.Where(r => r.RaffleNumber != selectedRaffle.RaffleNumber);

            selectedRaffle.IsPrized = true;

            foreach (var raffle in otherRaffles)
                raffle.IsPrized = false;

            this.item.ClosedAt = DateTime.Now;
        }

        private void ValidateTransaction(UserProfile buyer)
        {
            if (!this.item.CanBuy)
                throw new InvalidOperationException();

            if (this.item.OwnerId == buyer.UserId)
                throw new InvalidOperationException();
        }

        private Raffle.Models.Raffle ExecuteTransaction(UserProfile buyer)
        {
            int mostRecentRaffleNumber = this.item.Raffles.OrderByDescending(r => r.RaffleNumber)
                                                              .Select(r => r.RaffleNumber)
                                                              .FirstOrDefault();

            var raffle = new Raffle.Models.Raffle
            {
                ItemId = this.item.Id,
                UserProfileId = buyer.UserId,
                RaffleNumber = mostRecentRaffleNumber + 1,
                PurchasedAt = DateTime.Now
            };

            this.item.Raffles.Add(raffle);

            return raffle;
        }
    }
}