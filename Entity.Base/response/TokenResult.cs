using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.Base
{
  public  class TokenResult
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string scope { get; set; }
    }
    public class UserRoleResult
    {
        public int id { get; set; }
        public DateTime? created_at { get; set; }
        public DateTime? updated_at { get; set; }
        public DateTime? deleted_at { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public List<string> claims { get; set; }

    }
}
