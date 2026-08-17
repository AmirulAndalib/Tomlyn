// Copyright (c) Alexandre Mutel. All rights reserved.
// Licensed under the BSD-Clause 2 license.
// See license.txt file in the project root for full license information.

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Tomlyn.Model;
using Tomlyn.Parsing;

namespace Tomlyn.Tests
{
    public class BasicTests
    {
        [Test]
        public void TestTableArraysContainingPrimitiveArraysSerialize()
        {
            var test = @"[[table_array]]
primitive_list = [4, 5, 6]
";

            var model = TomlSerializer.Deserialize<TomlTable>(test);
            Assert.That(model, Is.Not.Null);
            var tomlOut = TomlSerializer.Serialize(model!);

            Assert.AreEqual(test.ReplaceLineEndings("\n"), tomlOut.ReplaceLineEndings("\n"));
        }

        [Test]
        public void TestHelloWorld()
        {
            var toml = @"global = ""this is a string""
# This is a comment of a table
[my_table]
key = 1 # Comment a key
value = true
 list = [4, 5, 6]
";

            var model = TomlSerializer.Deserialize<TomlTable>(toml);
            Assert.That(model, Is.Not.Null);
            var nonNullModel = model!;
            // Prints "this is a string"
            var global = nonNullModel["global"];
            Console.WriteLine($"found global = \"{global}\"");
            Assert.AreEqual("this is a string", global);
            // Prints 1
            var key = ((TomlTable)nonNullModel["my_table"]!)["key"];
            Console.WriteLine($"found key = {key}");
            Assert.AreEqual(1L, key);
            // Check list
            var list = (TomlArray)((TomlTable)nonNullModel["my_table"]!)["list"]!;
            Console.WriteLine($"found list = {string.Join(", ", list)}");
            Assert.AreEqual(new TomlArray() { 4, 5, 6 }, list);
        }

        [Test]
        [TestCase(7, 32, 0, 0)]
        [TestCase(7, 32, 0, 999)]
        [TestCase(0, 32, 0, 0)]
        public void TestLocalTime(int hour, int minute, int second, int millisecond)
        {
            var toml = $@"time = {hour:D2}:{minute:D2}:{second:D2}.{millisecond:D3}";
            var model = TomlSerializer.Deserialize<TomlTable>(toml);
            Assert.That(model, Is.Not.Null);
            var localTime = (TomlDateTime)model!["time"];

            Assert.AreEqual(hour, localTime.DateTime.Hour);
            Assert.AreEqual(minute, localTime.DateTime.Minute);
            Assert.AreEqual(second, localTime.DateTime.Second);
            Assert.AreEqual(millisecond, localTime.DateTime.Millisecond);
        }

        [Test]
        public void TestEmptyComment()
        {
            var input = "#\n";
            var doc = SyntaxParser.Parse(input);
            var docAsStr = doc.ToString();
            Assert.AreEqual(input, docAsStr);
        }

        [Test]
        public void TestIntegerOverflowIsRejected()
        {
            // TOML requires an error when an integer cannot be represented as a
            // signed 64-bit value; a positive literal in [2^63, 2^64-1] must not
            // silently wrap to a negative long.
            Assert.IsFalse(SyntaxParser.Parse("a = 9223372036854775807").HasErrors);  // long.MaxValue
            Assert.IsFalse(SyntaxParser.Parse("a = -9223372036854775808").HasErrors); // long.MinValue
            Assert.IsTrue(SyntaxParser.Parse("a = 9223372036854775808").HasErrors);   // 2^63
            Assert.IsTrue(SyntaxParser.Parse("a = 18446744073709551615").HasErrors);  // 2^64-1
        }

        [Test]
        public void SimpleTest()
        {
            var test = @"[table-1]
key1 = ""some string""    # This is a comment
key2 = 123
Key3 = true
Key4 = false
Key5 = +inf

[table-2]
key1 = ""another string""
key2 = 456
";
            var doc = SyntaxParser.Parse(test);
            Assert.AreEqual(test, doc.ToString());
        }

        [Test]
        public void TestInlineArray()
        {
            var input = @"x = [1,
2,
3
]
";
            var model = TomlSerializer.Deserialize<TomlTable>(input);
            Assert.That(model, Is.Not.Null);
            var array = model!["x"] as TomlArray;
            Assert.NotNull(array);
            var nonNullArray = array!;
            Assert.AreEqual(3, nonNullArray.Count);
            Assert.AreEqual(1L, nonNullArray[0]);
            Assert.AreEqual(2L, nonNullArray[1]);
            Assert.AreEqual(3L, nonNullArray[2]);
        }
    }
}
