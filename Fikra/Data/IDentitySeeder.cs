﻿using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace SparkLink.Data
{
    public static class IDentitySeeder
    {
        public async static Task seedRoles(RoleManager<IdentityRole> roleManager)
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

        }
    }
}
