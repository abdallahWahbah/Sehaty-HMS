namespace Sehaty.Infrastructure.Data.SeedClass
{
    public static class SeedExtensions
    {
        public static async Task SeedDataAsync(this SehatyDbContext context)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter() }
            };

            var passwordHasher = new PasswordHasher<ApplicationUser>();

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                if(!context.Roles.Any())
                {
                    var rolesData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Roles.json");
                    var roles = JsonSerializer.Deserialize<List<ApplicationRole>>(rolesData,options)!;
                    context.Roles.AddRange(roles);
                    await context.SaveChangesAsync();
                }

                if(!context.Departments.Any())
                {
                    var departmentsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Departments.json");
                    var departments = JsonSerializer.Deserialize<List<Department>>(departmentsData,options)!;
                    context.Departments.AddRange(departments);
                    await context.SaveChangesAsync();
                }

                if(!context.Users.Any())
                {
                    var anonymousUser = new ApplicationUser
                    {
                        Id = 999999,
                        Email = "Anonymous@example.com",
                        UserName = "Anonymous",
                        NormalizedEmail = "ANONYMOUS@EXAMPLE.COM",
                        NormalizedUserName = "ANONYMOUS",
                        FirstName = "Anonymous",
                        LastName = "User",
                        IsActive = true,
                        PhoneNumber = "+200000000000",
                        CreatedAt = DateTime.MinValue,
                        LanguagePreference = LanguagePreferenceEnum.Arabic
                    };
                    anonymousUser.PasswordHash = passwordHasher.HashPassword(anonymousUser,"P@ssw0rd");

                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users ON");
                    context.Users.Add(anonymousUser);
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Users OFF");

                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Users', RESEED, 0)");

                    var usersData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Users.json");
                    var users = JsonSerializer.Deserialize<List<ApplicationUser>>(usersData,options)!;
                    foreach(var user in users)
                    {
                        user.Id = 0;
                        user.NormalizedUserName = user.UserName.ToUpper();
                        user.NormalizedEmail = user.Email.ToUpper();
                        user.SecurityStamp = Guid.NewGuid().ToString();
                        user.PasswordHash = passwordHasher.HashPassword(user,"P@ssw0rd");
                    }
                    context.Users.AddRange(users);
                    await context.SaveChangesAsync();
                }

                if(!context.Patients.Any())
                {
                    var anonymousPatient = new Patient
                    {
                        Id = 999999,
                        Patient_Id = "PT-2025-0000",
                        FirstName = "Anonymous",
                        LastName = "Patient",
                        DateOfBirth = DateTime.MinValue,
                        Gender = "Unknown",
                        NationalId = "Unknown",
                        BloodType = "Unk",
                        Allergies = "Unknown",
                        ChrinicConditions = "Unknown",
                        Address = "Unknown",
                        EmergencyContactName = "Unknown",
                        EmergencyContactPhone = "Unknown",
                        Status = PatientStatus.Active,
                        RegistrationDate = DateTime.MinValue,
                        UserId = 999999
                    };

                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Patients ON");
                    context.Patients.Add(anonymousPatient);
                    await context.SaveChangesAsync();
                    await context.Database.ExecuteSqlRawAsync("SET IDENTITY_INSERT Patients OFF");

                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Patients', RESEED, 0)");

                    var patientsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Patients.json");
                    var patients = JsonSerializer.Deserialize<List<Patient>>(patientsData,options)!;
                    context.Patients.AddRange(patients);
                    await context.SaveChangesAsync();
                }
            }

            catch(Exception)
            {
                await transaction.RollbackAsync();
                try
                {
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Users', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Patients', RESEED, 0)");
                    await context.Database.ExecuteSqlRawAsync("DBCC CHECKIDENT ('Departments', RESEED, 0)");
                }
                catch(Exception innerEx)
                {
                    Console.WriteLine($"Failed to reset identities: {innerEx.Message}");
                }

                throw;
            }

            if(!context.UserRoles.Any())
            {
                var userRolesData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/UserRoles.json");
                var userRoles = JsonSerializer.Deserialize<List<IdentityUserRole<int>>>(userRolesData,options)!;
                context.UserRoles.AddRange(userRoles);
                await context.SaveChangesAsync();
            }

            if(!context.Doctors.Any())
            {
                var doctorsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/Doctors.json");
                var doctors = JsonSerializer.Deserialize<List<Doctor>>(doctorsData,options)!;
                context.Doctors.AddRange(doctors);
                await context.SaveChangesAsync();
            }

            if(!context.MedicalRecords.Any())
            {
                var medicalRecordsData = File.ReadAllText("../Sehaty.Infrastructure/Data/SeedDataFiles/MedicalRecords.json");
                var medicalRecords = JsonSerializer.Deserialize<List<MedicalRecord>>(medicalRecordsData,options)!;
                context.MedicalRecords.AddRange(medicalRecords);
                await context.SaveChangesAsync();
            }

            await transaction.CommitAsync();
        }

    }
}
