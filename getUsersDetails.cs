using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Group = Microsoft.Graph.Group;

namespace MSGraphAPI
{
    class getUsersDetails
    {
        private const string V = "\n";
        private const int V1 = 2;
        private static string clientId = "XXXXXXX";


        private static string tenantID = "XXXXXXXXXXXX";


        private static string objectId = "XXXXXX";


        private static string clientSecret = "XXXXXX";

        static async System.Threading.Tasks.Task Main(string[] args)
        {



            string strToCheck = "+1 2009008000";
            strToCheck = strToCheck.Replace("+", String.Empty).Replace("-", String.Empty).Replace(" ", String.Empty);
            Console.WriteLine(strToCheck);

            string name = "Global Admin";

            Console.WriteLine(name.Contains("al"));



            string pattern = ".*IT Operations.*";

            Console.WriteLine(Regex.IsMatch(name, pattern, RegexOptions.IgnoreCase));

            //string pattern = "9437";

            //Regex rgx = new Regex(pattern);

            // Console.WriteLine(Regex.IsMatch(strToCheck, pattern, RegexOptions.IgnoreCase));

                int Flag = 0;
            var tenantId = "XXXXXX.onmicrosoft.com";

            // The client ID of the app registered in Azure AD
            var clientId = "XXXXX";

            // *Never* include client secrets in source code!
            var clientSecret = "X"; // Or some other secure place.

            // The app registration should be configured to require access to permissions
            // sufficient for the Microsoft Graph API calls the app will be making, and
            
            
            // those permissions should be granted by a tenant administrator.
            // var scopes = new string[] {"https://graph.microsoft.com/.default"};
            var scopes = new string[] { "https://graph.microsoft.com/.default"   };
            //var scopes = new string[] { "https://graph.microsoft.com/User.ReadWrite.All"};


            // Configure the MSAL client as a confidential client
            var confidentialClient = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithAuthority($"https://login.microsoftonline.com/XXXXXX.onmicrosoft.com/v2.0")
                .WithClientSecret(clientSecret)
                .Build();

            // Build the Microsoft Graph client. As the authentication provider, set an async lambda
            // which uses the MSAL client to obtain an app-only access token to Microsoft Graph,
            // and inserts this access token in the Authorization header of each API request. 
            GraphServiceClient graphServiceClient =
                new GraphServiceClient(new DelegateAuthenticationProvider(async (requestMessage) => {

        // Retrieve an access token for Microsoft Graph (gets a fresh token if needed).
        var authResult = await confidentialClient
            .AcquireTokenForClient(scopes)
            .ExecuteAsync();

        // Add the access token in the Authorization header of the API request.
        requestMessage.Headers.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                })
                );

            // Make a Microsoft Graph API query and find all users in Active directory in a company

            var users = await graphServiceClient.Users.Request().GetAsync();

            //and condition based user search

          var users4 = await graphServiceClient.Users
        .Request()
        .Filter("startswith(displayName,'Rob') and startswith(UserPrincipalName ,'Thomas')")
        .Select(e => new {
            e.DisplayName,
            e.GivenName,
            e.PostalCode
        })
        .GetAsync();



            //Read file lines in list 

            List<string> allLinesText = System.IO.File.ReadAllLines("departmentList.txt").ToList();



            var userFilter = users.Select(u =>   true ? string.Join(", ", u.BusinessPhones) : u.DisplayName).Where(condit => condit.Contains("Chuck")).ToList();


            userFilter.ForEach(x => Console.WriteLine(x));


            var condition = "test";

            int num = 0;

            string departmentName = "Account %26 Finance";
            //Find Peoples in deprtment who satisfy below condition for department
            var departmentPeoples = await graphServiceClient.Users.Request().Filter($"department eq '{departmentName}'").Select(u => new {
                u.DisplayName,
                u.MobilePhone,
                u.BusinessPhones,
                u.UserPrincipalName
            }).GetAsync();

            foreach (User depPeople in departmentPeoples)
            {

                Console.WriteLine(depPeople.UserPrincipalName);

            }




            string searchString = "user@company.com";

            do
            {
                        foreach (User user in users)
                        {


                    if (num == 1)
                    {
                        condition = string.Join(", ", user.BusinessPhones);
                    }
                    else
                    {
                        condition = user.UserPrincipalName;
                        
                        
                      
                    }

     
                    if (user.DisplayName.Contains("Robert"))
                    {

                        Console.WriteLine(user.DisplayName);
                        Console.WriteLine(user.MobilePhone);
                        Console.WriteLine(string.Join(", ", user.BusinessPhones));
                        Console.WriteLine(user.JobTitle);
                        Console.WriteLine(user.JoinedTeams);
                        Console.WriteLine(user.UserPrincipalName);



                        var groupIds = await graphServiceClient.Users[user.UserPrincipalName].GetMemberGroups(false).Request().PostAsync();

                        var types = new List<string>() { "group" };
                        var groups = await graphServiceClient.DirectoryObjects
                                        .GetByIds(groupIds, types)
                                        .Request()
                                        .PostAsync();



                        foreach(Microsoft.Graph.Group group in groups)
                        {
                            //Console.WriteLine(group.DisplayName);

                            if(group.DisplayName.Equals("Procount_db_RW"))
                            {

              

                            
                                
                                var members = await graphServiceClient.Groups[group.Id].Members
        .Request()
        .GetAsync();

                                foreach (User mem in members)

                                {

                                    string dispName = mem.DisplayName;

                                    var users2 = await graphServiceClient.Users
    .Request()
    .Filter($"startswith(displayName,'{dispName}')")
    .Select(u => new {
        u.DisplayName,
        u.MobilePhone,
        u.UserPrincipalName
    })
    .GetAsync();
                                    Console.WriteLine(mem.DisplayName);
                                }
                                
                            }

                            // Console.WriteLine(group.GroupTypes);
                        }

                        try
                        {
                            // var directoryObject = await graphServiceClient.Users[user.UserPrincipalName].Manager.Request().GetAsync();
                            var Peoples = await graphServiceClient.Users[user.Id].People.Request().GetAsync();

                            do
                            {
                                foreach (Person People in Peoples)
                                {
                                    if (People.DisplayName.Contains("Benny"))
                                    {
                                       Console.WriteLine(People.Department);
                                    }
                                }
                            } while (Peoples.NextPageRequest != null && (Peoples = await Peoples.NextPageRequest.GetAsync()).Count > 0);
                            // var directoryObject = await graphServiceClient.Users["{id|userPrincipalName}"].Department.Request().GetAsync();

                           // var apiResponse = await directoryObject.Content.ReadAsStringAsync();
                           // var data = JsonConvert.DeserializeObject<jsonModel>(apiResponse);


                        }
                        catch
                        {
                            continue;
                        }
                       



                    }

                  
                    // Console.WriteLine($"{user.Id}");
                    Flag++;
                        }
                    }
                    while (users.NextPageRequest != null && (users = await users.NextPageRequest.GetAsync()).Count > 0);

                    Console.WriteLine("------");
                
           
            Console.WriteLine(Flag);
        }
    }
}
