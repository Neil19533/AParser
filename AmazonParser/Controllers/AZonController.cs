using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AmazonParser.Controllers
{
    public class AZonController : ApiController
    {

        public HttpResponseMessage Get(string ASin)
        {
            var res = new HttpResponseMessage();


            AmazonProduct content = GetContent(ASin);



            res.StatusCode = HttpStatusCode.OK;
            res.Content = new ObjectContent(typeof(AmazonProduct), content, new System.Net.Http.Formatting.XmlMediaTypeFormatter());

            
            
            return res; // new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
        }




        private AmazonProduct GetContent(String ASin)
        {
            var content = "";

            var client = new WebClient();

            var headers = new WebHeaderCollection();

            headers.Add(HttpRequestHeader.Accept, "text/html, application/xhtml+xml, */*");
            //headers.Add(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
            headers.Add(HttpRequestHeader.AcceptLanguage, "en-GB");
            headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 6.1; WOW64; Trident/7.0; rv:11.0) like Gecko");

            client.Headers = headers;


            var rawhtml = client.DownloadString("http://www.amazon.co.uk/dp/"+ ASin);



            HtmlAgilityPack.HtmlDocument Html = new HtmlAgilityPack.HtmlDocument();


            Html.LoadHtml(rawhtml);



            var title = GetTitle(Html);

            var description = GetDescription(Html);


            AmazonProduct prod = new AmazonProduct() { Description = description, Title = title };

            return prod; 
        }

        private string GetDescription(HtmlAgilityPack.HtmlDocument Html)
        {

            var description = Html.GetElementbyId("productDescription");
            

            if (description == null)
            {
                try
                {
                    description = Html.GetElementbyId("pd-available").NextSibling.NextSibling;


                    string javatoparse = description.InnerText;


                    var frame = GetJavaIFrame(javatoparse);

                    var htmlcode = WebUtility.UrlDecode(frame);

                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(htmlcode);

                    description = doc.GetElementbyId("productDescription");
                
                }
                catch (Exception)
                {
                    
                }


            }

            var descriptiontext = "";

            if (description != null)
            {
                descriptiontext = description.InnerText;
            }

            return descriptiontext;
        }

        private string GetJavaIFrame(string javatoparse)
        {
            int start = javatoparse.IndexOf("var iframeContent = ") + 21;

            int end = javatoparse.IndexOf('\"', start + 1);

            var frame = javatoparse.Substring(start, end - start);
            return frame;
        }

        private string GetTitle(HtmlAgilityPack.HtmlDocument Html)
        {

            var title = Html.GetElementbyId("productTitle");
            if (title == null)
            {
                title = Html.GetElementbyId("btAsinTitle");
            }

            return title.InnerText.Trim();
        }



    }

    [Serializable]
    public class AmazonProduct
    {
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
