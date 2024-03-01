﻿// <copyright file="RoleOneContainedInEnumerable.cs" company="Allors bv">
// Copyright (c) Allors bv. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Database.Adapters.Memory;

using System.Collections.Generic;
using Allors.Database.Meta;

internal sealed class ContainedInRoleOneEnumerable : ContainedIn
{
    private readonly IEnumerable<IObject> containingEnumerable;
    private readonly RoleType roleType;

    internal ContainedInRoleOneEnumerable(ExtentFiltered extent, RoleType roleType, IEnumerable<IObject> containingEnumerable)
    {
        extent.CheckForRoleType(roleType);
        PredicateAssertions.ValidateRoleContainedIn(roleType, containingEnumerable);

        this.roleType = roleType;
        this.containingEnumerable = containingEnumerable;
    }

    internal override ThreeValuedLogic Evaluate(Strategy strategy)
    {
        var containing = new HashSet<IObject>(this.containingEnumerable);
        var roleStrategy = strategy.GetCompositeRole(this.roleType);

        if (roleStrategy == null)
        {
            return ThreeValuedLogic.False;
        }

        return containing.Contains(roleStrategy)
            ? ThreeValuedLogic.True
            : ThreeValuedLogic.False;
    }
}