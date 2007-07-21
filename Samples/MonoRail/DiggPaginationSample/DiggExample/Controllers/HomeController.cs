namespace DiggExample.Controllers
{
	using System;
	using System.Collections.Generic;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;

	using DiggExample.Model;
	using NHibernate.Expression;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{

		}

		public void Index1()
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", true)),
				10
			);
		}
		public void Index2()
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", true)),
				10
			);
		}
		public void Index3()
		{
			List<MyEntity> el = new List<MyEntity>(MyEntity.FindAll(new Order("Index", true)));

			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				el.GetRange(0,5),
				10
			);
		}
		public void Index4()
		{
			List<MyEntity> el = new List<MyEntity>(MyEntity.FindAll(new Order("Index", true)));

			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				el.GetRange(0, 5),
				10
			);
		}
		public void Index5()
		{
			List<MyEntity> el = new List<MyEntity>(MyEntity.FindAll(new Order("Index", true)));

			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				el.GetRange(0, 5),
				10
			);
		}
		public void Index6()
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", true)),
				10
			);
		}
		public void Index7()
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", true)),
				10
			);
		}
		public void Index8(bool desc)
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", (!desc))),
				10
			);
			PropertyBag["desc"] = desc;
		}
		public void Index9()
		{
			PropertyBag["items"] = PaginationHelper.CreatePagination<MyEntity>(
				this,
				MyEntity.FindAll(new Order("Index", true)),
				10
			);
		}
	}
}
