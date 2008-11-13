using System;
using System.Collections.Generic;
using System.Text;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues
{
	public class IssueTestCase : AbstractNHibernateTestCase
	{
		protected virtual string BugNumber
		{
			get
			{
				string ns = GetType().Namespace;
				return ns.Substring(ns.LastIndexOf('.') + 1);
			}
		}

		protected override string ConfigurationFile
		{
			get
			{
				return "Issues/" + BugNumber + "/facility.xml";
			}
		}
	}
}