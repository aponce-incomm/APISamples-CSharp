using System;
using System.Xml.Serialization;

namespace TSG_API_Client
{
    [XmlRootAttribute(ElementName = "transaction", IsNullable = false)]
    public class transaction_request
    {
        public String api_key;
        public String type;
        public String card;
        public String csc;
        public String exp_date;
        public String amount;
        public String avs_address;
        public String avs_zip;
        public String purchase_order;
        public String invoice;
        public String email;
        public String customer_id;
        public String order_number;
        public String client_ip;
        public String description;
        public String comments;
        public shipping shipping;
        public billing billing;
    }
    
    [XmlRootAttribute(ElementName = "transaction", IsNullable = false)]
    public class transaction_response
    {
        public String id;
        public String result;
        public String display_message;
        public String result_code;
        public String avs_result_code;
        public String csc_result_code;
        public String error_code;
        public String authorization_code;
    }

    public class shipping
    {
        public String first_name;
        public String last_name;
        public String company;
        public String street;
        public String street2;
        public String city;
        public String state;
        public String zip;
        public String country;
        public String phone;
    }

    public class billing
    {
        public String first_name;
        public String last_name;
        public String company;
        public String street;
        public String street2;
        public String city;
        public String state;
        public String zip;
        public String country;
        public String phone;
    }
}
