using BrowserController.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.IO.Compression;
using System.Windows.Forms;

namespace BrowserController.DB
{ 
    public class WebDB 
    {
        static HttpClient client;

        static string login = "Cap", password = "pass";
        static string baseUrl = "http://localhost:8080";

        public WebDB() {
            var authValue = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}")));

            client = new HttpClient()
            {
                DefaultRequestHeaders = { Authorization = authValue }
            };
        }
        
        /////////////////////////////////////////////////////////////////////////////////////User requests
        public async Task<List<User>> getAllUsers() {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl+"/user/get/all");

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceUsersFromJSON(jsonReq);
        }

        public async Task<User> getUser(long id)
        {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl+"/user/get/" + id);

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceUserFromJSON(jsonReq);
        }
        public User getUserById(int id)
        {
            var request = WebRequest.Create(baseUrl + "/user/get/" + id.ToString());
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParceUserFromJSON(jsonOut);
            }
        }

        public void CreateUser(User user)
        {
            var request = WebRequest.Create(baseUrl+"/user/add");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = UserToJSON(user);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }
            
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Console.WriteLine(reader.ReadToEnd());
            }

        }

        public void CreateSite(Site site)
        {
            var request = WebRequest.Create(baseUrl + "/site/add");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = SiteToJSON(site);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Console.WriteLine(reader.ReadToEnd());
            }

        }
        public void CreatePage(PageDto page)
        {
            var request = WebRequest.Create(baseUrl + "/page/add");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = PageToJSON(page);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Console.WriteLine(reader.ReadToEnd());
            }

        }
        public void updatePage(PageDto page)
        {
            var request = WebRequest.Create(baseUrl + "/page/update");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "PUT";

            string json = PageToJSON(page);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Console.WriteLine(reader.ReadToEnd());
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////Sessions
        public async Task<List<Session>> getAllSessions()
        {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl +"/session/get/all");

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceSessionsFromJSON(jsonReq);
        }

        public async Task<List<Session>> allOnSite(int siteId) {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl + "/session/get/allOnSite/" + siteId);

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceSessionsFromJSON(jsonReq);
        }
        public List<Session> allSessionsOnSite(int siteId)
        {
            var request = WebRequest.Create(baseUrl + "/session/get/allOnSite/" + siteId);
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParceSessionsFromJSON(jsonOut);
            }
        }

        public List<PageDto> getAllOnSite(int id)
        {
            var request = WebRequest.Create(baseUrl + "/page/getAllOnSite/" + id.ToString());
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParcePagesDtoFromJSON(jsonOut);
            }
        }
        public async Task<Session> getSession(long id)
        {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl+"/session/get/" + id);

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceSessionFromJSON(jsonReq);
        }

        public Session CreateSession(Session session)
        {
            var request = WebRequest.Create(baseUrl+"/session/add");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = SessionToJSON(session);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                Console.WriteLine(reader.ReadToEnd());
                return ParceSessionFromJSON(jsonOut);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////SessionsFrame
        public void CreateFrames(List<FrameTraectoryes> ft)
        {
            var request = WebRequest.Create(baseUrl+ "/trajectory/save");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = FrameTraectoryesToJSON(ft);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                Console.WriteLine(reader.ReadToEnd());
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////Sites
        public async Task<List<Site>> getAllSites() {
            string jsonReq = "";

            HttpResponseMessage response = await client.GetAsync(baseUrl+"/site/get/all");

            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceSitesFromJSON(jsonReq);
        }

        public List<Site> getSites()
        {
            var request = WebRequest.Create(baseUrl + "/site/get/all");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";
            try
            {
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    string jsonOut = reader.ReadToEnd();
                    return ParceSitesFromJSON(jsonOut);
                }
            }
            catch (System.Net.WebException ex)
            {
                return null;
            }
        }

        public int getIdSiteByName(string name)
        {
            var request = WebRequest.Create(baseUrl + "/site/get/all");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                List<Site> sites = ParceSitesFromJSON(jsonOut);
                int answ = 0;
                for (int i = 0; i < sites.Count; i++)
                {
                    if (sites[i].name == name)
                    {
                        answ = sites[i].id;
                        break;
                    }
                }
                return answ;
            }
        }
        public PageDto getPage(int id)
        {
            var request = WebRequest.Create(baseUrl + "/page/get/"+id.ToString());
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParcePageDtoFromJSON(jsonOut);
            }
        }

        public static async Task DownloadFile(string name)
        {
            byte[] data;

            if (Directory.Exists("tmp"))
            {
                Directory.Delete("tmp", true);
            }

            Directory.CreateDirectory("tmp");

            using (HttpResponseMessage response = await client.GetAsync(baseUrl + "/site/files/" + name))
            using (HttpContent content = response.Content)
            {
                data = await content.ReadAsByteArrayAsync();
                using (FileStream file = File.Create("tmp/"+name))
                    file.Write(data, 0, data.Length);
                
                ZipFile.ExtractToDirectory("tmp/" + name, Application.StartupPath+"/tmp");
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////URLDTO
        public PageDto getPageId(urlDto url)
        {
            var request = WebRequest.Create(baseUrl + "/page/getPageId");
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "POST";

            string json = baseUrlToJSON(url);
            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(json);
            }

            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                Console.WriteLine(reader.ReadToEnd());
                return ParcePageDtoFromJSON(jsonOut);
            }
        }





        ////////////////////////////////////////////////////////////////////////////////////URLDTO
        public async Task<List<SessionFrame>> getAllBySessionIdAndPageId(int sessionId, int PageId)
        {
            string jsonReq = "";
            HttpResponseMessage response = await client.GetAsync(baseUrl + "/sessionFrame/getAllBySessionIdAndPageId/" + sessionId + "/" + PageId);
            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceSessionFramesFromJSON(jsonReq);
        }

        public List<SessionFrame> getAllBySessionAndPage(int sessionId, int PageId)
        {
            var request = WebRequest.Create(baseUrl + "/sessionFrame/getAllBySessionIdAndPageId/" + sessionId + "/" + PageId);
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParceSessionFramesFromJSON(jsonOut);
            }

        }
        public List<SessionFrame> getAllByPage(int PageId)
        {
            var request = WebRequest.Create(baseUrl + "/sessionFrame/getByPage/"+PageId);
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                if (response.StatusCode == HttpStatusCode.NoContent)
                {
                    return new List<SessionFrame>();
                }
                return ParceSessionFramesFromJSON(jsonOut);
            }

        }

        ////////////////////////////////////////////////////////////////////////////////////Trayectoryes
        public async Task<List<Traectory>> getTraectories(int FrameId)
        {
            string jsonReq = "";
            HttpResponseMessage response = await client.GetAsync(baseUrl + "/trajectory/get/"  + FrameId);
            if (response.IsSuccessStatusCode)
            {
                jsonReq = "" + await response.Content.ReadAsStringAsync();
            }

            return ParceTraectoryes(jsonReq);
        }

        public List<Traectory> getTraectoriesFromFrame(long FrameId)
        {
            var request = WebRequest.Create(baseUrl + "/trajectory/get/" + FrameId);
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));

            request.Method = "GET";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;

            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                string jsonOut = reader.ReadToEnd();
                return ParceTraectoryes(jsonOut);
            }
        }
        public void deletePage(int id)
        {
            var request = WebRequest.Create(baseUrl + "/page/" + id);
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{login}:{password}"));
            request.Method = "DELETE";
            HttpWebResponse response = request.GetResponse() as HttpWebResponse;
        }



        ////////////////////////////////////////////////////////////////////////////////////Support functions
        private User ParceUserFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(User));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((User)jsonFormatter.ReadObject(stream));
        }

        private List<User> ParceUsersFromJSON(string jsonReq) {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<User>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((List<User>)jsonFormatter.ReadObject(stream));
        }

        private static string UserToJSON(User user)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(User));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, user);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            if (user.gender) {
                ret = ret.Replace("gender: True", "gender: true");
            }else
            {
                ret = ret.Replace("gender: False", "gender: false");
            }

            return ret;
        }

        private List<Session> ParceSessionsFromJSON(string jsonReq)
        {
            if (jsonReq == "")
                return new List<Session>();
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Session>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((List<Session>)jsonFormatter.ReadObject(stream));
        }
        
        private Session ParceSessionFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Session));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((Session)jsonFormatter.ReadObject(stream));
        }


        
        private string SessionToJSON(Session session)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Session));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, session);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            return ret;
        }

        private string SiteToJSON(Site site)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(Site));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, site);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            return ret;
        }
        private string PageToJSON(PageDto page)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(PageDto));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, page);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            return ret;
        }

        private string FrameTraectoryesToJSON(List<FrameTraectoryes> ft)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<FrameTraectoryes>));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, ft);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            return ret;
        }
        
        private List<Site> ParceSitesFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Site>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));
            if (stream.Length == 0)
            {
                return new List<Site>();
            }

            return ((List<Site>)jsonFormatter.ReadObject(stream));
        }

        private string baseUrlToJSON(urlDto url)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(urlDto));

            Stream stream = new MemoryStream();
            jsonFormatter.WriteObject(stream, url);
            stream.Position = 0;
            var sr = new StreamReader(stream);

            var ret = sr.ReadLine();

            return ret;
        }
        
        private PageDto ParcePageDtoFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(PageDto));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((PageDto)jsonFormatter.ReadObject(stream));
        }
        private List<PageDto> ParcePagesDtoFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<PageDto>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((List<PageDto>)jsonFormatter.ReadObject(stream));
        }

        private List<SessionFrame> ParceSessionFramesFromJSON(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<SessionFrame>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((List<SessionFrame>)jsonFormatter.ReadObject(stream));
        }

        private List<Traectory> ParceTraectoryes(string jsonReq)
        {
            DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(List<Traectory>));
            Stream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonReq));

            return ((List<Traectory>)jsonFormatter.ReadObject(stream));
        }
    }
}
