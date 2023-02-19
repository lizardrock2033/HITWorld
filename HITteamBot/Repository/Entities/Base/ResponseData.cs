using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Base
{
    public class ResponseData<T>
    {
        public T Data { get; set; }
        public bool IsError { get; set; }
        public string ErrorText { get; set; }
    }
}
