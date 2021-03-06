﻿using Raffle.Core;
using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Raffle.Tests.Core
{
    public class ItemManagerTests
    {
        public class RaffleBuying
        {
            private UserProfile owner;
            private UserProfile buyer;
            private Item item;

            private void Setup()
            {
                owner = new UserProfile
                {
                    UserId = 1,
                    UserName = "user",
                };

                buyer = new UserProfile
                {
                    UserId = 2,
                    UserName = "buyer",
                };

                item = new Item
                {
                    Id = 1,
                    Name = "p1",
                    Description = "p1 description",
                    Price = 100,
                    OwnerId = owner.UserId
                };
            }

            [Fact, AutoRollback]
            public void AnItemOwnerCannotBuyARaffleForItsOwnItem()
            {
                Setup();

                var im = new ItemManager(item);

                Assert.Throws<InvalidOperationException>(() => im.BuyRaffle(this.owner));
            }

            [Fact, AutoRollback]
            public void AUserCannotBuyARaffleForAClosedItem()
            {
                Setup();

                this.item.Raffles = new Raffle.Models.Raffle[100].ToList();

                var im = new ItemManager(item);

                Assert.Throws<InvalidOperationException>(() => im.BuyRaffle(this.buyer));
            }

            [Fact, AutoRollback]
            public void AUserCannotBuyARaffleIfHeDoesNotHaveEnoughMoney()
            {
                Setup();

                this.item.Price = 100;

                var im = new ItemManager(item);

                Assert.Throws<InvalidOperationException>(() => im.BuyRaffle(this.buyer));
            }

            [Fact, AutoRollback]
            public void AUserShouldBeAbleToBuyARaffleInTheRightConditions()
            {
                Setup();

                this.item.Price = 100;

                var im = new ItemManager(item);

                var raffle = im.BuyRaffle(this.buyer);

                Assert.Equal(this.buyer.UserId, raffle.UserProfileId);
                Assert.Equal(this.item.Id, raffle.ItemId);
                Assert.True(raffle.RaffleNumber > 0);

                Assert.True(this.item.CanBuy);
                Assert.Null(raffle.IsPrized);
            }

            [Fact, AutoRollback]
            public void BuyingTheLastRaffleShouldCloseAnItemAndProvideAWinner()
            {
                Setup();

                this.item.Price = 1;

                var im = new ItemManager(item);

                var raffle = im.BuyRaffle(this.buyer);

                Assert.False(this.item.CanBuy);
                Assert.NotNull(raffle.IsPrized);
            }
        }
    }
}
