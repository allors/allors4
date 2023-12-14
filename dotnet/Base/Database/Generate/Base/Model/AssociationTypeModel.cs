﻿namespace Allors.Meta.Generation.Model;

using Allors.Database.Meta;

public class AssociationTypeModel : RelationEndTypeModel
{
    public AssociationTypeModel(Model model, IAssociationType associationType)
        : base(model) =>
        this.AssociationType = associationType;

    public IAssociationType AssociationType { get; }

    protected override IRelationEndType RelationEndType => this.AssociationType;

    // IAssociationType
    public RelationTypeModel RelationType => this.Model.Map(this.AssociationType.RelationType);

    public RoleTypeModel RoleType => this.Model.Map(this.AssociationType.RoleType);
}