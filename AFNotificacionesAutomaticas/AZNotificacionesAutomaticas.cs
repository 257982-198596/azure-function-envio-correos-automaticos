using System;
using System.Text.Json;
using System.Text;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AFNotificacionesAutomaticas
{
    public class AZNotificacionesAutomaticas
    {
        private readonly HttpClient _httpClient;

        public AZNotificacionesAutomaticas(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [Function("PostToApi")]
        public async Task Run([TimerTrigger("0 0 13 * * *")] TimerInfo myTimer, FunctionContext context)
        {
            // TODOS LOS DIAS A LAS 10 :00 AM
            var logger = context.GetLogger("PostToApi");
            logger.LogInformation($"Funcion ejecutada: {DateTime.UtcNow}");


            string baseUrl = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_URL");
            string apiUrl = $"{baseUrl}notificaciones/procesar-notificaciones-de-vencimientos";
            string apiKey = Environment.GetEnvironmentVariable("VITE_REACT_APP_API_KEY");


            var requestData = new
            {

            };

            var requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

            // API Key a los headers
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("ApiKey", apiKey);

            try
            {
                //post
                HttpResponseMessage response = await _httpClient.PostAsync(apiUrl, content);
                string responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    logger.LogInformation($"�xito: {responseContent}");
                }
                else
                {
                    logger.LogError($"Error: {response.StatusCode} - {responseContent}");
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Excepci�n: {ex.Message}");
            }
        }
    }
}
