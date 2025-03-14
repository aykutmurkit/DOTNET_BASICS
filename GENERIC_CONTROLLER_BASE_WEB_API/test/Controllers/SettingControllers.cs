using Microsoft.AspNetCore.Mvc;
using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Controllers
{
    // Base controller for all settings
    public abstract class BaseSettingController<TEntity, TValue> : BaseController<TEntity, SettingDto<TValue>, CreateSettingDto<TValue>, UpdateSettingDto<TValue>>
        where TEntity : BaseSetting<TValue>
    {
        protected BaseSettingController(IService<TEntity, SettingDto<TValue>, CreateSettingDto<TValue>, UpdateSettingDto<TValue>> service)
            : base(service)
        {
        }
    }

    // Specific setting controllers
    public class ApnNameController : BaseSettingController<ApnName, string>
    {
        public ApnNameController(IService<ApnName, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>> service)
            : base(service)
        {
        }
    }

    public class ApnPasswordController : BaseSettingController<ApnPassword, string>
    {
        public ApnPasswordController(IService<ApnPassword, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>> service)
            : base(service)
        {
        }
    }

    public class ApnAddressController : BaseSettingController<ApnAddress, string>
    {
        public ApnAddressController(IService<ApnAddress, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>> service)
            : base(service)
        {
        }
    }

    // Example of how to add a new setting controller:
    /*
    public class NewSettingController : BaseSettingController<NewSetting, string>
    {
        public NewSettingController(IService<NewSetting, SettingDto<string>, CreateSettingDto<string>, UpdateSettingDto<string>> service)
            : base(service)
        {
        }
    }
    */
} 