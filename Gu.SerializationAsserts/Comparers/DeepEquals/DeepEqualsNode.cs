﻿namespace Gu.SerializationAsserts
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    [DebuggerDisplay("Type: {Type} Field: {ParentField}}")]
    internal class DeepEqualsNode
    {
        private DeepEqualsNode(ICompared expected, ICompared actual)
        {
            this.Expected = expected;
            this.Actual = actual;
            var expectedType = this.Expected.Value?.GetType();
            var actualType = this.Actual.Value?.GetType();
            this.Type = expectedType == actualType ? expectedType : null;
            this.Fields = this.Type?.GetFields(BindingFlags.Instance |
                                               BindingFlags.Public |
                                               BindingFlags.NonPublic |
                                               BindingFlags.FlattenHierarchy)
                                     ?? new FieldInfo[0];
        }

        private DeepEqualsNode(
            DeepEqualsNode parent,
            FieldInfo parentField,
            ICompared expected,
            ICompared actual)
            : this(expected, actual)
        {
            this.Parent = parent;
            this.ParentField = parentField;
        }

        internal ICompared Expected { get; }

        internal ICompared Actual { get; }

        internal Type Type { get; }

        internal DeepEqualsNode Parent { get; }

        internal FieldInfo ParentField { get; }

        internal IReadOnlyList<FieldInfo> Fields { get; }

        internal static DeepEqualsNode CreateFor(object expected, object actual)
        {
            var ec = new ComparedField(expected, null);
            var ac = new ComparedField(actual, null);
            return new DeepEqualsNode(ec, ac);
        }

        internal bool Matches()
        {
            if (this.Expected.Value == null && this.Actual.Value == null)
            {
                return true;
            }

            if (this.Expected.Value == null || this.Actual.Value == null)
            {
                return false;
            }

            if (this.Expected is ComparedIEnumerable && this.Actual is ComparedIEnumerable)
            {
                return false;
            }

            if (this.Type.IsEquatable())
            {
                return object.Equals(this.Expected.Value, this.Actual.Value);
            }

            foreach (var child in this.GetChildren())
            {
                if (!child.Matches())
                {
                    return false;
                }
            }

            return true;
        }

        public IEnumerable<DeepEqualsNode> AllChildren()
        {
            foreach (var child in this.GetChildren())
            {
                yield return child;
                foreach (var nested in child.AllChildren())
                {
                    yield return nested;
                }
            }
        }

        internal IEnumerable<DeepEqualsNode> GetChildren()
        {
            var expected = this.Expected.Value;
            var actual = this.Actual.Value;
            if (expected == null || actual == null)
            {
                yield break;
            }

            if (this.Type != null && this.Type.IsEquatable())
            {
                yield break;
            }

            if (this.Expected is ComparedIEnumerable && this.Actual is ComparedIEnumerable)
            {
                yield break;
            }

            if (IsAnyIEnumerable(expected, actual))
            {
                if (!IsBothIEnumerable(expected, actual))
                {
                    throw new InvalidOperationException("Derp");
                }

                var expectedChildren = ((IEnumerable)expected).OfType<object>().ToArray();
                var actualChildren = ((IEnumerable)actual).OfType<object>().ToArray();
                if (expectedChildren.Length != actualChildren.Length)
                {
                    yield return this.CreateIEnumerableChild(expectedChildren, actualChildren, this.ParentField);
                }
                else
                {
                    for (int i = 0; i < Math.Max(expectedChildren.Length, actualChildren.Length); i++)
                    {
                        var expectedChild = expectedChildren.ElementAtOrDefault(i);
                        var actualChild = actualChildren.ElementAtOrDefault(i);

                        yield return this.CreateIndexChild(expectedChild, actualChild, this.ParentField, i);
                    }
                }
            }

            foreach (var fieldInfo in this.Fields)
            {
                var expectedChild = fieldInfo.GetValue(expected);
                var actualChild = fieldInfo.GetValue(actual);
                if (this.IsCircular(expectedChild, actualChild))
                {
                    continue;
                }

                yield return this.CreateFieldChild(expectedChild, actualChild, fieldInfo);
            }
        }

        private bool IsCircular(object expectedChild, object actualChild)
        {
            var parent = this;
            while (parent != null)
            {
                if (ReferenceEquals(parent.Expected.Value, expectedChild) && ReferenceEquals(parent.Actual.Value, actualChild))
                {
                    return true;
                }

                if (ReferenceEquals(parent.Expected.Value, expectedChild) || ReferenceEquals(parent.Actual.Value, actualChild))
                {
                    throw new NotSupportedException();
                }

                parent = parent.Parent;
            }

            return false;
        }

        private static bool IsAnyIEnumerable(object a, object b)
        {
            return a is IEnumerable || b is IEnumerable;
        }

        private static bool IsBothIEnumerable(object a, object b)
        {
            return a is IEnumerable && b is IEnumerable;
        }

        private DeepEqualsNode CreateIEnumerableChild(object[] expected, object[] actual, FieldInfo field)
        {
            var ecf = new ComparedIEnumerable(expected, field);
            var acf = new ComparedIEnumerable(actual, field);
            return new DeepEqualsNode(this, field, ecf, acf);
        }

        private DeepEqualsNode CreateFieldChild(object expected, object actual, FieldInfo field)
        {
            var ecf = new ComparedField(expected, field);
            var acf = new ComparedField(actual, field);
            return new DeepEqualsNode(this, field, ecf, acf);
        }

        private DeepEqualsNode CreateIndexChild(object expected, object actual, FieldInfo field, int index)
        {
            var ecf = new ComparedItem(expected, index);
            var acf = new ComparedItem(actual, index);
            return new DeepEqualsNode(this, field, ecf, acf);
        }

        // Using new here to hide it so it not called by mistake
        private new static void Equals(object x, object y)
        {
            throw new NotSupportedException($"{x}, {y}");
        }
    }
}