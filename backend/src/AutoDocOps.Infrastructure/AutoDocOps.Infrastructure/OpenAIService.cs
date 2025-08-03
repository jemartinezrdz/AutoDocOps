    /// <param name="httpClient">Cliente HTTP</param>
    public OpenAIService(
        ILogger<OpenAIService> logger,
        IConfiguration configuration,
        HttpClient httpClient)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

        _apiKey = configuration["OpenAI:ApiKey"] ?? throw new InvalidOperationException("OpenAI API Key no configurada");
        _model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";
        _maxTokens = int.Parse(configuration["OpenAI:MaxTokens"] ?? "4000");
        _temperature = float.Parse(configuration["OpenAI:Temperature"] ?? "0.1");
