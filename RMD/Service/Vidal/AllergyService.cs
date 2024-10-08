﻿using RMD.Extensions.Vidal;
using RMD.Interface.Vidal;
using RMD.Models.Vidal;
using System.Diagnostics;

namespace RMD.Service.Vidal 
{ 
    public class AllergyService : IAllergyService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public AllergyService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));

        }

        public async Task<List<Allergy>> GetAllergiesByName(string name)
        {
            var allergyEntries = new List<Allergy>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/allergies?q={name}&start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var allergyPage = xmlContent.ParseAllergyXmlToList();

                    // Obtener información de paginación
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");

                    allergyEntries.AddRange(allergyPage);

                    startPage++;
                } while (allergyEntries.Count < totalResults);

                Debug.WriteLine("Procesamiento de alergias completo.");
                return allergyEntries;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error inesperado: {ex.Message}");
                throw new Exception("Ha ocurrido un error inesperado. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<List<Allergy>> GetAllAllergiesAsync()
        {
            var allergyEntries = new List<Allergy>();
            int startPage = int.Parse(_startPage);
            int pageSize = int.Parse(_pageSize);
            var totalResults = 0;

            try
            {
                do
                {
                    var requestUrl = $"{_baseUrl}/allergies?start-page={startPage}&page-size={pageSize}&app_id={_appId}&app_key={_appKey}";
                    var response = await _httpClient.GetAsync(requestUrl);

                    response.EnsureSuccessStatusCode();

                    var xmlContent = await response.Content.ReadAsStringAsync();
                    var allergiesPage = xmlContent.ParseAllergiesXml();

                    // Obtener información de paginación
                    var startIndex = xmlContent.GetOpenSearchValue<int>("startIndex");
                    totalResults = xmlContent.GetOpenSearchValue<int>("totalResults");
                    var itemsPerPage = xmlContent.GetOpenSearchValue<int>("itemsPerPage");

                    allergyEntries.AddRange(allergiesPage);

                    startPage++;
                } while (allergyEntries.Count < totalResults);

                return allergyEntries;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw new Exception("Error al obtener alergias. Por favor, intente de nuevo más tarde.");
            }
        }

        public async Task<Allergy> GetAllergyByIdAsync(int allergyId)
        {
            var requestUrl = $"{_baseUrl}/allergy/{allergyId}?app_id={_appId}&app_key={_appKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                return xmlContent.ParseAllergyXml();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving allergy by ID: {ex.Message}");
            }
        }

        public async Task<List<AllergyMolecule>> GetMoleculesByAllergyIdAsync(int allergyId)
        {
            var requestUrl = $"{_baseUrl}/allergy/{allergyId}/molecules?app_id={_appId}&app_key={_appKey}";

            try
            {
                var response = await _httpClient.GetAsync(requestUrl);
                response.EnsureSuccessStatusCode();

                var xmlContent = await response.Content.ReadAsStringAsync();
                return xmlContent.ParseAllergyMoleculesXml();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving molecules for allergy ID: {ex.Message}");
            }
        }
    }

}
