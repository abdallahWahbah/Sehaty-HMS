namespace Sehaty.Application.Dtos.OpenAIDto.SuggestAppointmentBySymptomsDto
{
    public class SymptomsAnalysisRequestDto
    {
        public int PatientId { get; set; }
        public string Symptoms { get; set; }
    }
}
