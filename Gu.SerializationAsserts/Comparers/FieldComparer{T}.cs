﻿namespace Gu.SerializationAsserts
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// A deep equals checking nested fields.
    /// Handles collections and reference loops.
    /// </summary>
    /// <typeparam name="T">The type of instances to check</typeparam>
    public class FieldComparer<T> : IEqualityComparer<T>, IComparer
    {
        /// <summary>The default instance.</summary>
        public static readonly FieldComparer<T> Default = new FieldComparer<T>();

        private FieldComparer()
        {
        }

        /// <inheritdoc/>
        public bool Equals(T x, T y)
        {
            var comparison = DeepEqualsNode.CreateFor(x, y);
            return comparison.Matches();
        }

        /// <summary>
        /// nUnit uses IComparer for CollectionAssert
        /// Note: this is not a comparer that makes sense for sorting.
        /// </summary>
        /// <returns>
        /// 0 if <paramref name="x"/> and <paramref name="y"/> are equal.
        /// -1 if not equal.
        /// </returns>
        int IComparer.Compare(object x, object y)
        {
            var comparison = DeepEqualsNode.CreateFor(x, y);
            if (comparison.Matches())
            {
                return 0;
            }

            return -1;
        }

        /// <inheritdoc/>
        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            throw new System.NotImplementedException();
        }
    }
}
