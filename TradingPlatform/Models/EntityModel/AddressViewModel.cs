namespace TradingPlatform.Models.EntityModel
{
    public class AddressViewModel
    {
        public int Id { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string House { get; set; }
        public string AbonentBox { get; set; }
        public string ZipCode { get; set; }
        public bool IsEqualsToPostAddress { get; set; }
    }
}
