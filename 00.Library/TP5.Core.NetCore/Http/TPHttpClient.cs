using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TP5.Core.NetCore.Http
{
    /// <summary>
    /// 使用前請引用 Json.NET
    /// </summary>
    public class TPHttpClient
    {
        public int Timeout { get; set; }
        public string BaseUrl { get; set; }

        /// <summary>
        /// 要使用的網路授權
        /// </summary>
        public ICredentials Credentials { get; set; }

        /// <summary>
        /// 針對 Response 的檢查動作
        /// </summary>
        public Action<HttpResponseMessage> ResponseCheckHandler;

        public TPHttpClient() { }

        public TPHttpClient(string baseUrl, int timeout = 120 * 1000)
        {
            this.Timeout = timeout;
            this.BaseUrl = baseUrl;
        }

        public T PostJsonRequest<T>(string methodUrl, object jsonObject, Dictionary<string, string> headers = null)
        {
            string jsonString = JsonConvert.SerializeObject(jsonObject);
            var requestUrl = $"{this.BaseUrl}/{methodUrl}";
            return this.Post<T>((client, token) => client.PostAsync(
                requestUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"), token), headers);
        }

        public T PutJsonRequest<T>(string methodUrl, object jsonObject, Dictionary<string, string> headers = null)
        {
            string jsonString = JsonConvert.SerializeObject(jsonObject);
            var requestUrl = $"{this.BaseUrl}/{methodUrl}";
            return this.Post<T>((client, token) => client.PutAsync(
                requestUrl, new StringContent(jsonString, Encoding.UTF8, "application/json"), token), headers);
        }

        public T PostRequest<T>(string methodUrl, Dictionary<string, string> headers = null)
        {
            var requestUrl = $"{this.BaseUrl}/{methodUrl}";
            return this.Post<T>((client, token) => client.SendAsync(
                new HttpRequestMessage(HttpMethod.Post, requestUrl), token), headers);
        }

        public T GetRequest<T>(string methodUrl, Dictionary<string, string> headers = null)
        {
            var requestUrl = $"{this.BaseUrl}/{methodUrl}";
            return this.Post<T>((client, token) => client.GetAsync(requestUrl, token), headers);
        }

        private T Post<T>(Func<HttpClient, CancellationToken, Task<HttpResponseMessage>> func, Dictionary<string, string> headers)
        {
            var handler = new HttpClientHandler();
            // 授權
            if (this.Credentials != null) handler.Credentials = this.Credentials;

            HttpClient client = new HttpClient(handler);
            client.BaseAddress = new Uri(this.BaseUrl);
            if (headers != null)
            {
                foreach (var key in headers.Keys)
                    client.DefaultRequestHeaders.Add(key, headers[key]);
            }

            Exception ex = null;
            HttpResponseMessage response = null;
            T result = default(T);

            CancellationTokenSource cancelToken = new CancellationTokenSource(this.Timeout); // 執行序 Timeout
            try
            {
                response = func(client, cancelToken.Token)
                   .ContinueWith<HttpResponseMessage>(r =>
                   {
                       if (r.IsCompleted && r.Status== TaskStatus.RanToCompletion)
                           return r.Result;

                       if (r.IsCanceled)
                           ex = new TimeoutException(); // 理論上不會觸發
                       else if (r.Exception != null)
                           ex = r.Exception.InnerException;
                       return null;
                   }).Result;
            }
            catch (AggregateException e) // Timeout 觸發
            {
                Exception error = e;
                do
                {
                    error = error.InnerException;
                    if (error is System.Threading.Tasks.TaskCanceledException)
                    {
                        ex = error;
                        break;
                    }

                } while (error != null);

                if (ex == null)
                    throw e;
                else
                    throw new TimeoutException(string.Empty, ex);
            }

            if (response != null)
            {
                if (this.ResponseCheckHandler == null)
                {
                    this.ResponseCheckHandler = this.DefaultStatusCodeHandler();
                }

                this.ResponseCheckHandler(response);

                string responseJsonString = response.Content.ReadAsStringAsync().Result;
                result = JsonConvert.DeserializeObject<T>(responseJsonString);
            }

            return result;
        }

        private Action<HttpResponseMessage> DefaultStatusCodeHandler()
        {
            return new Action<HttpResponseMessage>(response =>
            {
                if (!response.IsSuccessStatusCode)
                    throw new Exception(response.StatusCode.ToString());
            });
        }
    }
}
