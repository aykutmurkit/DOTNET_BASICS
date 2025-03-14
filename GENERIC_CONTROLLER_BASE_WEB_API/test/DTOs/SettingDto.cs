namespace test.DTOs
{
    // Base DTO classes for all settings
    public class BaseSettingDto
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class BaseCreateSettingDto
    {
    }

    public class BaseUpdateSettingDto : BaseCreateSettingDto
    {
        public int Id { get; set; }
    }

    // Generic setting DTOs
    public class SettingDto<TValue> : BaseSettingDto
    {
        public TValue Value { get; set; }
    }

    public class CreateSettingDto<TValue> : BaseCreateSettingDto
    {
        public TValue Value { get; set; }
    }

    public class UpdateSettingDto<TValue> : BaseUpdateSettingDto
    {
        public TValue Value { get; set; }
    }

    // Specific setting DTOs using the generic base
    // ApnName
    public class ApnNameDto : SettingDto<string>
    {
    }

    public class CreateApnNameDto : CreateSettingDto<string>
    {
    }

    public class UpdateApnNameDto : UpdateSettingDto<string>
    {
    }

    // ApnPassword
    public class ApnPasswordDto : SettingDto<string>
    {
    }

    public class CreateApnPasswordDto : CreateSettingDto<string>
    {
    }

    public class UpdateApnPasswordDto : UpdateSettingDto<string>
    {
    }

    // ApnAddress
    public class ApnAddressDto : SettingDto<string>
    {
    }

    public class CreateApnAddressDto : CreateSettingDto<string>
    {
    }

    public class UpdateApnAddressDto : UpdateSettingDto<string>
    {
    }

    // Example of how to add new settings:
    // public class NewSettingDto : SettingDto<string> { }
    // public class CreateNewSettingDto : CreateSettingDto<string> { }
    // public class UpdateNewSettingDto : UpdateSettingDto<string> { }
    
    // For settings with different value types:
    // public class NumericSettingDto : SettingDto<int> { }
    // public class CreateNumericSettingDto : CreateSettingDto<int> { }
    // public class UpdateNumericSettingDto : UpdateSettingDto<int> { }
} 