﻿// <copyright file="Unit.cs" company="Allors bv">
// Copyright (c) Allors bv. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the IObjectType type.</summary>

namespace Allors.Database.Meta;

using System;
using System.Collections.Generic;
using Embedded;
using Embedded.Meta;

public sealed class Unit : EmbeddedObject, IObjectType
{
    private readonly IEmbeddedUnitRole<string> singularName;
    private readonly IEmbeddedUnitRole<string> assignedPluralName;
    private readonly IEmbeddedUnitRole<string> pluralName;

    public Unit(MetaPopulation metaPopulation, EmbeddedObjectType embeddedObjectType)
        : base(metaPopulation, embeddedObjectType)
    {
        this.Attributes = new MetaExtension();
        this.MetaPopulation = metaPopulation;

        this.singularName = this.EmbeddedPopulation.EmbeddedGetUnitRole<string>(this, metaPopulation.EmbeddedRoleTypes.ObjectTypeSingularName);
        this.assignedPluralName = this.EmbeddedPopulation.EmbeddedGetUnitRole<string>(this, metaPopulation.EmbeddedRoleTypes.ObjectTypeAssignedPluralName);
        this.pluralName = this.EmbeddedPopulation.EmbeddedGetUnitRole<string>(this, metaPopulation.EmbeddedRoleTypes.ObjectTypePluralName);

        this.MetaPopulation.OnCreated(this);
    }

    public dynamic Attributes { get; }

    MetaPopulation IMetaIdentifiableObject.MetaPopulation => this.MetaPopulation;

    public MetaPopulation MetaPopulation { get; }

    public Guid Id { get; set; }

    public string Tag { get; set; }

    public Type BoundType { get; set; }

    public string SingularName { get => this.singularName.Value; set => this.singularName.Value = value; }

    public string AssignedPluralName { get => this.assignedPluralName.Value; set => this.assignedPluralName.Value = value; }

    public string PluralName { get => this.pluralName.Value; set => this.pluralName.Value = value; }

    public bool IsUnit => true;

    public bool IsComposite => false;

    public bool IsInterface => false;

    public bool IsClass => false;

    public static implicit operator Unit(IUnitIndex index) => index.Unit;

    public override bool Equals(object other) => this.Id.Equals((other as IMetaIdentifiableObject)?.Id);

    public override int GetHashCode() => this.Id.GetHashCode();

    public int CompareTo(IObjectType other)
    {
        return this.Id.CompareTo(other?.Id);
    }

    public override string ToString()
    {
        if (!string.IsNullOrEmpty(this.SingularName))
        {
            return this.SingularName;
        }

        return this.Tag;
    }

    public void Validate(ValidationLog validationLog)
    {
        this.ValidateObjectType(validationLog);
    }
    
    public bool IsBinary => this.Tag == UnitTags.Binary;

    public bool IsBoolean => this.Tag == UnitTags.Boolean;

    public bool IsDateTime => this.Tag == UnitTags.DateTime;

    public bool IsDecimal => this.Tag == UnitTags.Decimal;

    public bool IsFloat => this.Tag == UnitTags.Float;

    public bool IsInteger => this.Tag == UnitTags.Integer;

    public bool IsString => this.Tag == UnitTags.String;

    public bool IsUnique => this.Tag == UnitTags.Unique;

    public IEnumerable<string> WorkspaceNames => this.MetaPopulation.WorkspaceNames;
}