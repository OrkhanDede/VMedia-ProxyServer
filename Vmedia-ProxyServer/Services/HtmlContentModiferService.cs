using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace VMedia_ProxyServer.Services
{
    public class HtmlContentModiferService: IContentModiferService
    {
        public string Modify(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var textNodes = doc.DocumentNode.SelectNodes("//div[contains(@id,'content')]//text()").Cast<HtmlTextNode>().ToList();

            foreach (var child in textNodes)
            {
                var strings = child.Text.Split(" ");
                foreach (var s in strings)
                {
                    var rgx = new Regex("^[a-zA-Z]{6}$");
                    if (rgx.Match(s).Success)
                    {
                        var replace = child.Text.Replace(s, $"{s}TM");
                        child.Text = replace;
                    }
                }
            }
            return doc.DocumentNode.OuterHtml;
        }
    }
}
