//using System;
//using Castle.MicroKernel;
//using Castle.Model.Configuration;
//
//namespace Castle.Facilities.ActiveRecord
//{
//	public class ActiveRecordFacility : IFacility
//	{
//		private IKernel _kernel;
//
//		#region IFacility implementation
//
//		public void Init( IKernel kernel, IConfiguration facilityConfig )
//		{
//			_kernel = kernel;
//			RegisterEngines( facilityConfig );
//		}
//
//		public void Terminate()
//		{
//		}
//
//		#endregion
//
//		private void RegisterEngines( IConfiguration config )
//		{
//			IConfiguration engines = config.Children["engines"];
//			foreach( IConfiguration engine in engines.Children )
//			{
//				RegisterEngine( engine );
//			}
//		}
//
//		private void RegisterEngine( IConfiguration engine )
//		{
//			string key = engine.Attributes["id"];
//			string arAssembly = engine.Attributes["assembly"];
//		}
//	}
//}