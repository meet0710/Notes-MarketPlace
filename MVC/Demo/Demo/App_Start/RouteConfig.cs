using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Demo
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default1",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Registration", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default2",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
			);

			
			routes.MapRoute(
				name: "Default3",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Admin", action = "AdminNotesUnderReview", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default4",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Admin", action = "AdminPublishedNotes", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default5",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Admin", action = "AdminDownloads", id = UrlParameter.Optional }
			);
		}
	}
}
