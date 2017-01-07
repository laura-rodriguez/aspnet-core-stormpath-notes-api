using Stormpath.SDK;
using Stormpath.SDK.Account;
using Stormpath.SDK.Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;

namespace NotesAPI.Services
{
    public class AccountService
    {
        private readonly IApplication stormpathApplication;
        public static readonly string NOTES_CUSTOM_DATA_KEY = "notes";

        public AccountService(IApplication stormpathApplication)
        {
            this.stormpathApplication = stormpathApplication;
        }

        private async Task<IAccount> GetUserAccount(IIdentity userIdentity)
        {
            return await stormpathApplication.GetAccounts().Where(x => x.Email == userIdentity.Name).FirstOrDefaultAsync();
        }

        public async Task<string> GetUserNotes(IAccount userAccount)
        {
            var accountCustomData = await userAccount.GetCustomDataAsync();

            // If the user doesn't have the Notes custom data created yet, return empty
            return (accountCustomData[NOTES_CUSTOM_DATA_KEY] ?? String.Empty).ToString();
        }

        public async Task UpdateUserNotes(IAccount userAccount, string value)
        {
            userAccount.CustomData.Put(new KeyValuePair<string, object>(NOTES_CUSTOM_DATA_KEY, value));

            await userAccount.SaveAsync();
        }
    }
}
