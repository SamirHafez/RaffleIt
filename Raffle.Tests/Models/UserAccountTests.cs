﻿using Raffle.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Extensions;

namespace Raffle.Tests.Models
{
    public class UserAccountTests
    {
        public class Creation
        {
            public Creation()
            {
                AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ""));
                Database.SetInitializer<Context>(new DropCreateDatabaseIfModelChanges<Context>());
                using (var context = new Context())
                    context.Database.Initialize(force: true);
            }

            [Fact, AutoRollback]
            public void ShouldCreateAnAccount()
            {
                using (var context = new Context())
                {
                    var account = new UserProfile
                    {
                        UserName = "user",
                    };

                    context.UserProfiles.Add(account);

                    context.SaveChanges();

                    Assert.True(account.UserId > 0);

                    Assert.True(account.Items.Count == 0);
                    Assert.True(account.Raffles.Count == 0);

                    Assert.True(account.Reputation == 0);
                }
            }
        }
    }
}
