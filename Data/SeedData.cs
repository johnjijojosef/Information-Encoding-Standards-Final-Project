using ContactManager.Authorization;
using ContactManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

//dotnet aspnet-codegenerator razorpage -m Contact -dc ApplicationDbContext -outDir Pages\Contacts --referenceScriptLibraries
namespace ContactManager.Data
{
    public static class SeedData
    {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        #region snippet_Initialize
        public static async Task Initialize(IServiceProvider serviceProvider, string testUserPw)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                // For sample purposes seed both with the same password.
                // Password is set with the following:
                // dotnet user-secrets set SeedUserPW <pw>
                // The admin user can do anything

                var adminID = await EnsureUser(serviceProvider, testUserPw, "admin@mygeorgian.com");
                await EnsureRole(serviceProvider, adminID, Constants.ContactAdministratorsRole);

                // allowed user can create and edit contacts that they create
                var managerID = await EnsureUser(serviceProvider, testUserPw, "manager@mygeorgian.com");
                await EnsureRole(serviceProvider, managerID, Constants.ContactManagersRole);

                SeedDB(context, adminID);
            }
        }

        private static async Task<string> EnsureUser(IServiceProvider serviceProvider,
                                                    string testUserPw, string UserName)
        {
            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            var user = await userManager.FindByNameAsync(UserName);
            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = UserName,
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(user, testUserPw);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private static async Task<IdentityResult> EnsureRole(IServiceProvider serviceProvider,
                                                                      string uid, string role)
        {
            var roleManager = serviceProvider.GetService<RoleManager<IdentityRole>>();

            if (roleManager == null)
            {
                throw new Exception("roleManager null");
            }

            IdentityResult IR;
            if (!await roleManager.RoleExistsAsync(role))
            {
                IR = await roleManager.CreateAsync(new IdentityRole(role));
            }

            var userManager = serviceProvider.GetService<UserManager<IdentityUser>>();

            //if (userManager == null)
            //{
            //    throw new Exception("userManager is null");
            //}

            var user = await userManager.FindByIdAsync(uid);

            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            IR = await userManager.AddToRoleAsync(user, role);

            return IR;
        }
        #endregion
        #region snippet1
        public static void SeedDB(ApplicationDbContext context, string adminID)
        {
            if (context.Contact.Any())
            {
                return;   // DB has been seeded
            }

            context.Contact.AddRange(
            #region snippet_Contact
                new Contact
                {
                    Name = "Jason",
                    Address = "194 Bayfield St",
                    City = "Barrie",
                    State = "ON",
                    Zip = "P1G J91",
                    Email = "jason.david@mygeorgian.ca",
                    Status = ContactStatus.Approved,
                    OwnerID = adminID
                },
            #endregion
            #endregion
                new Contact
                {
                    Name = "Laxmi",
                    Address = "12 Ricebrough Ave",
                    City = "Markam",
                    State = "ON",
                    Zip = "H1M 9L4",
                    Email = "laxmiramshankar.kanojia@mygeorgian.ca",
                    Status = ContactStatus.Approved,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Diljot",
                    Address = "18 Lovett Lane",
                    City = "Guelph",
                    State = "ON",
                    Zip = "N1G 4H9",
                    Email = "diljot.kaur@mygeorgian.ca",
                    Status = ContactStatus.Approved,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Shabista",
                    Address = "57 Steel Street",
                    City = "Barrie",
                    State = "ON",
                    Zip = "L31 9H3",
                    Email = "shabistayaseen.patne@mygeorgian.cam",
                    Status = ContactStatus.Submitted,
                    OwnerID = adminID
                },
                new Contact
                {
                    Name = "Jijo",
                    Address = "18 Arthur Ave",
                    City = "Barrie",
                    State = "ON",
                    Zip = "L4M 6H",
                    Email = "jijojohn.joseph@mygeorgian.ca",
                    Status = ContactStatus.Rejected,
                    OwnerID = adminID
                }
             );
            context.SaveChanges();
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
