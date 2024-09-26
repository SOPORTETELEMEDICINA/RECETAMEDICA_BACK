using RMD.Extensions.Vidal.BySideEffect;
using RMD.Interface.Vidal;
using RMD.Models.Vidal.BySideEffect;

namespace RMD.Service.Vidal.BySideEffect
{
    public class SideEffectService : ISideEffectService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _appId;
        private readonly string _appKey;
        private readonly string _startPage;
        private readonly string _pageSize;

        public SideEffectService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _baseUrl = configuration["VidalApi:BaseUrl"] ?? throw new ArgumentNullException(nameof(_baseUrl));
            _appId = configuration["VidalApi:AppId"] ?? throw new ArgumentNullException(nameof(_appId));
            _appKey = configuration["VidalApi:AppKey"] ?? throw new ArgumentNullException(nameof(_appKey));
            _pageSize = configuration["VidalApi:SizePage"] ?? throw new ArgumentNullException(nameof(_pageSize));
            _startPage = configuration["VidalApi:StartPage"] ?? throw new ArgumentNullException(nameof(_startPage));
        }

        public async Task<SideEffect> GetSideEffectByIdAsync(int id)
        {
            var requestUrl = $"{_baseUrl}/side-effect/{id}?app_id={_appId}&app_key={_appKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            response.EnsureSuccessStatusCode();

            var xmlContent = await response.Content.ReadAsStringAsync();
            var sideEffect = xmlContent.ParseSideEffectXml();

            return sideEffect;
        }
    }
}
