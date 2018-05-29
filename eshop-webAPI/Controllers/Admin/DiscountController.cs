using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/admin/discount")]
    public class DiscountController : ODataController
    {
        private readonly IDiscountRepository _discountRepository;

        public DiscountController(IDiscountRepository DiscountRepository)
        {
            _discountRepository = DiscountRepository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<AdminDiscountVM>> Get()
        {
            return await _discountRepository.GetAllValidDiscounts();
        }
    }
}