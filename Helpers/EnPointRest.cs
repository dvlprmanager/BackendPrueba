using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PruebaBackend.Models;
using RestSharp;

namespace PruebaBackend.Helpers
{
    public class EnPointRest
    {
        public List<Image> GetAllImage(string _url, string _user, string _pass)
        {
            List<Image> image = new List<Image>();
            var client = new RestClient(_url);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("user", _user);
            request.AddHeader("password", _pass);
            IRestResponse response = client.Execute(request);

            return image = JsonConvert.DeserializeObject<List<Image>>(response.Content);

        }

        public Image SaveImage(JObject request,string _url, string _user, string _pass)
        {
            Image resp = new Image();

            string bodyString = JsonConvert.SerializeObject(request);
            var client = new RestClient(_url);
            client.Timeout = -1;
            var req = new RestRequest(Method.POST);
            req.AddHeader("user", _user);
            req.AddHeader("password", _pass);
            req.AddHeader("Content-Type", "application/json");
            var body = bodyString;
            req.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(req);
           return resp = JsonConvert.DeserializeObject<Image>(response.Content);
        }

        public Image GetOneImage(int id,string _url, string _user, string _pass)
        {
            Image image = new Image();
            var client = new RestClient(_url+"/"+id);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            request.AddHeader("user", _user);
            request.AddHeader("password", _pass);
            IRestResponse response = client.Execute(request);

            return image = JsonConvert.DeserializeObject<Image>(response.Content);

        }

    }
}
