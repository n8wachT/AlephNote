﻿using System;
using System.Collections.Generic;
using System.Linq;
using AlephNote.Common.Network;
using AlephNote.PluginInterface;

namespace AlephNote.Common.Operations
{
	public class GithubConnection
	{
#pragma warning disable 0649
// ReSharper disable All
		private class JsonResponseAsset { public string browser_download_url; }
		private class JsonResponse { public string tag_name; public DateTime published_at; public List<JsonResponseAsset> assets; }
// ReSharper restore All
#pragma warning restore 0649

		public Tuple<Version, DateTime, string> GetLatestRelease(bool includeBeta)
		{
			if (includeBeta)
			{
				var rest = new SimpleJsonRest(null, @"https://api.github.com");

				var response = rest.Get<List<JsonResponse>>("repos/Mikescher/AlephNote/releases").First();

				var url = response.assets.First(a => a.browser_download_url.EndsWith(".zip")).browser_download_url;
				var date = response.published_at;
				var version = ParseVersion(response.tag_name);

				return Tuple.Create(version, date, url);
			}
			else
			{
				var rest = new SimpleJsonRest(null, @"https://api.github.com");

				var response = rest.Get<JsonResponse>("repos/Mikescher/AlephNote/releases/latest");

				var url = response.assets.First(a => a.browser_download_url.EndsWith(".zip")).browser_download_url;
				var date = response.published_at;
				var version = ParseVersion(response.tag_name);

				return Tuple.Create(version, date, url);
			}


		}

		private static Version ParseVersion(string dat)
		{
			dat = dat.Trim().ToLower();
			if (dat.StartsWith("v")) dat = dat.Substring(1);
			return Version.Parse(dat);
		}
	}
}
