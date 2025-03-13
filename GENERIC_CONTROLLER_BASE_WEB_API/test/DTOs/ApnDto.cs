namespace test.DTOs
{
    public class ApnNameDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ApnPasswordDto
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class ApnAddressDto
    {
        public int Id { get; set; }
        public string Address { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }

    public class CreateApnNameDto
    {
        public string Name { get; set; }
    }

    public class CreateApnPasswordDto
    {
        public string Password { get; set; }
    }

    public class CreateApnAddressDto
    {
        public string Address { get; set; }
    }

    public class UpdateApnNameDto : CreateApnNameDto
    {
        public int Id { get; set; }
    }

    public class UpdateApnPasswordDto : CreateApnPasswordDto
    {
        public int Id { get; set; }
    }

    public class UpdateApnAddressDto : CreateApnAddressDto
    {
        public int Id { get; set; }
    }
} 