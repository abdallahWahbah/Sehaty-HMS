using Sehaty.Core.Entities.Business_Entities.DoctorAvailabilitySlots;

namespace Sehaty.Infrastructure.Data.SeedClass
{
    public static class SeedExtensions
    {
        public static async Task SeedDataAsync(this SehatyDbContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() } // <-- important
            };
            if (context.Roles.Count() == 0)
            {
                var rolesData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Roles.json");
                var roles = JsonSerializer.Deserialize<List<ApplicationRole>>(rolesData, options);
                context.Roles.AddRange(roles!);
                await context.SaveChangesAsync();
            }

            if (context.Users.Count() == 0)
            {
                var usersData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Users.json");
                var users = JsonSerializer.Deserialize<List<ApplicationUser>>(usersData, options);
                var passwordHasher = new PasswordHasher<ApplicationUser>();
                foreach (var user in users!)
                {
                    user.NormalizedUserName = user.UserName.ToUpper();
                    user.NormalizedEmail = user.Email.ToUpper();
                    user.SecurityStamp = Guid.NewGuid().ToString();
                    user.PasswordHash = passwordHasher.HashPassword(user, "P@ssw0rd");
                }
                context.Users.AddRange(users!);
                await context.SaveChangesAsync();
            }

            if (context.UserRoles.Count() == 0)
            {
                var userRolesData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/UserRoles.json");
                var userRoles = JsonSerializer.Deserialize<List<IdentityUserRole<int>>>(userRolesData, options);
                context.UserRoles.AddRange(userRoles!);
                await context.SaveChangesAsync();
            }

            if (context.Departments.Count() == 0)
            {
                var departmentsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Departments.json");
                var departments = JsonSerializer.Deserialize<List<Department>>(departmentsData, options);
                context.Departments.AddRange(departments!);
                await context.SaveChangesAsync();
            }

            if (context.Doctors.Count() == 0)
            {
                var doctorsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Doctors.json");
                var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorsData, options);
                context.Doctors.AddRange(doctors!);
                await context.SaveChangesAsync();
            }

            if (context.Patients.Count() == 0)
            {
                var patientsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Patients.json");
                var patients = JsonSerializer.Deserialize<List<Patient>>(patientsData, options);
                context.Patients.AddRange(patients!);
                await context.SaveChangesAsync();
            }

            if (context.Appointments.Count() == 0)
            {
                var appointmentsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Appointments.json");
                var appointments = JsonSerializer.Deserialize<List<Appointment>>(appointmentsData, options);
                context.Appointments.AddRange(appointments!);
                await context.SaveChangesAsync();
            }

            if (context.MedicalRecords.Count() == 0)
            {
                var medicalRecordsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/MedicalRecords.json");
                var medicalRecords = JsonSerializer.Deserialize<List<MedicalRecord>>(medicalRecordsData, options);
                context.MedicalRecords.AddRange(medicalRecords!);
                await context.SaveChangesAsync();
            }

            if (context.Medications.Count() == 0)
            {
                var medicationsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Medications.json");
                var medications = JsonSerializer.Deserialize<List<Medication>>(medicationsData, options);
                context.Medications.AddRange(medications!);
                await context.SaveChangesAsync();
            }

            if (context.Prescriptions.Count() == 0)
            {
                var prescriptionsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Prescriptions.json");
                var prescriptions = JsonSerializer.Deserialize<List<Prescription>>(prescriptionsData, options);
                context.Prescriptions.AddRange(prescriptions!);
                await context.SaveChangesAsync();
            }

            if (context.Billings.Count() == 0)
            {
                var billingsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Billings.json");
                var billings = JsonSerializer.Deserialize<List<Billing>>(billingsData, options);
                context.Billings.AddRange(billings!);
                await context.SaveChangesAsync();
            }

            if (context.Feedbacks.Count() == 0)
            {
                var feedbacksData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Feedbacks.json");
                var feedbacks = JsonSerializer.Deserialize<List<Feedback>>(feedbacksData, options);
                context.Feedbacks.AddRange(feedbacks!);
                await context.SaveChangesAsync();
            }

            if (context.DoctorAvailabilitySlots.Count() == 0)
            {
                var slotsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/DoctorAvailabilitySlots.json");
                var slots = JsonSerializer.Deserialize<List<DoctorAvailabilitySlot>>(slotsData, options);
                context.DoctorAvailabilitySlots.AddRange(slots!);
                await context.SaveChangesAsync();
            }

            if (context.Notifications.Count() == 0)
            {
                var notificationsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Notifications.json");
                var notifications = JsonSerializer.Deserialize<List<Notification>>(notificationsData, options);
                context.Notifications.AddRange(notifications!);
                await context.SaveChangesAsync();
            }

            if (context.PrescriptionMedications.Count() == 0)
            {
                var prescriptionMedicationsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/PrescriptionMedications.json");
                var prescriptionMedications = JsonSerializer.Deserialize<List<PrescriptionMedications>>(prescriptionMedicationsData, options);
                context.PrescriptionMedications.AddRange(prescriptionMedications!);
                await context.SaveChangesAsync();
            }
        }
    }

}
