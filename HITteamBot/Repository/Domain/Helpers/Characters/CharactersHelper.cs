using HITteamBot.Repository.Entities;
using HITteamBot.Repository.Entities.Base;
using HITteamBot.Repository.Domain.Helpers;
using HITteamBot.Repository.Domain.DatabaseEntities.Characters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using HITteamBot.Repository.Entities.Characters;

namespace HITteamBot.Repository.Domain.Helpers.Characters
{
    public class CharactersHelper
    {
        public static async Task<ResponseData<Character>> CreateNewCharacter(RequestData<Users> requestData)
        {
            ResponseData<Character> responseData = new ResponseData<Character>();
            try
            {
                Users user = requestData.Data;
                await Task.Factory.StartNew(() =>
                {
                    using (DatabaseContext dbContext = new DatabaseContext())
                    {
                        using (var transaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                var special = dbContext.SPECIAL.Add(new SPECIAL()
                                {
                                    Strength = 1,
                                    Perception = 1,
                                    Endurance = 1,
                                    Charisma = 1,
                                    Intellegence = 1,
                                    Agility = 1,
                                    Luck = 1,
                                    IsSet = false
                                });
                                dbContext.SaveChanges();

                                var newCharacter = dbContext.Character.Add(new Character()
                                {
                                    Name = user.Username,
                                    Avatar = Emoji.Incognito,
                                    Experience = 0,
                                    Level = 1,
                                    SPECIALsId = special.Entity.Id,
                                    Health = 100,
                                    CurrentHealth = 100,
                                    ActionPoints = 100,
                                    CurrentAP = 100,
                                    Rads = 0,
                                    Caps = 0,
                                    IsAlive = true
                                });
                                dbContext.SaveChanges();

                                user.CharacterId = newCharacter.Entity.Id;
                                dbContext.Users.Add(user);
                                dbContext.SaveChanges();
                                transaction.Commit();
                                responseData.Data = newCharacter.Entity;
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception(ex.Message);
                            }

                        }

                    }
                });
            }
            catch (Exception ex)
            {
                responseData.IsError = true;
                responseData.ErrorText = ex.Message;
            }
            return responseData;
        }

        public static async Task<ResponseData<int>> SetCharacterStats(RequestData<SPECIAL> requestData)
        {
            ResponseData<int> responseData = new ResponseData<int>();
            try
            {
                SPECIAL newSpecials = requestData.Data;
                await Task.Factory.StartNew(() =>
                {
                    using (DatabaseContext dbContext = new DatabaseContext())
                    {
                        using (var transaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                SPECIAL special = dbContext.SPECIAL.Where(s => s.Id == newSpecials.Id).FirstOrDefault();
                                special = BaseHelper.Map(newSpecials, special);
                                dbContext.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                responseData.IsError = true;
                responseData.ErrorText = ex.Message;
            }
            return responseData;
        }

        public static async Task<ResponseData<int>> SetCharacterName(RequestData<CharacterData> requestData)
        {
            ResponseData<int> responseData = new ResponseData<int>();
            try
            {
                CharacterData characterData = requestData.Data;
                await Task.Factory.StartNew(() =>
                {
                    using (DatabaseContext dbContext = new DatabaseContext())
                    {
                        using (var transaction = dbContext.Database.BeginTransaction())
                        {
                            try
                            {
                                Character character = dbContext.Character.Where(i => i.Id == characterData.CharacterId).FirstOrDefault();
                                character.Name = characterData.CharacterName;
                                dbContext.SaveChanges();
                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                transaction.Rollback();
                                throw new Exception(ex.Message);
                            }
                        }
                    }
                });
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
