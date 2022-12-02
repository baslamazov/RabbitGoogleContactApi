using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Newtonsoft.Json.Linq;

namespace GoogleAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoogleContactsController : ControllerBase
    {
        private readonly ILogger<GoogleContactsController> _logger;
        public UnitOfWork UnitOfWork { get; set; }

        public GoogleContactsController(IServiceProvider provider)
        {
            _logger = provider.GetService<ILogger<GoogleContactsController>>();
            UnitOfWork = provider.GetService<UnitOfWork>();
        }




        [HttpGet]
        public async Task<IActionResult> Get(string fio, string phone, string bank, string emp_oid, string place_name, string email_sd, string tabnum, string blocked, int id, string doljnost, string internal_number)
        {
            try
            {
                if (blocked != null)
                {
                    var contact = await UnitOfWork.GoogleContacts.GetWhereAsync
                        (
                        c => fio != null && c.FIO.Contains(fio) ||
                        phone != null && c.PHONE.Contains(phone) ||
                        bank != null && c.BANK.Contains(bank) ||
                        emp_oid != null && c.EMP_OID.Contains(emp_oid) ||
                        place_name !=null && c.PLACE_NAME.Contains(place_name) ||
                        email_sd != null && c.EMAIL_SD.Contains(email_sd) ||
                        tabnum != null && c.TABNUM == Convert.ToInt32(tabnum) ||
                        blocked != null && c.BLOCKED == Convert.ToBoolean(blocked) ||
                        id != 0 && c.Id == id ||
                        doljnost != null && c.DOLJNOST.Contains(doljnost) ||
                        internal_number != null && c.INTERNAL_NUMBER == Convert.ToInt32(internal_number)
                        ,
                        0, -1
                        );
                    return Ok(contact);
                }
                else
                {
                    var contact = await UnitOfWork.GoogleContacts.GetWhereAsync
                        (
                        c => fio != null && c.FIO.Contains(fio) ||
                        phone != null && c.PHONE.Contains(phone) ||
                        bank != null && c.BANK.Contains(bank) ||
                        emp_oid != null && c.EMP_OID.Contains(emp_oid) ||
                        place_name != null && c.PLACE_NAME.Contains(place_name) ||
                        email_sd != null && c.EMAIL_SD.Contains(email_sd) ||
                        tabnum != null && c.TABNUM == Convert.ToInt32(tabnum) ||
                        id != 0 && c.Id == id ||
                        doljnost != null && c.DOLJNOST.Contains(doljnost) ||
                        internal_number != null && c.INTERNAL_NUMBER == Convert.ToInt32(internal_number)
                        ,
                        0, -1
                        );
                    return Ok(contact);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Где-то плачет маленький карлик");
                _logger.Equals(ex.Message);
                return BadRequest(ex.Message);
            }
        }
        //[HttpGet]
        //public async Task<IActionResult> Get()
        //{
        //    return Ok(await UnitOfWork.GoogleContacts.GetAllAsync(0, -1));
        //}

        //[HttpGet("phone")]
        //public async Task<IActionResult> Get(string Searchphone)
        //{

        //    try
        //    {
        //        _logger.LogInformation(Searchphone);
        //        var findphone = await UnitOfWork.GoogleContacts.GetWhereAsync(c => c.PHONE.Contains(Searchphone), 0, -1);
        //        _logger.LogInformation(findphone.Count().ToString());
        //        foreach (var contact in findphone)
        //        {
        //            _logger.LogInformation(contact.ToString());
        //        }
        //        return Ok(findphone);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Equals(ex.Message);
        //        return BadRequest(ex.Message);
        //    }

        //}

        //[HttpGet("phone")]
        //public async Task<IActionResult> Get(string Searchphone)
        //{

        //    try
        //    {
        //        _logger.LogInformation(Searchphone);
        //        var findphone = await UnitOfWork.GoogleContacts.GetWhereAsync(c => c.PHONE.Contains(Searchphone), 0, -1);
        //        _logger.LogInformation(findphone.Count().ToString());
        //        foreach (var contact in findphone)
        //        {
        //            _logger.LogInformation(contact.ToString());
        //        }
        //        return Ok(findphone);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Equals(ex.Message);
        //        return BadRequest(ex.Message);
        //    }

        //}
        //public async Task<ActionResult<GoogleContactsEntity>> Get(int id)
        //{
        //    GoogleContactsEntity user = await UnitOfWork.GoogleContacts.FirstOrDefaultAsync(x => x.Id == id);
        //    Console.WriteLine("eff");
        //    Console.WriteLine(user);
        //    if (user == null)
        //        return NotFound();
        //    return new ObjectResult(user);
        //}
    }
}
