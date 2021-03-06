﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DotVVM.Framework.Compilation.Javascript;
using DotVVM.Framework.Compilation.Javascript.Ast;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DotVVM.Framework.Tests.Common.Runtime.JavascriptCompilation
{
    [TestClass]
    public class JsParensInsersionTests
    {
        private void AssertFormatting(string expectedString, JsNode node)
        {
            Assert.AreEqual(expectedString, node.Clone().FormatScript());

            foreach (var dd in node.Descendants.OfType<JsExpression>()) {
                var symbol = new JsSymbolicParameter(new object());
                dd.ReplaceWith(symbol);
                var parametrized = node.Clone().FormatParametrizedScript();
                var resolved = parametrized.ToString(o =>
                    o == symbol.Symbol ? CodeParameterAssignment.FromExpression(dd) :
                    throw new Exception());
                Assert.AreEqual(expectedString, resolved, $"Replaced expression: {dd.FormatScript()}");
                symbol.ReplaceWith(dd);
            }
        }

        [TestMethod]
        public void JsParens_SimpleExpression()
        {
            AssertFormatting("a.b(4+b,5)", new JsIdentifierExpression("a").Member("b").Invoke(
                new JsBinaryExpression(new JsLiteral(4), BinaryOperatorType.Plus, new JsIdentifierExpression("b")),
                new JsLiteral(5)));
        }

        [TestMethod]
        public void JsParens_OperatorPriority()
        {
            AssertFormatting("a*b+c", new JsBinaryExpression(
                new JsBinaryExpression(new JsIdentifierExpression("a"), BinaryOperatorType.Times, new JsIdentifierExpression("b")),
                BinaryOperatorType.Plus,
                new JsIdentifierExpression("c")));
            AssertFormatting("(a+b)*c", new JsBinaryExpression(
                new JsBinaryExpression(new JsIdentifierExpression("a"), BinaryOperatorType.Plus, new JsIdentifierExpression("b")),
                BinaryOperatorType.Times,
                new JsIdentifierExpression("c")));
        }

        [TestMethod]
        public void JsParens_OperatorAsociativity()
        {
            AssertFormatting("a+b+c", new JsBinaryExpression(
                new JsBinaryExpression(new JsIdentifierExpression("a"), BinaryOperatorType.Plus, new JsIdentifierExpression("b")),
                BinaryOperatorType.Plus,
                new JsIdentifierExpression("c")));

            AssertFormatting("c+(a+b)", new JsBinaryExpression(
                new JsIdentifierExpression("c"),
                BinaryOperatorType.Plus,
                new JsBinaryExpression(new JsIdentifierExpression("a"), BinaryOperatorType.Plus, new JsIdentifierExpression("b"))));
        }

        [TestMethod]
        public void JsParens_AssignmentOperator()
        {
            AssertFormatting("a=b=1?2:3", new JsAssignmentExpression(
                new JsIdentifierExpression("a"),
                new JsAssignmentExpression(
                    new JsIdentifierExpression("b"),
                    new JsConditionalExpression(new JsLiteral(1), new JsLiteral(2), new JsLiteral(3)))));
        }

        [TestMethod]
        public void JsParens_MemberAccessOnOperator()
        {
            AssertFormatting("(1+1).a", new JsBinaryExpression(new JsLiteral(1), BinaryOperatorType.Plus, new JsLiteral(1))
                .Member("a"));
        }
    }
}
