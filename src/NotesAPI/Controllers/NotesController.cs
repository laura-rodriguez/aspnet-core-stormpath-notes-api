using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Stormpath.SDK.Account;
using NotesAPI.Services;
using NotesAPI.Models;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace NotesAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class NotesController : Controller
    {
        private AccountService _accountService;
        private IAccount _account;

        public NotesController(IAccount account, AccountService accountService)
        {
            _account = account;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<string> Get()
        {
            return await _accountService.GetUserNotes(_account);
        }

        [HttpPost]
        public async Task Post([FromBody]UpdateNoteRequest newNoteRequest)
        {
            await _accountService.UpdateUserNotes(_account, newNoteRequest.Notes);
        }
    }
}
