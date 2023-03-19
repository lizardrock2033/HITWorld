using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace HITteamBot.Repository.Domain.Helpers
{
    public class BaseHelper
    {
        public static D Map<S, D>(S source, D destination)
        {
            return new MapperConfiguration(cfg => cfg.CreateMap<S, D>()).CreateMapper().Map(source, destination);
        }
    }
}
