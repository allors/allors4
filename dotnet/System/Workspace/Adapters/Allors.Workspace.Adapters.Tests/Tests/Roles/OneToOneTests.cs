// <copyright file="Many2OneTests.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Adapters.Tests
{
    using System.Threading.Tasks;
    using Allors.Workspace.Domain;
    using Allors.Workspace;
    using Xunit;
    using Allors.Workspace.Data;
    using System;
    using System.Linq;

    public abstract class OneToOneTests : Test
    {
        private Func<Context>[] contextFactories;
        protected OneToOneTests(Fixture fixture) : base(fixture)
        {
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await this.Login("administrator");

            var singleSessionContext = new SingleSessionContext(this, "Single shared");
            var multipleSessionContext = new MultipleSessionContext(this, "Multiple shared");

            this.contextFactories = new Func<Context>[]
            {
                () => singleSessionContext,
                //() => new SingleSessionContext(this, "Single"),
                //() => multipleSessionContext,
                () => new MultipleSessionContext(this, "Multiple"),
            };
        }

        [Fact]
        public async void SetRole()
        {
            foreach (DatabaseMode mode1 in Enum.GetValues(typeof(DatabaseMode)))
            {
                foreach (DatabaseMode mode2 in Enum.GetValues(typeof(DatabaseMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (workspace1, workspace2) = ctx;

                        var c1x_1 = await ctx.Create<C1>(workspace1, mode1);
                        var c1y_2 = await ctx.Create<C1>(workspace2, mode2);

                        await workspace2.PushAsync();
                        var result = await workspace1.PullAsync(new Pull { Object = c1y_2 });

                        var c1y_1 = (C1)result.Objects.Values.First();

                        c1y_1.ShouldNotBeNull(ctx, mode1, mode2);

                        if (!c1x_1.CanWriteC1C1One2One)
                        {
                            await workspace1.PullAsync(new Pull { Object = c1x_1 });
                        }

                        c1x_1.C1C1One2One = c1y_1;

                        c1x_1.C1C1One2One.ShouldEqual(c1y_1, ctx, mode1, mode2);
                        c1y_1.C1WhereC1C1One2One.ShouldEqual(c1x_1, ctx);
                    }
                }
            }
        }

        [Fact]
        public async void RemoveRole()
        {
            foreach (DatabaseMode mode1 in Enum.GetValues(typeof(DatabaseMode)))
            {
                foreach (DatabaseMode mode2 in Enum.GetValues(typeof(DatabaseMode)))
                {
                    foreach (var contextFactory in this.contextFactories)
                    {
                        var ctx = contextFactory();
                        var (workspace1, workspace2) = ctx;

                        var c1x_1 = await ctx.Create<C1>(workspace1, mode1);
                        var c1y_2 = await ctx.Create<C1>(workspace2, mode2);

                        await workspace2.PushAsync();
                        var result = await workspace1.PullAsync(new Pull { Object = c1y_2 });

                        var c1y_1 = (C1)result.Objects.Values.First();

                        c1y_1.ShouldNotBeNull(ctx, mode1, mode2);

                        if (!c1x_1.CanWriteC1C1One2One)
                        {
                            await workspace1.PullAsync(new Pull { Object = c1x_1 });
                        }

                        c1x_1.C1C1One2One = c1y_1;

                        c1x_1.C1C1One2One.ShouldEqual(c1y_1, ctx, mode1, mode2);
                        c1y_1.C1WhereC1C1One2One.ShouldEqual(c1x_1, ctx);

                        c1x_1.RemoveC1C1One2One();

                        c1x_1.C1C1One2One.ShouldNotEqual(c1y_1, ctx, mode1, mode2);
                        c1y_1.C1WhereC1C1One2One.ShouldNotEqual(c1x_1, ctx);
                    }
                }
            }
        }
    }
}