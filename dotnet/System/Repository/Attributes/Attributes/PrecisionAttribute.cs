// <copyright file="PrecisionAttribute.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the Extent type.</summary>

namespace Allors.Repository.Attributes
{
    using System;

    [AttributeUsage(AttributeTargets.Property)]
    public class PrecisionAttribute : RepositoryAttribute
    {
        public PrecisionAttribute(int value) => this.Value = value;

        public int Value { get; set; }
    }
}