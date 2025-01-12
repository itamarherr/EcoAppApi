namespace EcoAppApi.Calculations
{
    public class Customer
    {
       public virtual int GetOrderByName(string Name)
        {
            if (string.IsNullOrEmpty(Name))
            {
               throw new ArgumentException("Hello");
            }
            return 100;
        }
        public int Age => 41;

        public string GetFullName(string firstName, string LastName)
        {
            return $"{firstName} {LastName}";
        }
    }
    public class LoyalCustomer : Customer
    {
        public int Discount
        {
            get;
            set;
        }
        public LoyalCustomer()
        {
            Discount = 20;
        }
        public override int GetOrderByName(string name)
        {
            return 101;
        }
      
    }
    public static class CustomerFactory
    {
        public static Customer CreateCustomerInsance(int ordrCount)
        {
            if(ordrCount <= 100)
                return new Customer();
            else
                return new LoyalCustomer(); 
        }
       
    }
}
