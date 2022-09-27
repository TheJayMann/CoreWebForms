// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Web.ModelBinding;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

public class ModelValidatorProviderCollection : Collection<ModelValidatorProvider>
{

    public ModelValidatorProviderCollection()
    {
    }

    public ModelValidatorProviderCollection(IList<ModelValidatorProvider> list)
        : base(list)
    {
    }

    protected override void InsertItem(int index, ModelValidatorProvider item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        base.InsertItem(index, item);
    }

    protected override void SetItem(int index, ModelValidatorProvider item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        base.SetItem(index, item);
    }

    public IEnumerable<ModelValidator> GetValidators(ModelMetadata metadata, ModelBindingExecutionContext context)
    {
        return this.SelectMany(provider => provider.GetValidators(metadata, context));
    }

}
