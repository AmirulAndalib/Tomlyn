// Copyright (c) 2019 - Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license. 
// See license.txt file in the project root for full license information.

namespace SharpToml.Syntax
{
    /// <summary>
    /// Defines the kind for a <see cref="SyntaxNode"/>
    /// </summary>
    public enum SyntaxKind
    {
        Array,

        ArrayItem,

        BasicKey,

        Boolean,

        DateTime,

        Document,

        DottedKeyItem,

        Float,

        InlineTable,

        Integer,

        Key,

        KeyValue,

        List,


        String,

        Table,

        TableArray,

        Token,
    }
}