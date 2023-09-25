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
            lock (this)
            {
                string url = "https://hacker-news.firebaseio.com/v0/";///beststories.json";
                string urlParameters = "beststories.json";
                var client = new HttpClient();

                client.BaseAddress = new Uri(url);// Add an Accept header for JSON format.
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var response = client.GetAsync(urlParameters).Result;// Get data response

                NewsItemList.Clear();
                if (response.IsSuccessStatusCode)
                {
                    // Parse the response body
                    IEnumerable<long> IDListOfFavoriteStories = response.Content.ReadAsAsync<IEnumerable<long>>().Result;

                    foreach (var item in IDListOfFavoriteStories)
                    {
                        urlParameters = string.Format("item/{0}.json", item);
                        var response2 = client.GetAsync(urlParameters).Result;
                        if (response2.IsSuccessStatusCode)
                        {
                            var NewsDataItemResponse = response2.Content.ReadAsStringAsync().Result;
                            NewsViewModelItem myDeserializedClass = JsonConvert.DeserializeObject<NewsViewModelItem>(NewsDataItemResponse);
                            NewsItemList.Add(myDeserializedClass);
                        }
                        else
                        {
                            throw new Exception("had an error");
                        }
                    }
                    List<NewsViewModelItem> EnumerableNewsViewModelItem = NewsItemList.OrderBy(T => T.score).Reverse().ToList();
                    //            for (int n = 0; n < 100; n++)
                    //             {
                    //Console.WriteLine($"Item number [{n + 1}] Title [{SortedListByScore[n].title}] Score : {SortedListByScore[n].score}");
                    //           }
                }
                else
                {
                    //         Console.WriteLine("{0} ({1})", (int)response.StatusCode,response.ReasonPhrase);
                }
            }
        }

        public List<NewsViewModelItem> NewsItemList { get; set; } = new List<NewsViewModelItem>();
        public IEnumerable<NewsViewModelItem> EnumerableNewsViewModelItem { get; set; }
        public class NewsViewModelItem
        {
            public string by { get; set; }
            public int descendants { get; set; }
            public int id { get; set; }
            public List<int> kids { get; set; }
            public int score { get; set; }
            public int time { get; set; }
            public string title { get; set; }
            public string type { get; set; }
            public string url { get; set; }
        }
    }
}

