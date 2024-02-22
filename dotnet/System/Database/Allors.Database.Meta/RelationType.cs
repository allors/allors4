﻿// <copyright file="RelationType.cs" company="Allors bv">
// Copyright (c) Allors bv. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>
// <summary>Defines the RelationType type.</summary>

namespace Allors.Database.Meta;

using System;
using System.Collections.Generic;
using System.Linq;
using Allors.Text;

/// <summary>
///     A <see cref="RelationType" /> defines the state and behavior for
///     a set of <see cref="AssociationType" />s and <see cref="RoleType" />s.
/// </summary>
public sealed class RelationType : IMetaIdentifiableObject, IComparable
{
    private string[] derivedWorkspaceNames;

    private AssociationType associationType;
    private RoleType roleType;

    public RelationType(MetaPopulation metaPopulation, Guid id, Multiplicity? assignedMultiplicity, bool isDerived, AssociationType associationType, RoleType roleType)
    {
        this.Attributes = new MetaExtension();
        this.MetaPopulation = metaPopulation;
        this.Id = id;
        this.Tag = id.Tag();

        this.IsDerived = isDerived;
        this.Multiplicity = roleType.ObjectType.IsUnit ? Multiplicity.OneToOne : assignedMultiplicity ?? Multiplicity.ManyToOne;
        this.AssignedWorkspaceNames = Array.Empty<string>();

        this.associationType = associationType;
        this.associationType.RelationType = this;

        this.roleType = roleType;
        this.roleType.RelationType = this;
        this.roleType.SingularName = this.roleType.AssignedSingularName ?? this.roleType.ObjectType.SingularName;

        this.roleType.CompositeRoleType = new CompositeRoleType(this.associationType.Composite, this.roleType);

        this.MetaPopulation.OnCreated(this);
    }

    public dynamic Attributes { get; }

    MetaPopulation IMetaIdentifiableObject.MetaPopulation => this.MetaPopulation;

    public MetaPopulation MetaPopulation { get; }

    public Guid Id { get; }

    public string Tag { get; set; }

    public IReadOnlyList<string> AssignedWorkspaceNames { get; set; }

    public Multiplicity Multiplicity { get; }

    public AssociationType AssociationType { get => this.associationType; set => this.associationType = value; }

    public RoleType RoleType { get => this.roleType; set => this.roleType = value; }

    public string Name => this.associationType.ObjectType + this.roleType.SingularName;

    private string ReverseName => this.roleType.SingularName + this.associationType.ObjectType;

    public IEnumerable<string> WorkspaceNames
    {
        get
        {
            return this.derivedWorkspaceNames;
        }
    }

    public bool IsDerived { get; }

    public bool IsKey { get; set; }

    // TODO: Derive
    public bool ExistExclusiveClasses
    {
        get
        {
            if (this.associationType?.ObjectType != null && this.roleType?.ObjectType != null)
            {
                return this.associationType.Composite.ExclusiveClass != null && this.roleType.ObjectType is Composite roleCompositeType && roleCompositeType.ExclusiveClass != null;
            }

            return false;
        }
    }

    private string ValidationName => "relation type" + this.Name;

    public int CompareTo(object other) => this.Id.CompareTo((other as RelationType)?.Id);

    public override bool Equals(object other) => this.Id.Equals((other as RelationType)?.Id);

    public override int GetHashCode() => this.Id.GetHashCode();

    public override string ToString()
    {
        try
        {
            return this.Name;
        }
        catch
        {
            return this.Tag;
        }
    }

    public void DeriveWorkspaceNames() =>
        this.derivedWorkspaceNames = this.AssignedWorkspaceNames != null
            ? this.AssignedWorkspaceNames
                .Intersect(this.associationType.Composite.Classes.SelectMany(v => v.WorkspaceNames))
                .Intersect(this.roleType.ObjectType switch
                {
                    Unit unit => unit.WorkspaceNames,
                    Interface @interface => @interface.Classes.SelectMany(v => v.WorkspaceNames),
                    Class @class => @class.WorkspaceNames,
                    _ => Array.Empty<string>(),
                })
                .ToArray()
            : Array.Empty<string>();

    public void Validate(ValidationLog validationLog)
    {
        this.ValidateIdentity(validationLog);

        if (this.associationType != null && this.roleType != null)
        {
            if (validationLog.ExistRelationName(this.Name))
            {
                var message = "name of " + this.ValidationName + " is already in use";
                validationLog.AddError(message, this, ValidationKind.Unique, "RelationType.Name");
            }
            else
            {
                validationLog.AddRelationTypeName(this.Name);
            }

            if (validationLog.ExistRelationName(this.ReverseName))
            {
                var message = "reversed name of " + this.ValidationName + " is already in use";
                validationLog.AddError(message, this, ValidationKind.Unique, "RelationType.Name");
            }
            else
            {
                validationLog.AddRelationTypeName(this.ReverseName);
            }

            if (validationLog.ExistObjectTypeName(this.Name))
            {
                var message = "name of " + this.ValidationName + " is in conflict with object type " + this.Name;
                validationLog.AddError(message, this, ValidationKind.Unique, "RelationType.Name");
            }

            if (validationLog.ExistObjectTypeName(this.ReverseName))
            {
                var message = "reversed name of " + this.ValidationName + " is in conflict with object type " + this.Name;
                validationLog.AddError(message, this, ValidationKind.Unique, "RelationType.Name");
            }
        }
        else if (this.associationType == null)
        {
            var message = this.ValidationName + " has no association type";
            validationLog.AddError(message, this, ValidationKind.Required, "RelationType.AssociationType");
        }
        else
        {
            var message = this.ValidationName + " has no role type";
            validationLog.AddError(message, this, ValidationKind.Required, "RelationType.RoleType");
        }

        this.associationType?.Validate(validationLog);

        this.roleType?.Validate(validationLog);
    }
}