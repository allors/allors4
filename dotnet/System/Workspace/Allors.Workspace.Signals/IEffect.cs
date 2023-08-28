﻿// <copyright file="Object.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace
{
    using System;

    public interface IEffect : IDisposable
    {
        Object Context { get; }
    }

    public interface IEffect<out T> : IEffect
    {
        new T Context { get; }
    }
}