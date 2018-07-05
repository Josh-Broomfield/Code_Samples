using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Web_Page_Download
{
    class Program
    {
        static void Main(String[] args)
        {
            String saveDir = @"S:\X";
            String preUrl = @"http://X.gov.ca/";
            String postUrl = @"FixedWidthFormat.aspx";
            String contents = ReadTextFromUrl(preUrl + postUrl);

            String link = FindLink(contents, "X");

            String text = ReadTextFromUrl(preUrl + link);

            String filename = link.Substring(link.IndexOf(@"/") + 1);
            File.WriteAllText(saveDir + "\\" + filename, text);
        }

        /// <summary>
        /// Finds an a href link that occurs after some text
        /// </summary>
        /// <param name="contents">Webpage</param>
        /// <param name="textBeforeLink">Used to search for a file's link</param>
        /// <returns></returns>
        static String FindLink(String contents, String textBeforeLink)
        {
            String preLink = "a href=\"";
            Console.WriteLine(contents);

            int linkIndex = contents.LastIndexOf(textBeforeLink);
            String contentsBeforeLink = contents.Substring(linkIndex);

            String atLink = contentsBeforeLink.Substring(contentsBeforeLink.IndexOf(preLink) + preLink.Length);
            String onlyLink = atLink.Substring(0, atLink.IndexOf("\""));

            return onlyLink;
        }

        //http://stackoverflow.com/a/16369288/4051272
        static String ReadTextFromUrl(String url)
        {
            // WebClient is still convenient
            // Assume UTF8, but detect BOM - could also honor response charset I suppose
            using (var client = new WebClient())
            using (var stream = client.OpenRead(url))
            using (var textReader = new StreamReader(stream, Encoding.UTF8, true))
            {
                return textReader.ReadToEnd();
            }
        }
    }
}
