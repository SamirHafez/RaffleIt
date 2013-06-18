using Raffle.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Raffle.Tests.Models
{
    public class ItemTests
    {
        public class Creation
        {
            public Creation()
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
                using (var context = new Context())
                    context.Database.Initialize(force: true);
            }

            [Fact, AutoRollback]
            public void ShouldCreateAnItem()
            {
                using (var context = new Context())
                {
                    var account = new UserProfile
                    {
                        UserName = "user",
                        Email = "user@raffle.com"
                    };

                    context.UserProfiles.Add(account);

                    context.SaveChanges();

                    var item = new Item
                    {
                        Name = "p1",
                        Description = "p1 description",
                        Price = 100,
                        TotalRaffleCount = 100,
                        OwnerId = account.UserId
                    };

                    account.Items.Add(item);

                    context.SaveChanges();

                    Assert.True(item.Id > 0);
                    Assert.Equal(100, item.Price);
                    Assert.Equal(100, item.TotalRaffleCount);
                    Assert.Equal(account.UserId, item.OwnerId);
                }
            }
        }
    }
}
