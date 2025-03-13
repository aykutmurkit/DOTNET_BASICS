namespace test.DTOs
{
    public class DeviceDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public string ApnName { get; set; }
        public string ApnPassword { get; set; }
        public string ApnAddress { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class DeviceDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int ApnNameId { get; set; }
        public int ApnPasswordId { get; set; }
        public int ApnAddressId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateDeviceDto
    {
        public string Name { get; set; }
        public string SerialNumber { get; set; }
        public int ApnNameId { get; set; }
        public int ApnPasswordId { get; set; }
        public int ApnAddressId { get; set; }
    }

    public class UpdateDeviceDto : CreateDeviceDto
    {
        public int Id { get; set; }
    }
} 