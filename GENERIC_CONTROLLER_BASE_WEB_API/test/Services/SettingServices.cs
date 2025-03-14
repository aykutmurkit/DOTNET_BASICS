using test.Core;
using test.DTOs;
using test.Entities;

namespace test.Services
{
    public class ApnNameService : BaseSettingService<ApnName, string>
    {
        public ApnNameService(IRepository<ApnName> repository) : base(repository) { }

        protected override ApnName CreateNewEntity()
        {
            return new ApnName();
        }
    }

    public class ApnPasswordService : BaseSettingService<ApnPassword, string>
    {
        public ApnPasswordService(IRepository<ApnPassword> repository) : base(repository) { }

        protected override ApnPassword CreateNewEntity()
        {
            return new ApnPassword();
        }
    }

    public class ApnAddressService : BaseSettingService<ApnAddress, string>
    {
        public ApnAddressService(IRepository<ApnAddress> repository) : base(repository) { }

        protected override ApnAddress CreateNewEntity()
        {
            return new ApnAddress();
        }
    }

    // Example of how to add a new setting service:
    /*
    public class NewSettingService : BaseSettingService<NewSetting, string>
    {
        public NewSettingService(IRepository<NewSetting> repository) : base(repository) { }

        protected override NewSetting CreateNewEntity()
        {
            return new NewSetting();
        }
    }
    */
} 