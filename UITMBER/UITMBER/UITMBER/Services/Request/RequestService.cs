﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace UITMBER.Services.Request
{
    public class RequestService : IRequestService
    {

        private HttpClient CreateHttpClient()
        {
            var httpClient = new HttpClient(GetInsecureHandler());

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (!string.IsNullOrEmpty(Settings.AccessToken))
            {
                httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer" + Settings.AccessToken);
            }

            return httpClient;

        }

        private async Task CheckResponse(HttpResponseMessage respose)
        {
            if (!respose.IsSuccessStatusCode)
            {
                var content = await respose.Content.ReadAsStringAsync();

                throw new Exception(content);
            }
        }

        public async Task<TResult> GetAsync<TResult>(string uri)
        {
            HttpClient httpClient = CreateHttpClient();
            var respose = await httpClient.GetAsync(uri);

            await CheckResponse(respose);

            var responseContent = await respose.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(responseContent);
        }



        public Task<TResult> PostAsync<TResult>(string uri, TResult data)
        {
            return PostAsync<TResult, TResult>(uri, data);
        }

        public async Task<TResult> PostAsync<TRequest, TResult>(string uri, TRequest data)
        {
            HttpClient httpClient = CreateHttpClient();

            var serializedObject = JsonConvert.SerializeObject(data);

            var respose = await httpClient.PostAsync(uri, new StringContent(serializedObject, Encoding.UTF8, "application/json"));

            await CheckResponse(respose);

            var responseContent = await respose.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResult>(responseContent);
        }

        public Task<TResult> PutAsync<TResult>(string uri, TResult data)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PutAsync<TRequest, TResult>(string uri, TRequest data)
        {
            throw new NotImplementedException();
        }

        // This method must be in a class in a platform project, even if
        // the HttpClient object is constructed in a shared project.
        public HttpClientHandler GetInsecureHandler()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain,
           errors) =>
            {
                return true;
            };
            return handler;
        }
    }
}
