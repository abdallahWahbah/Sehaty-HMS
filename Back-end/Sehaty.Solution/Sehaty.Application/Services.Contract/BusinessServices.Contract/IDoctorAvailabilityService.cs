
namespace Sehaty.Application.Services.Contract.BusinessServices.Contract
{
    public interface IDoctorAvailabilityService
    {
        public Task AddAvailabilityAsync(CreateDoctorAvailabilityDto dto);
        public Task GenerateSlotsAsync(GenerateSlotsDto model);

        public Task<DoctorAvailableDaysDto> GetDoctorAvailableDaysAsync(int doctorId);
        public Task<GetAvailableSlotsResponseDto> GetAvailableSlotsAsync(GetAvailableSlotsRequestDto model);
        public Task<List<GetAvailableSlotsResponseDto>> GetAvailableSlotsRangeAsync(int doctorId, DateOnly startDate, DateOnly endDate);
        public Task<BookSlotResponseDto> BookSlotAsync(BookSlotDto model);
        public Task CancelSlotAsync(int slotId);
        public Task<SlotDetailsDto> GetSlotDetailsAsync(int slotId);
    }
}
