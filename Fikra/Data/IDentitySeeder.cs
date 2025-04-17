using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SparkLink.Models.Dto;
using SparkLink.Models.Identity;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace SparkLink.Data
{
    public static class IDentitySeeder
    {
        public async static Task seedRoles(RoleManager<IdentityRole> roleManager,UserManager<ApplicationUser> userManager)
        {
            List<IdentityRole> roles = new List<IdentityRole>() { new IdentityRole("IdeaOwner"), new IdentityRole("Investor"), new IdentityRole("Admin"), new IdentityRole("Freelancer") };
            if (!roleManager.Roles.Any())
            {

                foreach (var role in roles)
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        continue;
                    }
                }

               

            }
            if (roleManager.Roles.FirstOrDefault(x => x.Name == "Admin")==null)
            {
                var result = await roleManager.CreateAsync(new IdentityRole("Admin"));
                if (result.Succeeded)
                {
                    Console.WriteLine("AdminRole has Been Created Successfully");
                    return;
                }
                return;
            }
            var  AdminUserExist = await  userManager.Users.FirstOrDefaultAsync(x=>x.UserName=="KaoudAdmin");
            if (AdminUserExist == null)
            {
                var AdminUser = new ApplicationUser()
                {
                    Email="KaoudAdmin@gmail.com",
                    UserName="KaoudAdmin",
                    Gender="Male",
                    LinkedinUrl="adminLinkedin",
                    FatherName="Ahmad",
                    Country="Syria",
                    FirstName="Mo",
                    LastName="Kaoud",
                    CompanyName="FikraCompany"
                };
                AdminUser.EmailConfirmed = true;
                var resultofcreatingadminuser=await userManager.CreateAsync(AdminUser,"Suarez.123!@#qwe");
                if (resultofcreatingadminuser.Succeeded)
                {
                    var resultAssignToAdminRole= await userManager.AddToRoleAsync(AdminUser, "Admin");
                    if (resultAssignToAdminRole.Succeeded)
                    {
                        Console.WriteLine("AssignToAdmin Role Assign Successfully");
                        return;
                    }
                    return ;
                }
            }
          
          



        }
    }
}
