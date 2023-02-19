using HITteamBot.Repository.Entities.Base;
using HITteamBot.Repository.Entities.Characters;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.Helpers.Characters
{
    public class CharactersHelper
    {
        public ResponseData<string> CreateNewCharacter(RequestData<Character> requestData)
        {
            ResponseData<string> responseData = new ResponseData<string>();
            try
            {
                Character newCharacter = requestData.Data;
                using (DatabaseContext dbContext = new DatabaseContext())
                {

                }
            }
            catch (Exception ex)
            {
                responseData.IsError = true;
                responseData.ErrorText = ex.Message;
            }
            return responseData;
        }
    }
}
