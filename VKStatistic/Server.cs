using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.ComponentModel;
using Newtonsoft.Json;
namespace VKStatistic
{
    public class Message
    {
        public int id
        {
            get;
            set;
        }
        public int date
        {
            get;
            set;
        }
        public int user_id
        {
            get;
            set;
        }
        public Message()
        {
        }
    }
    public class Messages : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        //// Create the OnPropertyChanged method to raise the event
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs("_count"));
            }
        }
        public IEnumerable<Message> _items;
        public IEnumerable<Message> items
        {
            get { return _items;  }
            set { _items = value;OnPropertyChanged("items"); }
        }
        public int count
        {
            get;
            set;
        }
        public string _count
        {
            get
            {
                return _items.Count() + " из " + count;
            }
        }
        public Messages()
        {
        }






    }

    public class Dialogs
    {
        public int count 
        {
            get;
            set; 
        }
        public IEnumerable<Dialog> items
        {
            get;
            set;
        }
        public Dialogs()
        { 
        }
    }
    public class Dialog
    {
        public int user_id
        {
            get;
            set;
        }
        public string user_name
        {
            get;
            set;
        }
        public Dialog()
        {
        }
        public override string ToString()
        {
            return user_name;
        }
    }
    public class User
    {
        public int id
        {
            get;
            set;
        }
        public string first_name
        {
            get;
            set;
        }
        public string last_name
        {
            get;
            set;
        }
        public User()
        {
        }
    }

    public class Response
    {
        public object response 
        {
            get;
            set;
        }
    }
    public class Server
    {
        public static string access_token;
        static string getUrl(string method_name,string parameters) 
        {
            return "https://api.vk.com/method/"+method_name+"?"+parameters+"&v=5.2&access_token="+access_token; 
        }
        public static string GetResponse(string method, string parameters)
        {
            string url = getUrl(method, parameters);
            
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            string responseData = readStream.ReadToEnd();
            return responseData;
        }
        public static Dialogs GetDialogs()
        {
            try
            {
                string responseData = GetResponse("messages.getDialogs", "");
                Response o = JsonConvert.DeserializeObject<Response>(responseData);
                Dialogs a = JsonConvert.DeserializeObject<Dialogs>(o.response.ToString());
                return a;
            }
            catch
            {
                return null;
            }
        }
        public static void DialogsUserInfo(Dialogs ds)
        {
            string usrs="";
            foreach(var d in ds.items)
            {
                usrs += d.user_id+",";
            }
            try
            {
                string responseData = GetResponse("users.get", "user_ids=" + usrs);
                Response o = JsonConvert.DeserializeObject<Response>(responseData);
                var a = JsonConvert.DeserializeObject<IEnumerable<User>>(o.response.ToString());
                foreach (var a1 in a)
                {
                    ds.items.FirstOrDefault(x => x.user_id == a1.id).user_name = a1.first_name + " " + a1.last_name;
                }
            }
            catch
            { 
            }
        }
        public static Messages GetMessages(int user_id)
        {
            string responseData = GetResponse("messages.getHistory","count=200&user_id="+user_id);
            Response o = JsonConvert.DeserializeObject<Response>(responseData);
            Messages a = JsonConvert.DeserializeObject<Messages>(o.response.ToString());
            
            return a;
        }
        public static async void DownloadAllMessages(Messages a,int user_id)
        {

            while (a.count > a.items.Count())
            {
                try
                {
                    var task = await MakeAsyncRequest("messages.getHistory", "count=200&user_id=" + user_id + "&offset=" + a.items.Count());
                    System.Threading.Thread.Sleep(100);
                    string responseData = task;
                    Response o = JsonConvert.DeserializeObject<Response>(responseData);
                    Messages an = JsonConvert.DeserializeObject<Messages>(o.response.ToString());
                    a.items = a._items.Concat(an.items);
                }
                catch(Exception exp)
                { 
                }
            }
        }

        public static Task<string> MakeAsyncRequest(string method, string parameters)
        {
            string url = getUrl(method, parameters);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
           
            Task<WebResponse> task = Task.Factory.FromAsync(
                request.BeginGetResponse,
                asyncResult => request.EndGetResponse(asyncResult),
                (object)null);

            return task.ContinueWith(t => ReadStreamFromResponse(t.Result));
        }

        private static string ReadStreamFromResponse(WebResponse response)
        {
            using (Stream responseStream = response.GetResponseStream())
            using (StreamReader sr = new StreamReader(responseStream))
            {
                //Need to return this response 
                string strContent = sr.ReadToEnd();
                return strContent;
            }
        }
    }
}
