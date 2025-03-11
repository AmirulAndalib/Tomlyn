// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license. 
// See license.txt file in the project root for full license information.

using System;
using NUnit.Framework;
using Tomlyn.Model;

namespace Tomlyn.Tests;

public class SerializationTests
{
    [Test]
    public void TestCrlfInMultilineString()
    {
        var model = new TomlTable
        {
            PropertiesMetadata = new TomlPropertiesMetadata()
        };
        model.PropertiesMetadata.SetProperty("property", new TomlPropertyMetadata
        {
            DisplayKind = TomlPropertyDisplayKind.StringLiteralMulti
        });

        model["property"] = "string\r\nwith\r\nnewlines";

        var result = Toml.FromModel(model).Trim();
        AssertHelper.AreEqualNormalizeNewLine("property = '''string\r\nwith\r\nnewlines'''", result);
    }

    [Test]
    public void TestArrayWithPrimitives()
    {
        var model = new TomlTable()
        {
            ["mixed-array"] = new TomlArray()
            {
                new TomlTable() { ["a"] = 1 },
                2,  // If instead the second or final item in the array was the table, it works as expected.
                3
            }
        };

        var result = Toml.FromModel(model).ReplaceLineEndings("\n").Trim();
        AssertHelper.AreEqualNormalizeNewLine("mixed-array = [{a = 1}, 2, 3]", result);
    }

    [Test]
    public void TestNestedEmptyArrays()
    {
        var outer = new TomlTable
        {
            ["inner1"] = new TomlTable { },
            ["inner2"] = new TomlTable {
                { "array1", new TomlArray {} },
                { "array2", new TomlArray {} }
            },
            ["inner3"] = new TomlTable {
                { "array1", new TomlArray {} },
                { "array2", new TomlArray {} },
                { "array3", new TomlArray { "hello" } },
                { "array4", new TomlArray { } },
                { "array5", new TomlArray { } },
                { "array6", new TomlArray { } }
            },
            ["inner4"] = new TomlTable {
                { "array1", new TomlArray {} },
                { "string", "value" },
                { "array2", new TomlArray {} },
                { "array3", new TomlArray {} },
            },
        };

        var expecting = """
                        [inner1]
                        [inner2]
                        array1 = []
                        array2 = []
                        [inner3]
                        array1 = []
                        array2 = []
                        array3 = ["hello"]
                        array4 = []
                        array5 = []
                        array6 = []
                        [inner4]
                        array1 = []
                        string = "value"
                        array2 = []
                        array3 = []
                        """.ReplaceLineEndings("\n");
        
        var result = Toml.FromModel(outer).ReplaceLineEndings("\n").Trim();
        AssertHelper.AreEqualNormalizeNewLine(expecting, result);
    }
}