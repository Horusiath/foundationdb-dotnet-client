﻿#region BSD Licence
/* Copyright (c) 2013-2018, Doxense SAS
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:
	* Redistributions of source code must retain the above copyright
	  notice, this list of conditions and the following disclaimer.
	* Redistributions in binary form must reproduce the above copyright
	  notice, this list of conditions and the following disclaimer in the
	  documentation and/or other materials provided with the distribution.
	* Neither the name of Doxense nor the
	  names of its contributors may be used to endorse or promote products
	  derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND
ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL <COPYRIGHT HOLDER> BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */
#endregion

//#define ENABLE_VALUETUPLES

namespace Doxense.Collections.Tuples
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using Doxense.Diagnostics.Contracts;
	using JetBrains.Annotations;

	/// <summary>Add extensions methods that deal with tuples on various types</summary>
	public static class TupleExtensions
	{

		#region ITuple extensions...

		/// <summary>Returns true if the tuple is either null or empty</summary>
		[ContractAnnotation("null => true")]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsNullOrEmpty(this ITuple tuple)
		{
			return tuple == null || tuple.Count == 0;
		}

		/// <summary>Returns true if the tuple is not null, and contains only one item</summary>
		[ContractAnnotation("null => false")]
		public static bool IsSingleton(this ITuple tuple)
		{
			return tuple != null && tuple.Count == 1;
		}

		/// <summary>Returns an array containing all the objects of a tuple</summary>
		[NotNull, ItemCanBeNull]
		public static object[] ToArray([NotNull] this ITuple tuple)
		{
			Contract.NotNull(tuple, nameof(tuple));

			var items = new object[tuple.Count];
			if (items.Length > 0)
			{
				tuple.CopyTo(items, 0);
			}
			return items;
		}

		/// <summary>Returns a typed array containing all the items of a tuple</summary>
		[NotNull]
		public static T[] ToArray<T>([NotNull] this ITuple tuple)
		{
			Contract.NotNull(tuple, nameof(tuple));

			var items = new T[tuple.Count];
			if (items.Length > 0)
			{
				for (int i = 0; i < items.Length; i++)
				{
					items[i] = tuple.Get<T>(i);
				}
			}
			return items;
		}

		/// <summary>Returns the typed value of the first item in this tuple</summary>
		/// <typeparam name="T">Expected type of the first item</typeparam>
		/// <returns>Value of the first item, adapted into type <typeparamref name="T"/>.</returns>
		[Pure]
		[ContractAnnotation("null => true")]
		public static T First<T>([NotNull] this ITuple tuple)
		{
			return tuple.Get<T>(0);
		}

		/// <summary>Return the typed value of the last item in the tuple</summary>
		/// <typeparam name="T">Expected type of the item</typeparam>
		/// <returns>Value of the last item of this tuple, adapted into type <typeparamref name="T"/></returns>
		/// <remarks>Equivalent of tuple.Get&lt;T&gt;(-1)</remarks>
		[Pure]
		[ContractAnnotation("null => true")]
		public static T Last<T>([NotNull] this ITuple tuple)
		{
			return tuple.Get<T>(-1);
		}

		/// <summary>Appends two values at the end of a tuple</summary>
		[NotNull]
		public static ITuple Append<T1, T2>([NotNull] this ITuple tuple, T1 value1, T2 value2)
		{
			Contract.NotNull(tuple, nameof(tuple));
			return new JoinedTuple(tuple, STuple.Create(value1, value2));
		}

		/// <summary>Appends three values at the end of a tuple</summary>
		[NotNull]
		public static ITuple Append<T1, T2, T3>([NotNull] this ITuple tuple, T1 value1, T2 value2, T3 value3)
		{
			Contract.NotNull(tuple, nameof(tuple));
			return new JoinedTuple(tuple, STuple.Create<T1, T2, T3>(value1, value2, value3));
		}

		/// <summary>Appends four values at the end of a tuple</summary>
		[NotNull]
		public static ITuple Append<T1, T2, T3, T4>([NotNull] this ITuple tuple, T1 value1, T2 value2, T3 value3, T4 value4)
		{
			Contract.NotNull(tuple, nameof(tuple));
			return new JoinedTuple(tuple, STuple.Create<T1, T2, T3, T4>(value1, value2, value3, value4));
		}

		/// <summary>Returns a substring of the current tuple</summary>
		/// <param name="tuple">Current tuple</param>
		/// <param name="offset">Offset from the start of the current tuple (negative value means from the end)</param>
		/// <returns>Tuple that contains only the items past the first <param name="offset"/> items of the current tuple</returns>
		[NotNull]
		public static ITuple Substring([NotNull] this ITuple tuple, int offset)
		{
			Contract.NotNull(tuple, nameof(tuple));

			return tuple[offset, null];
		}

		/// <summary>Returns a substring of the current tuple</summary>
		/// <param name="tuple">Current tuple</param>
		/// <param name="offset">Offset from the start of the current tuple (negative value means from the end)</param>
		/// <param name="count">Number of items to keep</param>
		/// <returns>Tuple that contains only the selected items from the current tuple</returns>
		[NotNull]
		public static ITuple Substring([NotNull] this ITuple tuple, int offset, int count)
		{
			Contract.NotNull(tuple, nameof(tuple));
			Contract.Positive(count, nameof(count));

			if (count == 0) return STuple.Empty;

			return tuple[offset, offset + count];
		}

		/// <summary>Returns a tuple with only the first (or last) items of this tuple</summary>
		/// <param name="tuple">Tuple to truncate</param>
		/// <param name="count">Number of items to keep. If positive, items will be taken from the start of the tuple. If negative, items will be taken from the end of the tuple</param>
		/// <returns>New tuple of size |<paramref name="count"/>|.</returns>
		/// <example>
		/// (a, b, c).Truncate(2) => (a, b)
		/// (a, b, c).Truncate(-2) => (b, c)
		/// </example>
		public static ITuple Truncate([NotNull] this ITuple tuple, int count)
		{
			tuple.OfSizeAtLeast(Math.Abs(count));

			if (count < 0)
			{
				int offset = tuple.Count + count;
				return Substring(tuple, offset, -count);
			}
			else
			{
				return Substring(tuple, 0, count);
			}
		}

		/// <summary>Test if the start of current tuple is equal to another tuple</summary>
		/// <param name="left">Larger tuple</param>
		/// <param name="right">Smaller tuple</param>
		/// <returns>True if the beginning of <paramref name="left"/> is equal to <paramref name="right"/> or if both tuples are identical</returns>
		public static bool StartsWith([NotNull] this ITuple left, [NotNull] ITuple right)
		{
			Contract.NotNull(left, nameof(left));
			Contract.NotNull(right, nameof(right));

			//REVIEW: move this on ITuple interface ?
			return TupleHelpers.StartsWith(left, right);
		}

		/// <summary>Test if the end of current tuple is equal to another tuple</summary>
		/// <param name="left">Larger tuple</param>
		/// <param name="right">Smaller tuple</param>
		/// <returns>True if the end of <paramref name="left"/> is equal to <paramref name="right"/> or if both tuples are identical</returns>
		public static bool EndsWith([NotNull] this ITuple left, [NotNull] ITuple right)
		{
			Contract.NotNull(left, nameof(left));
			Contract.NotNull(right, nameof(right));

			//REVIEW: move this on ITuple interface ?
			return TupleHelpers.EndsWith(left, right);
		}

		/// <summary>Transform a tuple of N elements into a list of N singletons</summary>
		/// <param name="tuple">Tuple that contains any number of elements</param>
		/// <returns>Sequence of tuples that contains a single element</returns>
		/// <example>(123, ABC, false,).Explode() => [ (123,), (ABC,), (false,) ]</example>
		public static IEnumerable<ITuple> Explode([NotNull] this ITuple tuple)
		{
			Contract.NotNull(tuple, nameof(tuple));

			int p = 0;
			int n = tuple.Count;
			while (p < n)
			{
				yield return tuple[p, p + 1];
				++p;
			}
		}

		/// <summary>Verify that this tuple has the expected size</summary>
		/// <param name="tuple">Tuple which must be of a specific size</param>
		/// <param name="size">Expected number of items in this tuple</param>
		/// <returns>The <paramref name="tuple"/> itself it it has the correct size; otherwise, an exception is thrown</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="tuple"/> is null</exception>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> is smaller or larger than <paramref name="size"/></exception>
		[ContractAnnotation("halt <= tuple:null")]
		[NotNull]
		public static ITuple OfSize(this ITuple tuple, int size)
		{
			if (tuple == null || tuple.Count != size) ThrowInvalidTupleSize(tuple, size, 0);
			return tuple;
		}

		/// <summary>Verify that this tuple has at least a certain size</summary>
		/// <param name="tuple">Tuple which must be of a specific size</param>
		/// <param name="size">Expected minimum number of items in this tuple</param>
		/// <returns>The <paramref name="tuple"/> itself it it has the correct size; otherwise, an exception is thrown</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="tuple"/> is null</exception>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> is smaller than <paramref name="size"/></exception>
		[ContractAnnotation("halt <= tuple:null")]
		[NotNull]
		public static ITuple OfSizeAtLeast(this ITuple tuple, int size)
		{
			if (tuple == null || tuple.Count < size) ThrowInvalidTupleSize(tuple, size, -1);
			return tuple;
		}

		/// <summary>Verify that this tuple has at most a certain size</summary>
		/// <param name="tuple">Tuple which must be of a specific size</param>
		/// <param name="size">Expected maximum number of items in this tuple</param>
		/// <returns>The <paramref name="tuple"/> itself it it has the correct size; otherwise, an exception is thrown</returns>
		/// <exception cref="ArgumentNullException">If <paramref name="tuple"/> is null</exception>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> is larger than <paramref name="size"/></exception>
		[ContractAnnotation("halt <= tuple:null")]
		[NotNull]
		public static ITuple OfSizeAtMost(this ITuple tuple, int size)
		{
			if (tuple == null || tuple.Count > size) ThrowInvalidTupleSize(tuple, size, 1);
			return tuple;
		}

		[ContractAnnotation("=> halt")]
		internal static void ThrowInvalidTupleSize(ITuple tuple, int expected, int test)
		{
			Contract.NotNull(tuple, nameof(tuple));
			switch(test)
			{
				case 1: throw new InvalidOperationException($"This operation requires a tuple of size {expected} or less, but this tuple has {tuple.Count} elements");
				case -1: throw new InvalidOperationException($"This operation requires a tuple of size {expected} or more, but this tuple has {tuple.Count} elements");
				default: throw new InvalidOperationException($"This operation requires a tuple of size {expected}, but this tuple has {tuple.Count} elements");
			}
		}

		/// <summary>Creates pre-packed and isolated copy of this tuple</summary>
		/// <param name="tuple"></param>
		/// <returns>Create a copy of the tuple that can be reused frequently to pack values</returns>
		/// <remarks>If the tuple is already memoized, the current instance will be returned</remarks>
		[CanBeNull, ContractAnnotation("null => null")]
		public static MemoizedTuple Memoize(this ITuple tuple)
		{
			if (tuple == null) return null;

			var memoized = tuple as MemoizedTuple ?? new MemoizedTuple(tuple.ToArray(), TuPack.Pack(tuple));

			return memoized;
		}

		/// <summary>Returns a typed version of a tuple of size 1</summary>
		/// <typeparam name="T1">Expected type of the single element</typeparam>
		/// <param name="tuple">Tuple that must be of size 1</param>
		/// <returns>Equivalent tuple, with its element converted to the specified type</returns>
		public static STuple<T1> As<T1>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(1);
			return new STuple<T1>(tuple.Get<T1>(0));
		}

		/// <summary>Returns a typed version of a tuple of size 2</summary>
		/// <typeparam name="T1">Expected type of the first element</typeparam>
		/// <typeparam name="T2">Expected type of the second element</typeparam>
		/// <param name="tuple">Tuple that must be of size 2</param>
		/// <returns>Equivalent tuple, with its elements converted to the specified types</returns>
		public static STuple<T1, T2> As<T1, T2>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(2);
			return new STuple<T1, T2>(
				tuple.Get<T1>(0),
				tuple.Get<T2>(1)
			);
		}

		/// <summary>Returns a typed version of a tuple of size 3</summary>
		/// <typeparam name="T1">Expected type of the first element</typeparam>
		/// <typeparam name="T2">Expected type of the second element</typeparam>
		/// <typeparam name="T3">Expected type of the third element</typeparam>
		/// <param name="tuple">Tuple that must be of size 3</param>
		/// <returns>Equivalent tuple, with its elements converted to the specified types</returns>
		public static STuple<T1, T2, T3> As<T1, T2, T3>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(3);
			return new STuple<T1, T2, T3>(
				tuple.Get<T1>(0),
				tuple.Get<T2>(1),
				tuple.Get<T3>(2)
			);
		}

		/// <summary>Returns a typed version of a tuple of size 4</summary>
		/// <typeparam name="T1">Expected type of the first element</typeparam>
		/// <typeparam name="T2">Expected type of the second element</typeparam>
		/// <typeparam name="T3">Expected type of the third element</typeparam>
		/// <typeparam name="T4">Expected type of the fourth element</typeparam>
		/// <param name="tuple">Tuple that must be of size 4</param>
		/// <returns>Equivalent tuple, with its elements converted to the specified types</returns>
		public static STuple<T1, T2, T3, T4> As<T1, T2, T3, T4>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(4);
			return new STuple<T1, T2, T3, T4>(
				tuple.Get<T1>(0),
				tuple.Get<T2>(1),
				tuple.Get<T3>(2),
				tuple.Get<T4>(3)
			);
		}

		/// <summary>Returns a typed version of a tuple of size 5</summary>
		/// <typeparam name="T1">Expected type of the first element</typeparam>
		/// <typeparam name="T2">Expected type of the second element</typeparam>
		/// <typeparam name="T3">Expected type of the third element</typeparam>
		/// <typeparam name="T4">Expected type of the fourth element</typeparam>
		/// <typeparam name="T5">Expected type of the fifth element</typeparam>
		/// <param name="tuple">Tuple that must be of size 5</param>
		/// <returns>Equivalent tuple, with its elements converted to the specified types</returns>
		public static STuple<T1, T2, T3, T4, T5> As<T1, T2, T3, T4, T5>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(5);
			return new STuple<T1, T2, T3, T4, T5>(
				tuple.Get<T1>(0),
				tuple.Get<T2>(1),
				tuple.Get<T3>(2),
				tuple.Get<T4>(3),
				tuple.Get<T5>(4)
			);
		}

		/// <summary>Returns a typed version of a tuple of size 5</summary>
		/// <typeparam name="T1">Expected type of the first element</typeparam>
		/// <typeparam name="T2">Expected type of the second element</typeparam>
		/// <typeparam name="T3">Expected type of the third element</typeparam>
		/// <typeparam name="T4">Expected type of the fourth element</typeparam>
		/// <typeparam name="T5">Expected type of the fifth element</typeparam>
		/// <typeparam name="T6">Expected type of the sixth element</typeparam>
		/// <param name="tuple">Tuple that must be of size 5</param>
		/// <returns>Equivalent tuple, with its elements converted to the specified types</returns>
		public static STuple<T1, T2, T3, T4, T5, T6> As<T1, T2, T3, T4, T5, T6>([NotNull] this ITuple tuple)
		{
			tuple.OfSize(6);
			return new STuple<T1, T2, T3, T4, T5, T6>(
				tuple.Get<T1>(0),
				tuple.Get<T2>(1),
				tuple.Get<T3>(2),
				tuple.Get<T4>(3),
				tuple.Get<T5>(4),
				tuple.Get<T6>(5)
			);
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 1</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1>([NotNull] this ITuple tuple, [NotNull] Action<T1> lambda)
		{
			OfSize(tuple, 1);
			lambda(tuple.Get<T1>(0));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 2</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2> lambda)
		{
			OfSize(tuple, 2);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 3</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3> lambda)
		{
			OfSize(tuple, 3);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 4</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3, T4>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3, T4> lambda)
		{
			OfSize(tuple, 4);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 5</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3, T4, T5>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3, T4, T5> lambda)
		{
			OfSize(tuple, 5);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 6</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3, T4, T5, T6>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3, T4, T5, T6> lambda)
		{
			OfSize(tuple, 6);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 7</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3, T4, T5, T6, T7>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3, T4, T5, T6, T7> lambda)
		{
			OfSize(tuple, 7);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5), tuple.Get<T7>(6));
		}

		/// <summary>Execute a lambda Action with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 8</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static void With<T1, T2, T3, T4, T5, T6, T7, T8>([NotNull] this ITuple tuple, [NotNull] Action<T1, T2, T3, T4, T5, T6, T7, T8> lambda)
		{
			OfSize(tuple, 8);
			lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5), tuple.Get<T7>(6), tuple.Get<T8>(7));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 1</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, TResult> lambda)
		{
			return lambda(tuple.OfSize(1).Get<T1>(0));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 2</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, TResult> lambda)
		{
			OfSize(tuple, 2);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 3</param>
		/// <param name="lambda">Action that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, TResult> lambda)
		{
			OfSize(tuple, 3);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 4</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, T4, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, T4, TResult> lambda)
		{
			OfSize(tuple, 4);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 5</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, T4, T5, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, T4, T5, TResult> lambda)
		{
			OfSize(tuple, 5);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 6</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, T4, T5, T6, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, T4, T5, T6, TResult> lambda)
		{
			OfSize(tuple, 6);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 7</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, T4, T5, T6, T7, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, TResult> lambda)
		{
			OfSize(tuple, 7);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5), tuple.Get<T7>(6));
		}

		/// <summary>Execute a lambda Function with the content of this tuple</summary>
		/// <param name="tuple">Tuple of size 8</param>
		/// <param name="lambda">Function that will be passed the content of this tuple as parameters</param>
		/// <returns>Result of calling <paramref name="lambda"/> with the items of this tuple</returns>
		/// <exception cref="InvalidOperationException">If <paramref name="tuple"/> has not the expected size</exception>
		public static TResult With<T1, T2, T3, T4, T5, T6, T7, T8, TResult>([NotNull] this ITuple tuple, [NotNull] Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult> lambda)
		{
			OfSize(tuple, 8);
			return lambda(tuple.Get<T1>(0), tuple.Get<T2>(1), tuple.Get<T3>(2), tuple.Get<T4>(3), tuple.Get<T5>(4), tuple.Get<T6>(5), tuple.Get<T7>(6), tuple.Get<T8>(7));
		}

		#endregion

		#region Deconstruction (C#7)

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1>(this ITuple value, out T1 item1)
		{
			item1 = value.OfSize(1).Get<T1>(0);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2>(this ITuple value, out T1 item1, out T2 item2)
		{
			value.OfSize(2);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3>(this ITuple value, out T1 item1, out T2 item2, out T3 item3)
		{
			value.OfSize(3);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3, T4>(this ITuple value, out T1 item1, out T2 item2, out T3 item3, out T4 item4)
		{
			value.OfSize(4);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
			item4 = value.Get<T4>(3);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3, T4, T5>(this ITuple value, out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5)
		{
			value.OfSize(5);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
			item4 = value.Get<T4>(3);
			item5 = value.Get<T5>(4);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3, T4, T5, T6>(this ITuple value, out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6)
		{
			value.OfSize(6);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
			item4 = value.Get<T4>(3);
			item5 = value.Get<T5>(4);
			item6 = value.Get<T6>(5);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7>(this ITuple value, out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7)
		{
			value.OfSize(7);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
			item4 = value.Get<T4>(3);
			item5 = value.Get<T5>(4);
			item6 = value.Get<T6>(5);
			item7 = value.Get<T7>(6);
		}

		[EditorBrowsable(EditorBrowsableState.Never)]
		public static void Deconstruct<T1, T2, T3, T4, T5, T6, T7, T8>(this ITuple value, out T1 item1, out T2 item2, out T3 item3, out T4 item4, out T5 item5, out T6 item6, out T7 item7, out T8 item8)
		{
			value.OfSize(8);
			item1 = value.Get<T1>(0);
			item2 = value.Get<T2>(1);
			item3 = value.Get<T3>(2);
			item4 = value.Get<T4>(3);
			item5 = value.Get<T5>(4);
			item6 = value.Get<T6>(5);
			item7 = value.Get<T7>(6);
			item8 = value.Get<T8>(7);
		}

		#endregion

		#region ValueTuple (C#7)

#if ENABLE_VALUETUPLES

		[Pure]
		public static STuple ToSTuple<T1>(this ValueTuple tuple)
		{
			return default(STuple);
		}

		[Pure]
		public static STuple<T1> ToSTuple<T1>(this ValueTuple<T1> tuple)
		{
			return new STuple<T1>(tuple.Item1);
		}

		[Pure]
		public static STuple<T1, T2> ToSTuple<T1, T2>(this ValueTuple<T1, T2> tuple)
		{
			return new STuple<T1, T2>(tuple.Item1, tuple.Item2);
		}

		[Pure]
		public static STuple<T1, T2, T3> ToSTuple<T1, T2, T3>(this ValueTuple<T1, T2, T3> tuple)
		{
			return new STuple<T1, T2, T3>(tuple.Item1, tuple.Item2, tuple.Item3);
		}

		[Pure]
		public static STuple<T1, T2, T3, T4> ToSTuple<T1, T2, T3, T4>(this ValueTuple<T1, T2, T3, T4> tuple)
		{
			return new STuple<T1, T2, T3, T4>(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
		}

		[Pure]
		public static STuple<T1, T2, T3, T4, T5> ToSTuple<T1, T2, T3, T4, T5>(this ValueTuple<T1, T2, T3, T4, T5> tuple)
		{
			return new STuple<T1, T2, T3, T4, T5>(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
		}

		[Pure]
		public static STuple<T1, T2, T3, T4, T5, T6> ToSTuple<T1, T2, T3, T4, T5, T6>(this ValueTuple<T1, T2, T3, T4, T5, T6> tuple)
		{
			return new STuple<T1, T2, T3, T4, T5, T6>(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);
		}

#endif

		#endregion

	}

}
