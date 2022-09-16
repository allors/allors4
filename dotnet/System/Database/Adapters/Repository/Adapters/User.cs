// <copyright file="User.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository;

using Attributes;

#region Allors
[Id("0d6bc154-112b-4a58-aa96-3b2a96f82523")]
#endregion
public class User : object
{
    #region Allors
    [Id("1ffa3cb7-41f0-406a-a3a5-2f3a4c5ad59c")]
    [Indexed]
    #endregion
    public User[] Selects { get; set; }

    #region Allors
    [Id("bc6b71a8-2a66-4b57-9c86-ecf521b973ba")]
    [Size(256)]
    #endregion
    public string From { get; set; }

    #region inherited properties
    #endregion

    #region inherited methods
    #endregion
}