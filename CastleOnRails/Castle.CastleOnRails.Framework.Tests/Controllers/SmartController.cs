using NUnit.Framework;
// ${Copyrigth}

namespace Castle.CastleOnRails.Framework.Tests.Controllers
{
	using System;

	
	public class SmartController : SmartDispatcherController
	{
		internal static bool StrInvoked;
		internal static bool IntInvoked;

		public SmartController()
		{
		}

		public void ToBoolean(Boolean bol)
		{
			Assert.IsTrue(bol);

			OkMessage();
		}

		private void OkMessage()
		{
			RenderText("ok");
		}

		public void ToDatetime(DateTime dt)
		{
			Assert.AreEqual(DateTime.Parse("12/12/2000"), dt);
			OkMessage();
		}

		public void ToDouble(Double d)
		{
			Assert.AreEqual(Convert.ToDouble(1), d);
			OkMessage();
		}

		public void ToSingle(Single s)
		{
			Assert.AreEqual(Convert.ToSingle(1), s);
			OkMessage();
		}

		public void ToSByte(SByte b)
		{
			Assert.AreEqual(Convert.ToSByte(1), b);
			OkMessage();
		}

		public void ToByte(Byte b)
		{
			Assert.AreEqual(Convert.ToByte(1), b);
			OkMessage();
		}

		public void ToInt64(Int64 i)
		{
			Assert.AreEqual(Convert.ToInt64(1), i);
			OkMessage();
		}

		public void ToInt32Array(Int32[] i)
		{
			Assert.AreEqual(3, i.Length);
			Assert.AreEqual(1, i[0]);
			Assert.AreEqual(2, i[1]);
			Assert.AreEqual(3, i[2]);

			OkMessage();
		}

		public void ToInt32(Int32 i)
		{
			Assert.AreEqual(1, i);
			OkMessage();
		}

		public void ToInt16(Int16 i)
		{
			Assert.AreEqual(Convert.ToInt16(1), i);
			OkMessage();
		}

		public void ToUInt64(UInt64 i)
		{
			Assert.AreEqual(Convert.ToUInt64(1), i);
			OkMessage();
		}

		public void ToUInt32(UInt32 i)
		{
			Assert.AreEqual(Convert.ToUInt32(1), i);
			OkMessage();
		}

		public void ToUInt16(UInt16 i)
		{
			Assert.AreEqual(Convert.ToUInt16(1), i);
			OkMessage();
		}

		public void ToGuid(Guid gd)
		{
			Assert.AreEqual(Guid.Empty, gd);
			OkMessage();
		}

		public void ToStringArray(string[] str)
		{
			Assert.AreEqual(3, str.Length);
			Assert.AreEqual("a", str[0]);
			Assert.AreEqual("b", str[1]);
			Assert.AreEqual("c", str[2]);

			OkMessage();
		}

		public void ToString(string str)
		{
			Assert.AreEqual("strv", "strv");
			OkMessage();
		}

		public void ToInt(int i)
		{
			Assert.AreEqual(1, i);
			OkMessage();
		}

		public void Overloaded(int i)
		{
			ToInt(i);

			IntInvoked = true;
		}

		public void Overloaded(string str)
		{
			ToString(str);

			StrInvoked = true;
		}
	}
}
