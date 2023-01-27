using HITteamBot.Repository.Entities.Base;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HITteamBot.Repository.Controllers.Base
{
    public class BaseController
    {
        public static Timer SetTimer(TimerCallback timerCallback, object state, int dueTime)
        {
            return new Timer(timerCallback, state, dueTime * 60000, Timeout.Infinite);
        }

        public static bool RemoveTimer(EventsTimer timer)
        {
            try
            {
                //timer.Timer?.Dispose();
                return Program.Events.Remove(timer);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string GetUserDirectory(string username)
        {
            return Program.UsersDirectory + $@"\{username}";
        }

        public static async Task<string> SetPermissions(string query)
        {
            try
            {
                string[] data = query.Trim().Split(new char[] { ' ' });
                string path = Program.ObjectsDirectory + $@"\Permissions.json";
                Permissions permission = new Permissions()
                {
                    Username = data[0],
                    Type = (PermissionsType)Enum.Parse(typeof(PermissionsType), data[1])
                };

                Task task = Task.Factory.StartNew(() =>
                {
                    List<Permissions> permissions = System.IO.File.Exists(path) ? JsonConvert.DeserializeObject<List<Permissions>>(System.IO.File.ReadAllText(path)) : new List<Permissions>();
                    permissions.Add(permission);
                    System.IO.File.WriteAllText(path, JsonConvert.SerializeObject(permissions));
                });

                await task;
                return $"Пользователю {permission.Username} выданы права уровня {permission.Type.ToString()}";
            }
            catch (Exception)
            {
                return "Ошибка";
            }
        }

        public static async Task<bool> CheckPermissions(string username, PermissionsType type)
        {
            try
            {
                string path = Program.ObjectsDirectory + $@"\Permissions.json";
                if (System.IO.File.Exists(path))
                {
                    List<Permissions> permissions = new List<Permissions>();
                    Task task = Task.Factory.StartNew(() =>
                    {
                        permissions = JsonConvert.DeserializeObject<List<Permissions>>(System.IO.File.ReadAllText(path));
                    });
                    await task;
                    if (permissions.Any(u => u.Username == username && u.Type == type)) return true;
                    else return false;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
