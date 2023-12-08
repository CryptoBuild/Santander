using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
namespace Santander.Controllers
{
    public class NewsViewModel
    {
        public NewsViewModel()
        {
            LoadViewModel();
        }

        public void LoadViewModel()
        {
            string url = "https://hacker-news.firebaseio.com/v0/";
            string urlParameters = "beststories.json";
            var client = new HttpClient();

            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = client.GetAsync(urlParameters).Result;

            NewsItemList.Clear();
            if (response.IsSuccessStatusCode)
            {
                IEnumerable<long> IDListOfFavoriteStories = response.Content.ReadAsAsync<IEnumerable<long>>().Result;

                // Use Parallel.ForEach to process items concurrently
                Parallel.ForEach(IDListOfFavoriteStories, item =>
                {
                    urlParameters = string.Format("item/{0}.json", item);
                    var response2 = client.GetAsync(urlParameters).Result;

                    if (response2.IsSuccessStatusCode)
                    {
                        var NewsDataItemResponse = response2.Content.ReadAsStringAsync().Result;
                        NewsViewModelItem myDeserializedClass = JsonConvert.DeserializeObject<NewsViewModelItem>(NewsDataItemResponse);
                        //myDeserializedClass.uri = url + urlParameters;
                        // Use lock to ensure thread safety when modifying shared data (NewsItemList)
                      //  lock (NewsItemList)
                       // {
                            NewsItemList.Add(myDeserializedClass);
                        //}
                    }
                    else
                    {
                        throw new Exception("Had an error");
                    }
                });

                // Sort the NewsItemList after all threads have completed
                List<NewsViewModelItem> EnumerableNewsViewModelItem = NewsItemList.OrderBy(T => T.score).Reverse().ToList();
            }
            else
            {
                // Handle error response
                throw new Exception($"{(int)response.StatusCode} ({response.ReasonPhrase})");
            }
        }
        public List<NewsViewModelItem> NewsItemList { get; set; } = new List<NewsViewModelItem>();
        public IEnumerable<NewsViewModelItem> EnumerableNewsViewModelItem { get; set; }
        public class NewsViewModelItem
        {

            DateTime t = new DateTime();
            public string title { get; set; }
           
            public string uri
            {
                get => url;
            }

            [JsonProperty]
            private string url { get; set; }

            private string m_postedBy;
            [JsonProperty]
            private string by
            {   get;set; }
            public string postedBy
            {
                get
                {
                    return by;
                }
                set { m_postedBy = by; }
            }
            private string m_time;
                
            public string time {
                get { 
                    return UnixTimeStampToDateTime(m_time); 
                }
                set {
                    m_time = value; 
                }
            }
            public int score { get; set; }
            public int commentCount{
                get {
                    return kids !=null ? kids.Count():0 ; 
                }
            }

            [JsonProperty] 
            private  List<int> kids { get; set; }
            // unused properties 
            //public string type { get; set; }
            //public string url { get; set; }
            //public int descendants { get; set; }
            //public int id { get; set; }
        }
        public static string UnixTimeStampToDateTime(string unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(double.Parse(unixTimeStamp)).ToLocalTime();
            DateTimeOffset dto = new DateTimeOffset(dateTime);
            return dto.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss':'zzz");
        }
    }
}

