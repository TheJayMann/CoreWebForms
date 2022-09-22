// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Web.Util;

internal class HttpUtility2
{
    internal static bool TryParseCoordinates(string value, out double doubleValue)
    {
        var flags = NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign;
        return Double.TryParse(value, flags, CultureInfo.InvariantCulture, out doubleValue);
    }
}