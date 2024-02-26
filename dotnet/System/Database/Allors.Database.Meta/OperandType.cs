﻿// <copyright file="IRelationEndType.cs" company="Allors bv">
// Copyright (c) Allors bv. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the RoleType type.</summary>

namespace Allors.Database.Meta;

using Embedded.Meta;

/// <summary>
///     A <see cref="OperandType" /> can be a <see cref="AssociationType" /> or a <see cref="RoleType" />.
/// </summary>
public abstract class OperandType : EmbeddedObject, IMetaExtensible
{
    protected OperandType(MetaPopulation metaPopulation, EmbeddedObjectType embeddedObjectType)
        : base(metaPopulation, embeddedObjectType)
    {
        this.MetaPopulation = metaPopulation;
        this.Attributes = new MetaExtension();
    }

    public MetaPopulation MetaPopulation { get; }

    public dynamic Attributes { get; }

    public static implicit operator OperandType(OperandTypeIndex index) => index?.OperandType;

    public static implicit operator OperandType(MethodTypeIndex index) => index?.MethodType;

    public static implicit operator OperandType(RelationEndTypeIndex index) => index?.RelationEndType;

    public static implicit operator OperandType(AssociationTypeIndex index) => index?.AssociationType;

    public static implicit operator OperandType(RoleTypeIndex index) => index?.RoleType;
}