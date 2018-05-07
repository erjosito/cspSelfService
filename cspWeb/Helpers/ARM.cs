using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using cspWeb.Helpers;
using Microsoft.Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Azure.Management.ResourceManager;
using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest;
using System.IO;
//using Microsoft.Azure.Management.BackupServices;
//using Microsoft.Azure.Management.BackupServices.Models;
using Microsoft.Azure.Management.RecoveryServices;
using Microsoft.Azure.Management.RecoveryServices.Models;
using Microsoft.Azure.Management.RecoveryServices.Backup;
using Microsoft.Azure.Management.RecoveryServices.Backup.Models;
using Microsoft.Azure.Management.Resources;
using Microsoft.Azure.Management.Resources.Models;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text;

namespace cspWeb.Helpers
{
    public static class ARM
    {
        public static string createResourceGroup(string customerId, string subscriptionId, string groupName, string location)
        {
            string rg = null;
            do
            {
                rg = createResourceGroupTry(customerId, subscriptionId, groupName, location);
                if (rg == null)
                {
                    Thread.Sleep(15000);
                }
            } while (rg == null);
            return rg;
        }

        public static async Task<string> createResourceGroupAsync(string customerId, string subscriptionId, string groupName, string location)
        {
            string rg = null;
            do
            {
                rg = await createResourceGroupTryAsync(customerId, subscriptionId, groupName, location);
                if (rg == null)
                {
                    Thread.Sleep(15000);
                }
            } while (rg == null);
            return rg;
        }


        private static string createResourceGroupTry (string customerId, string subscriptionId, string groupName, string location)
        {
            string token = REST.getArmToken(customerId, UserAuth: true);
            if (token != null)
            {
                var credential = new TokenCredentials(token);
                var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
                var resourceGroup = new Microsoft.Azure.Management.ResourceManager.Models.ResourceGroup { Location = location };
                try
                {
                    var rg = armClient.ResourceGroups.CreateOrUpdate(groupName, resourceGroup);
                    return rg.Id.ToString();
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private static async Task<string> createResourceGroupTryAsync(string customerId, string subscriptionId, string groupName, string location)
        {
            string token = await REST.getArmTokenAsync(customerId, UserAuth: true);
            if (token != null)
            {
                var credential = new TokenCredentials(token);
                var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
                var resourceGroup = new Microsoft.Azure.Management.ResourceManager.Models.ResourceGroup { Location = location };
                try
                {
                    var rg = await armClient.ResourceGroups.CreateOrUpdateAsync(groupName, resourceGroup);
                    return rg.Id.ToString();
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }


        public static async Task<bool> DeleteResourceGroupAsync(string customerId, string subscriptionId, string groupName)
        {
            string token = await REST.getArmTokenAsync(customerId, UserAuth: true);
            if (token != null)
            {
                var credential = new TokenCredentials(token);
                var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
                armClient.ResourceGroups.Delete(groupName);
                return true;
            }
            return false;
        }


        // Generate a random string with a given size  
        public static string RandomString(int size, bool lowerCase=true)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }

        public static void registerRS(string customerId, string subscriptionId, string rpName)
        {
            string token = REST.getArmToken(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);
            var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
            bool rpRegistered = false;
            // Check the registration state until it is Registered.
            do
            {
                var RPlist = armClient.Providers.List().ToList();
                foreach (var provider in RPlist)
                {
                    if (provider.NamespaceProperty == rpName)
                    {
                        if (provider.RegistrationState == "NotRegistered")
                        {
                            armClient.Providers.Register(rpName);
                        }
                        else
                        {
                            if (provider.RegistrationState == "Registered")
                            {
                                rpRegistered = true;
                            }
                            else
                            {
                                Thread.Sleep(5000);
                            }
                        }

                    }
                }

            } while (rpRegistered == false);
        }


        public static async Task registerRSAsync(string customerId, string subscriptionId, string rpName)
        {
            string token = await REST.getArmTokenAsync(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);
            var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };
            bool rpRegistered = false;
            // Check the registration state until it is Registered. Wait 5 seconds before each retry
            do
            {
                var RPlist = (await armClient.Providers.ListAsync()).ToList();
                foreach (var provider in RPlist)
                {
                    if (provider.NamespaceProperty == rpName)
                    {
                        if (provider.RegistrationState == "NotRegistered")
                        {
                            await armClient.Providers.RegisterAsync(rpName);
                        }
                        else
                        {
                            if (provider.RegistrationState == "Registered")
                            {
                                rpRegistered = true;
                            }
                            else
                            {
                                Thread.Sleep(5000);
                            }
                        }

                    }
                }

            } while (rpRegistered == false);
        }


        public static string createSRVault(string customerId, string subscriptionId, string groupName, string vaultName, string location)
        {
            // Do we need to register the resource provider because it is a sandbox???
            registerRS(customerId, subscriptionId, "Microsoft.RecoveryServices");
            string token = REST.getArmToken(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);
            var VaultClient = new RecoveryServicesClient(credential) { SubscriptionId = subscriptionId };
            Vault vault = new Vault()
            {
                Location = location,
                Sku = new Microsoft.Azure.Management.RecoveryServices.Models.Sku() { Name = SkuName.Standard },
                Properties = new VaultProperties()
            };
            var newVault = VaultClient.Vaults.CreateOrUpdate(groupName, vaultName, vault);
            //return VaultClient
            return newVault.Id.ToString();
        }

        public static async Task<string> createSRVaultAsync(string customerId, string subscriptionId, string groupName, string vaultName, string location)
        {
            // Do we need to register the resource provider because it is a sandbox???
            await registerRSAsync(customerId, subscriptionId, "Microsoft.RecoveryServices");
            string token = await REST.getArmTokenAsync(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);
            var VaultClient = new RecoveryServicesClient(credential) { SubscriptionId = subscriptionId };
            Vault vault = new Vault()
            {
                Location = location,
                Sku = new Microsoft.Azure.Management.RecoveryServices.Models.Sku() { Name = SkuName.Standard },
                Properties = new VaultProperties()
            };
            var newVault = await VaultClient.Vaults.CreateOrUpdateAsync(groupName, vaultName, vault);
            //return VaultClient
            return newVault.Id.ToString();
        }


        public static void getBackupCredentials(string customerId, string subscriptionId, string groupName, string vaultName)
        {
            string token = REST.getArmToken(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);
            var backupClient = new RecoveryServicesBackupClient(credential) { SubscriptionId = subscriptionId };
            //backupClient.
        }

        public static async Task<string> createVMsAsync(string customerId, string subscriptionId, string groupName, string vmName, string location)
        {
            var myTemplate = "";
            string templateUrl = "https://raw.githubusercontent.com/erjosito/AzureBlackMagic/master/vnet.json";
            var webRequest = System.Net.WebRequest.Create(templateUrl);
            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            using (var reader = new StreamReader(content))
            {
                myTemplate = reader.ReadToEnd();
            }

            string token = await REST.getArmTokenAsync(customerId, UserAuth: true);
            var credential = new TokenCredentials(token);

            var armClient = new Microsoft.Azure.Management.ResourceManager.ResourceManagementClient(credential) { SubscriptionId = subscriptionId };

            var myParameters = new Dictionary<string, Dictionary<string, object>>{
                 {"vnetName", new Dictionary<string, object> { {"value", vmName} } }
            };


            var resourceGroupResponse = await armClient.ResourceGroups.CreateOrUpdateAsync(groupName,
                        new Microsoft.Azure.Management.ResourceManager.Models.ResourceGroup { Location = location });

            var deploymentExtended = await armClient.Deployments.CreateOrUpdateAsync(groupName, vmName,
                      new Microsoft.Azure.Management.ResourceManager.Models.Deployment
                      {
                          Properties = new Microsoft.Azure.Management.ResourceManager.Models.DeploymentProperties
                          {
                              Mode = Microsoft.Azure.Management.ResourceManager.Models.DeploymentMode.Incremental,
                              Template = myTemplate,
                              Parameters = myParameters
                          }
                      });

            var hasSucceeded = deploymentExtended.Properties.ProvisioningState == "Succeeded";

            //return Dummy
            return null;
        }

        // The resource ID has this format:
        // /subscriptions/2bf8fc82-ec43-4af2-967e-a5992701ef76/resourceGroups/testRg/providers/Microsoft.RecoveryServices/vaults/testVault
        // The function takes the value after 'resourceGroups'
        public static string GetResourceGroupNameFromId (string resourceId)
        {
            String[] substrings = resourceId.Split('/');
            string result = "";
            for (int i = 0; i < substrings.Length; i = i + 1)
            {
                if (substrings[i] == "resourceGroups")
                {
                    result = substrings[i + 1];
                }
            }
            return result;
        }


        // The resource ID has this format:
        // /subscriptions/2bf8fc82-ec43-4af2-967e-a5992701ef76/resourceGroups/testRg/providers/Microsoft.RecoveryServices/vaults/testVault
        // The function takes the value after 'resourceGroups'
        public static string GetSubscriptionFromId(string resourceId)
        {
            String[] substrings = resourceId.Split('/');
            string result = "";
            for (int i = 0; i < substrings.Length; i = i + 1)
            {
                if (substrings[i] == "subscriptions")
                {
                    result = substrings[i + 1];
                }
            }
            return result;
        }



    }
}