﻿#region BSD Licence
/* Copyright (c) 2013, Doxense SARL
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

namespace FoundationDB.Client
{
	using System;
	using System.Runtime.Serialization;
	using System.Security;

	/// <summary>FoundationDB API Error Code</summary>
	[Serializable]
	public sealed class FdbException : Exception
	{

		public FdbException(FdbError errorCode)
			: this(errorCode, Fdb.GetErrorMessage(errorCode), null)
		{
		}

		public FdbException(FdbError errorCode, string message)
			: this(errorCode, message, null)
		{		
		}

		public FdbException(FdbError errorCode, string message, Exception innerException)
			: base(message, innerException)
		{
			this.Code = errorCode;
		}

		private FdbException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Code = (FdbError)info.GetInt32("Code");
		}

		[SecurityCritical]
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Code", (int)this.Code);
		}

		/// <summary>Gets the code for this error.</summary>
		public FdbError Code { get; private set; }

		/// <summary>Determine if this FDBError represents a success code from the native layer.</summary>
		public bool Success { get { return this.Code == FdbError.Success; } }

	}

}
