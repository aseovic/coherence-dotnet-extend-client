/*
 * Copyright (c) 2000, 2020, Oracle and/or its affiliates.
 *
 * Licensed under the Universal Permissive License v 1.0 as shown at
 * http://oss.oracle.com/licenses/upl.
 */
using System;

using Tangosol.Net.Cache;

namespace Tangosol.Util.Aggregator
{
    /// <summary>
    /// Sums up numeric values extracted from a collection of entries in a
    /// cache.
    /// </summary>
    /// <remarks>
    /// All the extracted objects (Byte, Int16, Int32, Int64, Single, Double)
    /// will be treated as <b>Int64</b> values. If the collection of
    /// entries is empty, a <c>null</c> result is returned.
    /// </remarks>
    /// <author>Gene Gleyzer  2005.09.05</author>
    /// <author>Ivan Cikic  2005.10.24</author>
    /// <since>Coherence 3.1</since>
    public class LongSum : AbstractLongAggregator
    {
        #region Constructors

        /// <summary>
        /// Default constructor.
        /// </summary>
        public LongSum()
        {}

        /// <summary>
        /// Construct a <b>LongSum</b> aggregator.
        /// </summary>
        /// <param name="extractor">
        /// The extractor that provides a value in the form of any .NET
        /// object out of Byte, Int16, Int32, Int64, Single, Double.
        /// </param>
        public LongSum(IValueExtractor extractor)
            : base(extractor)
        {}

        /// <summary>
        /// Construct a <b>LongSum</b> aggregator.
        /// </summary>
        /// <param name="member">
        /// The name of the member that returns a value in the form of any
        /// .NET object out of Byte, Int16, Int32, Int64, Single, Double.
        /// </param>
        public LongSum(string member)
            : base(member)
        {}

        #endregion

        #region AbstractAggregator override methods

        /// <summary>
        /// Initialize the aggregation result.
        /// </summary>
        /// <param name="isFinal">
        /// <b>true</b> is passed if the aggregation process that is being
        /// initialized must produce a final aggregation result; this will
        /// only be <b>false</b> if a parallel approach is being used and the
        /// initial (partial) aggregation process is being initialized.
        /// </param>
        protected override void Init(bool isFinal)
        {
            base.Init(isFinal);
            m_result = 0;
        }

        /// <summary>
        /// Incorporate one aggregatable value into the result.
        /// </summary>
        /// <remarks>
        /// If the <paramref name="isFinal"/> parameter is <b>true</b>, the
        /// given object is a partial result (returned by an individual
        /// parallel aggregator) that should be incorporated into the final
        /// result; otherwise, the object is a value extracted from an
        /// <see cref="IInvocableCacheEntry"/>.
        /// </remarks>
        /// <param name="o">
        /// The value to incorporate into the aggregated result.
        /// </param>
        /// <param name="isFinal">
        /// <b>true</b> to indicate that the given object is a partial
        /// result returned by a parallel aggregator.
        /// </param>
        protected override void Process(object o, bool isFinal)
        {
            if (o != null)
            {
                m_result += Convert.ToInt64(o);
                m_count++;
            }
        }

        #endregion
    }
}