using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace Bing
{
	class Program
	{
		private static readonly string directory = @"C:\Bing\";
		[DllImport("user32.dll")]
		private static extern long SystemParametersInfo(int uActiong, int uParam, string lpvParam, int fuWinIni);
		const int SPIF_UPDATEINIFILE = 1;
		const int SPI_SETSCREENSAVEACTIVE = 17;
		const int SPI_SETDESKWALLPAPER = 20;
		const int SPIF_SENDWININICHANGE = 2;
		const int SPI_SCREENSAVERRUNNING = 97;
		static DateTime DateLastRun;

		static void Main(string[] args)
		{
			DateLastRun = DateTime.Now.Date;
			Console.WriteLine("Welcome in Bing Daily Wallpaper app.\n-If u want change your wallpaper now write 1\n-If u want to zet your wallpaper automatically at midnight set write 2");
			int n = int.Parse(Console.ReadLine());
			switch (n)
			{
				case 1:
					Wallpaper();
					Console.WriteLine("Apllication will change wallpaper now automatically at midnight.");
					goto case 2;
				case 2:
					Console.WriteLine("Working...");
					if (DateLastRun < DateTime.Now.Date)
					{
						Wallpaper();
					}
					else
					{
						int milliseconds = 1000 * 60 * 60; //verification every hour
						Thread.Sleep(milliseconds);
					}
					goto case 2;
				default:
					Console.WriteLine("Invalid selection. Try again.");
					break;
			}
			Console.ReadLine();
		}

		private static void Wallpaper()
		{
			Console.WriteLine($"Creating directory if not exist.");
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
			string backgroundName = directory + DateTime.Today.ToString("dd-MM-yyyy") + ".jpg";
			string reply = null;
			FinxXmlFile(ref reply);
			FindUrlInXml(ref reply);
			Console.WriteLine($"Downloading today Bing wallpaper file: {backgroundName}");
			new WebClient().DownloadFile("http://www.bing.com" + reply + "_1920x1200.jpg", backgroundName);
			Console.WriteLine($"Setting Bing photo as wallpaper\nDone!");
			SystemParametersInfo(SPI_SETDESKWALLPAPER, 1, backgroundName, SPIF_UPDATEINIFILE);
			DateLastRun = DateTime.Now.Date;
			Console.WriteLine($"Changed wallpeper at {DateTime.Now}");
		}

		private static void FinxXmlFile(ref string reply)
		{
			var webrequest = (HttpWebRequest)WebRequest.Create("https://www.bing.com/HPImageArchive.aspx?format=xml&idx=0&n=1&mkt=en-US");
			using (var webreply = (HttpWebResponse)webrequest.GetResponse())
			using (var stream = new StreamReader(webreply.GetResponseStream()))
			{
				reply = stream.ReadToEnd();
			}
		}

		private static void FindUrlInXml(ref string imageUrl)
		{
			if (imageUrl == null)
			{
				throw new ArgumentNullException(nameof(imageUrl));
			}

			using (var findUrl = System.Xml.XmlReader.Create(new StringReader(imageUrl)))
			{
				findUrl.ReadToFollowing("urlBase");
				imageUrl = findUrl.ReadElementContentAsString();
			}
		}
	}
}