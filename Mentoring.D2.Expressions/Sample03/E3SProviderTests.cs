﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sample03.E3SClient.Entities;
using Sample03.E3SClient;
using System.Configuration;
using System.Linq;

namespace Sample03
{
	[TestClass]
	public class E3SProviderTests
	{
		[TestMethod]
		public void WithoutProvider()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"] , ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS<EmployeeEntity>("workstation:(EPRUIZHW0249)", 0, 1);

			foreach (var emp in res)
			{
				Console.WriteLine("{0} {1}",emp.nativename, emp.shortStartWorkDate);
			}
		}

		[TestMethod]
		public void WithoutProviderNonGeneric()
		{
			var client = new E3SQueryClient(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);
			var res = client.SearchFTS(typeof(EmployeeEntity), "workstation:(EPRUIZHW0249)", 0, 10);

			foreach (var emp in res.OfType<EmployeeEntity>())
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.workstation);
			}
		}


		[TestMethod]
		public void AndAlsoAndStartsWithAndContainsTest()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.StartsWith("EPRUIZHW") && e.nativename.Contains("Михаил")))
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.workstation);
			}
		}
		[TestMethod]
		public void ConstantFirstTest()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => ("EPRUIZHW0249*") == e.workstation))
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.workstation);
			}
		}
		[TestMethod]
		public void EndsWithTest()
		{
			var employees = new E3SEntitySet<EmployeeEntity>(ConfigurationManager.AppSettings["user"], ConfigurationManager.AppSettings["password"]);

			foreach (var emp in employees.Where(e => e.workstation.EndsWith("0249")))
			{
				Console.WriteLine("{0} {1}", emp.nativename, emp.workstation);
			}
		}

	}
}
