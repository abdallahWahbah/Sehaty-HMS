namespace Sehaty.Application.Services
{
    public class OpenAIService : IOpenAIService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;

        public OpenAIService(IUnitOfWork unitOfWork, IHttpClientFactory httpClientFactory, IConfiguration config, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
            _httpClient = httpClientFactory.CreateClient();
            _openAiApiKey = config["OpenAI:ApiKey"] ?? Environment.GetEnvironmentVariable("ApiKey");
        }

        public async Task<Result<PrescriptionAnalysisResponseDto>> AnalyzePrescriptionAsync(int prescriptionId)
        {
            var spec = new PrescriptionSpecifications(prescriptionId);
            var prescription = await _unitOfWork.Repository<Prescription>().GetByIdWithSpecAsync(spec);

            if (prescription == null)
                return Result<PrescriptionAnalysisResponseDto>.Failure(ErrorType.NotFound, "Prescription not found");

            string prompt = BuildAnalysisPrompt(prescription);

            var result = await CallOpenAiAsync(prompt);
            if (!result.IsSuccess)
                return Result<PrescriptionAnalysisResponseDto>.Failure(result.ErrorType, result.Error);

            string aiResponse = result.Data;
            var analysisResponse = mapper.Map<PrescriptionAnalysisResponseDto>(prescription);
            analysisResponse.AnalysisResult = aiResponse;
            analysisResponse.MedicationsAnalysis = ParseMedicationsFromAI(aiResponse, prescription);

            //var analysisResponse = new PrescriptionAnalysisResponseDto
            //{
            //    PrescriptionId = prescription.Id,
            //    PatientName = prescription.Patient?.FirstName + " " + prescription.Patient?.LastName ?? "غير متوفر",
            //    DoctorName = prescription.Doctor?.FirstName + " " + prescription.Doctor?.LastName ?? "غير متوفر",
            //    DateIssued = prescription.DateIssued,
            //    AnalysisResult = aiResponse,
            //    MedicationsAnalysis = ParseMedicationsFromAI(aiResponse, prescription),
            //    GeneralInstructions = prescription.SpecialInstructions ?? "لا توجد تعليمات خاصة"
            //};

            return Result<PrescriptionAnalysisResponseDto>.Success(analysisResponse);
        }

        private string BuildAnalysisPrompt(Prescription prescription)
        {
            var sb = new StringBuilder();


            sb.AppendLine("أنت مساعد طبي ذكي. قم بتحليل الروشتة التالية وشرحها للمريض بطريقة مبسطة وواضحة.");
            sb.AppendLine($"اسم المريض: {prescription.Patient?.FirstName + "" + prescription.Patient?.LastName}");
            sb.AppendLine($"اسم الدكتور: {prescription.Doctor?.FirstName + "" + prescription.Doctor?.LastName}");
            sb.AppendLine($"تاريخ الروشتة: {prescription.DateIssued:dd/MM/yyyy}");
            sb.AppendLine("\n--- الأدوية الموصوفة ---");

            foreach (var med in prescription.Medications)
            {
                sb.AppendLine($"\n{med.Medication?.Name ?? "Unknown"}:");
                sb.AppendLine($"  - الجرعة: {med.Dosage}");
                sb.AppendLine($"  - التكرار: {med.Frequency}");
                sb.AppendLine($"  - المدة: {med.Duration}");
            }

            if (!string.IsNullOrEmpty(prescription.SpecialInstructions))
            {
                sb.AppendLine($"\nتعليمات خاصة: {prescription.SpecialInstructions}");
            }
            sb.AppendLine("\n\nمطلوب منك:");
            sb.AppendLine("1. شرح كل دواء: ليه بياخده، إزاي ياخده، امتى ياخده");
            sb.AppendLine("2. الآثار الجانبية المحتملة لكل دواء");
            sb.AppendLine("3. أي تحذيرات مهمة");
            sb.AppendLine("4. نصائح عامة للمريض");
            sb.AppendLine("\nيرجى الرد باللغة العربية بأسلوب واضح وسهل الفهم.");

            return sb.ToString();
        }

        private async Task<Result<string>> CallOpenAiAsync(string prompt)
        {
            var requestBody = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                    new { role = "system", content = "أنت مساعد طبي متخصص في شرح الروشتات الطبية للمرضى بطريقة مبسطة." },
                    new { role = "user", content = prompt }
                },
                temperature = 0.7,
                max_tokens = 2000
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_openAiApiKey}");

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return Result<string>.Failure(ErrorType.BadRequest, $"OpenAI API Error: {error}");
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            var jsonDoc = JsonDocument.Parse(responseJson);
            var aiMessage = jsonDoc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (aiMessage != null)
            {
                aiMessage = CleanMarkdown(aiMessage);
                return Result<string>.Success(aiMessage);
            }
            return Result<string>.Success("No response from AI");

        }

        private List<MedicationAnalysisDto> ParseMedicationsFromAI(string aiResponse, Prescription prescription)
        {
            var result = new List<MedicationAnalysisDto>();

            foreach (var med in prescription.Medications)
            {
                result.Add(new MedicationAnalysisDto
                {
                    MedicationName = med.Medication?.Name ?? "Unknown",
                    Dosage = med.Dosage,
                    Frequency = med.Frequency,
                    Duration = med.Duration
                });
            }

            return result;
        }
        private string CleanMarkdown(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            // Remove Markdown headings #### ### ##
            text = Regex.Replace(text, @"#{1,6}\s*", "");

            // Remove bold & italics symbols **, *, __, _
            text = Regex.Replace(text, @"(\*\*|\*|__|_)", "");

            // Remove extra backticks ```
            text = text.Replace("```", "");

            return text.Trim();
        }


    }
}
