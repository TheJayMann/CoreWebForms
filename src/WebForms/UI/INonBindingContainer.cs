//------------------------------------------------------------------------------
// <copyright file="INonBindingContainer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

/*
 * Marker interface implemented by all controls that wish to introduce a new
 * logical namespace into the control hierarchy tree.
 * 
 * Copyright (c) 2000 Microsoft Corporation
 */
namespace System.Web.UI
{
    /// <devdoc>
    ///    <para>Identifies 
    ///       a container control that scopes a new ID namespace within a page's control
    ///       hierarchy. This is a marker interface only.</para>
    /// </devdoc>
    internal interface INonBindingContainer : INamingContainer {
    }
}
