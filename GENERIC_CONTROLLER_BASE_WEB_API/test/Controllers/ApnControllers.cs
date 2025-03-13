using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Controllers
{
    public class ApnNameController : BaseController<ApnName, ApnNameDto, CreateApnNameDto, UpdateApnNameDto>
    {
        public ApnNameController(IService<ApnName, ApnNameDto, CreateApnNameDto, UpdateApnNameDto> service)
            : base(service)
        {
        }
    }

    public class ApnPasswordController : BaseController<ApnPassword, ApnPasswordDto, CreateApnPasswordDto, UpdateApnPasswordDto>
    {
        public ApnPasswordController(IService<ApnPassword, ApnPasswordDto, CreateApnPasswordDto, UpdateApnPasswordDto> service)
            : base(service)
        {
        }
    }

    public class ApnAddressController : BaseController<ApnAddress, ApnAddressDto, CreateApnAddressDto, UpdateApnAddressDto>
    {
        public ApnAddressController(IService<ApnAddress, ApnAddressDto, CreateApnAddressDto, UpdateApnAddressDto> service)
            : base(service)
        {
        }
    }
} 