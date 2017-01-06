using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Web.Api.Auth
{
	public class ApiSettings
	{
		public const string URLS_WEB = "projectrequests.weburl";
		public const string URLS_API = "projectrequests.apiurl";

		public const string SECURITY_ACCESSTOKENEXPIRATIONINDAYS = "security.accesstokenexpirationindays";

		public Dictionary<string, string> Settings
		{
			get
			{
				ConfigurationManager.RefreshSection("appSettings");

				var dic = new Dictionary<string, string>();

				foreach (var keys in ConfigurationManager.AppSettings.AllKeys)
				{
					dic.Add(keys, ConfigurationManager.AppSettings[keys]);
				}

				return dic;
			}
		}

		public ApiSettings() { }
	}
}